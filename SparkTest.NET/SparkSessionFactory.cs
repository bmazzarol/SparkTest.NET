using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Spark.Sql;

namespace SparkTest.NET;

using SparkSessions = Dictionary<string, SparkSession>;

/// <summary>
/// Provides access to spark sessions backed by a running spark-debug process
/// </summary>
[ExcludeFromCodeCoverage]
public static class SparkSessionFactory
{
    private static readonly object Lock = new();

    private static SparkSessions? _sparkSessions;
    private static SparkSession? _defaultSession;

    private static SparkSessions Initialize(Assembly callingAssembly)
    {
        if (_sparkSessions != null && _defaultSession != null)
            return _sparkSessions;

        lock (Lock)
        {
            _sparkSessions = new SparkSessions(StringComparer.CurrentCultureIgnoreCase);

            // get the metadata from the calling assembly
            var metadata = callingAssembly
                .GetCustomAttributes(typeof(AssemblyMetadataAttribute))
                .OfType<AssemblyMetadataAttribute>();

            // start the spark-debug process
            var process = TryStartSparkDebug(metadata);

            // set default session
            _defaultSession = GetOrCreate(nameof(_defaultSession));
            _sparkSessions[nameof(_defaultSession)] = _defaultSession;

            // shutdown hook
            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                foreach (var kvp in _sparkSessions)
                    kvp.Value.Stop();

                if (process == null)
                    return;

                // CSparkRunner will exit upon receiving newline from
                // the standard input stream.
                process.StandardInput.WriteLine("done");
                process.StandardInput.Flush();
                process.WaitForExit();
            };

            return _sparkSessions;
        }
    }

    /// <summary>
    /// Uses a spark session and returns the result
    /// </summary>
    /// <param name="fn">function that takes a session and returns a T</param>
    /// <param name="appName">optional app name</param>
    /// <typeparam name="T">some T</typeparam>
    /// <returns>T</returns>
    public static Task<T> UseSession<T>(Func<SparkSession, Task<T>> fn, string? appName = default)
    {
        lock (Lock)
        {
            return fn(GetOrCreateSession(appName, Assembly.GetCallingAssembly()));
        }
    }

    /// <summary>
    /// Uses a spark session and returns the result
    /// </summary>
    /// <param name="fn">function that takes a session and returns a T</param>
    /// <param name="appName">optional app name</param>
    /// <typeparam name="T">some T</typeparam>
    /// <returns>T</returns>
    public static T UseSession<T>(Func<SparkSession, T> fn, string? appName = default)
    {
        lock (Lock)
        {
            return fn(GetOrCreateSession(appName, Assembly.GetCallingAssembly()));
        }
    }

    private static SparkSession GetOrCreateSession(string? appName, Assembly callingAssembly)
    {
        var key = appName ?? nameof(_defaultSession);
        var sessions = Initialize(callingAssembly);

        if (sessions.TryGetValue(key, out var session))
            return session;

        session = GetOrCreate(appName);
        sessions[key] = session;
        return session;
    }

    private static SparkSession GetOrCreate(string? appName) =>
        SparkSession
            .Builder()
            .Config("spark.sql.session.timeZone", "UTC")
            .Config("spark.ui.enabled", value: false)
            .Config("spark.ui.showConsoleProgress", value: false)
            .AppName(appName)
            .GetOrCreate();

    private static Process? TryStartSparkDebug(IEnumerable<AssemblyMetadataAttribute> metadata)
    {
        // if we are running in a CI env dont try start spark-submit
        if (
            string.Equals(
                Environment.GetEnvironmentVariable("DISABLE_AUTO_SPARK_DEBUG"),
                "true",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return null;
        }

        // if the user already has spark running lets not try and start it
        try
        {
            _ = GetOrCreate("test");
            return null;
        }
        catch
        {
            // no-op
        }

        // load the assembly attribute if its set
        var config = new SparkSessionFactoryConfig(metadata);
        return Process(config.SparkHome, config.SparkDotnetJarName, config.ExtraJars);
    }

    private static Process Process(string sparkHome, string sparkJarName, string? extraJars)
    {
        var process = new Process();
        process.StartInfo.FileName =
            Path.Combine(sparkHome, "bin", "spark-submit")
            + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".cmd" : string.Empty);
        process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

        WriteLogConfiguration(process);

        process.StartInfo.Arguments = string.Join(
            " ",
            "--class org.apache.spark.deploy.dotnet.DotnetRunner",
            string.IsNullOrWhiteSpace(extraJars) ? string.Empty : $"--jars {extraJars}",
            "--conf \"spark.driver.extraJavaOptions=-Dlog4j.configuration=file:log4j.properties\"",
            "--conf \"spark.executor.extraJavaOptions=-Dlog4j.configuration=file:log4j.properties\"",
            "--conf \"spark.deploy.spreadOut=false\"",
            "--conf \"spark.shuffle.service.db.enabled=false\"",
            "--conf \"spark.sql.shuffle.partitions=1\"",
            $"--master local[*] {sparkJarName} debug"
        );
        // UseShellExecute defaults to true in .NET Framework,
        // but defaults to false in .NET Core. To support both, set it
        // to false which is required for stream redirection.
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        var isSparkReady = false;
        process.OutputDataReceived += (_, arguments) =>
        {
            // Scala-side driver for .NET emits the following message after it is
            // launched and ready to accept connections.
            if (!isSparkReady && arguments.Data?.Contains("Backend running debug mode") == true)
            {
                isSparkReady = true;
            }
        };

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        var processExited = false;
        while (!isSparkReady && !processExited)
        {
            processExited = process.WaitForExit(500);
        }

        if (!processExited)
            return process;

        process.Dispose();

        // The process should not have been exited.
        throw new InvalidOperationException(
            $"Process exited prematurely with '{process.StartInfo.FileName} {process.StartInfo.Arguments}' in working directory '{process.StartInfo.WorkingDirectory}'"
        );
    }

    private static void WriteLogConfiguration(Process process) =>
        File.WriteAllText(
            Path.Combine(process.StartInfo.WorkingDirectory, "log4j.properties"),
            @"
log4j.rootCategory=ERROR, console
log4j.appender.console=org.apache.log4j.ConsoleAppender
log4j.appender.console.target=System.err
log4j.appender.console.layout=org.apache.log4j.PatternLayout
log4j.appender.console.layout.ConversionPattern=%d{yy/MM/dd HH:mm:ss} %p %c{1}: %m%n"
        );
}
