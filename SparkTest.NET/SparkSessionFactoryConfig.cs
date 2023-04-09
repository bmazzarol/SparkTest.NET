using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace SparkTest.NET;

/// <summary>
/// Configuration elements for the spark session factory
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class SparkSessionFactoryConfig
{
    public string SparkHome { get; }
    public string SparkDotnetJarName { get; }
    public string? ExtraJars { get; }

    public SparkSessionFactoryConfig(IEnumerable<AssemblyMetadataAttribute> attributes)
    {
        var metadata = attributes.ToList();
        SparkHome = GetFromAttributeOrEnvOrThrow(metadata, "SparkTest.NET.SparkHome", "SPARK_HOME");
        SparkDotnetJarName = GetFromAttributeOrEnvOrThrow(
            metadata,
            "SparkTest.NET.SparkDotnetJarName",
            "SPARK_DOTNET_JAR_NAME"
        );
        ExtraJars = GetFromAttributeOrEnv(
            metadata,
            "SparkTest.NET.ExtraJars",
            "SPARK_DEBUG_EXTRA_JARS"
        );
    }

    private static string? GetFromAttributeOrEnv(
        List<AssemblyMetadataAttribute> metadata,
        string metaDataName,
        string envName
    )
    {
        var mv = metadata.Find(
            x => string.Equals(x.Key, metaDataName, StringComparison.OrdinalIgnoreCase)
        );
        var result = mv?.Value ?? Environment.GetEnvironmentVariable(envName);
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    private static string GetFromAttributeOrEnvOrThrow(
        List<AssemblyMetadataAttribute> metadata,
        string metaDataName,
        string envName
    )
    {
        var result = GetFromAttributeOrEnv(metadata, metaDataName, envName);

        if (result == null)
        {
            throw new InvalidOperationException(
                $"An AssemblyMetadata attribute for '{metaDataName}' or Environment variable '{envName}' must be set."
            );
        }

        return result;
    }
}
