using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Docfx.ResultSnippets;
using Microsoft.Spark;
using Microsoft.Spark.Interop.Ipc;
using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Types;
using SparkTest.NET.Converters;

namespace SparkTest.NET.Extensions;

/// <summary>
/// Extension methods for working with the `DataFrame` API
/// </summary>
public static class DataFrameExtensions
{
    private static readonly JsonSerializerOptions Options =
        new()
        {
            Converters =
            {
                new JsonStringEnumConverter(),
                new SparkDateConverter(),
                new SparkTimestampConverter()
            }
        };

    private static bool IsEnumerable(this Type type, out Type? nestedType)
    {
        var enumerableInterface = Array.Find(
            type.GetInterfaces(),
            x =>
                x.IsGenericType
                && typeof(IEnumerable).IsAssignableFrom(x)
                && x.GenericTypeArguments.Length == 1
        );

        enumerableInterface =
            enumerableInterface == null && typeof(IEnumerable).IsAssignableFrom(type)
                ? type
                : enumerableInterface;

        if (enumerableInterface == null)
        {
            nestedType = null;
            return false;
        }

        nestedType = enumerableInterface.GenericTypeArguments[0];
        return true;
    }

    /// <summary>
    /// Converts a type to its spark type
    /// </summary>
    /// <param name="type">.NET type</param>
    /// <returns>spark data type</returns>
    /// <exception cref="NotSupportedException">if the type is not supported in spark</exception>
    [SuppressMessage("Design", "MA0051:Method is too long")]
    public static DataType AsSparkType(this Type type)
    {
        while (true)
        {
            if (typeof(bool).IsAssignableFrom(type))
                return new BooleanType();
            if (typeof(byte).IsAssignableFrom(type))
                return new ByteType();
            if (typeof(short).IsAssignableFrom(type))
                return new ShortType();
            if (typeof(int).IsAssignableFrom(type))
                return new IntegerType();
            if (typeof(long).IsAssignableFrom(type))
                return new LongType();
            if (typeof(float).IsAssignableFrom(type))
                return new FloatType();
            if (typeof(double).IsAssignableFrom(type))
                return new DoubleType();
            if (typeof(decimal).IsAssignableFrom(type))
                return new DecimalType();
            if (typeof(Date).IsAssignableFrom(type) || typeof(DateTime).IsAssignableFrom(type))
                return new DateType();
            if (
                typeof(Timestamp).IsAssignableFrom(type)
                || typeof(DateTimeOffset).IsAssignableFrom(type)
            )
            {
                return new TimestampType();
            }

            if (
                typeof(string).IsAssignableFrom(type)
                || typeof(char).IsAssignableFrom(type)
                || type.IsEnum
            )
            {
                return new StringType();
            }

            if (typeof(byte[]).IsAssignableFrom(type))
                return new BinaryType();
            if (
                type.IsEnumerable(out var ct1)
                && ct1 is { IsGenericType: true }
                && typeof(KeyValuePair<,>).IsAssignableFrom(ct1.GetGenericTypeDefinition())
                && ct1.GenericTypeArguments.Length > 1
            )
            {
                var keyType = ct1.GenericTypeArguments[0].AsSparkType();
                var valueType = ct1.GenericTypeArguments[1].AsSparkType();
                return new MapType(keyType, valueType);
            }

            if (type.IsEnumerable(out var ct2) && ct2 != null)
            {
                var innerType = ct2.AsSparkType();
                return new ArrayType(innerType);
            }

            if (type.IsClass)
                return type.AsStructType();
            if (type is not { IsGenericType: true, Namespace: not null })
                throw new NotSupportedException($"{type.FullName} is not supported in Spark");

            type = type.GenericTypeArguments[0];
        }
    }

    /// <summary>
    /// Creates a struct type from a given T
    /// </summary>
    /// <typeparam name="T">some T</typeparam>
    /// <returns>struct type</returns>
    /// <exception cref="NotSupportedException">if the type is not supported in spark</exception>
    [Pure]
    public static StructType CreateStructType<T>()
        where T : class => typeof(T).AsStructType();

    /// <summary>
    /// Creates a struct type from a given T
    /// </summary>
    /// <param name="_">instance of T</param>
    /// <typeparam name="T">some T</typeparam>
    /// <returns>struct type</returns>
    /// <exception cref="NotSupportedException">if the type is not supported in spark</exception>
    [Pure]
    public static StructType AsStructType<T>(this T _)
        where T : class => CreateStructType<T>();

    /// <summary>
    /// Creates a struct type from a given Type
    /// </summary>
    /// <param name="type">type to convert</param>
    /// <returns>struct type</returns>
    /// <exception cref="NotSupportedException">if the type is not supported in spark</exception>
    [Pure]
    public static StructType AsStructType(this Type type) =>
        new(
            type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(
                    x =>
                        x.PropertyType.IsGenericType
                            ? new StructField(
                                x.Name,
                                x.PropertyType.AsSparkType(),
                                Nullable.GetUnderlyingType(x.PropertyType.GenericTypeArguments[0])
                                    == null
                            )
                            : new StructField(
                                x.Name,
                                x.PropertyType.AsSparkType(),
                                Nullable.GetUnderlyingType(x.PropertyType) == null
                            )
                )
        );

    /// <summary>
    /// Creates an empty `DataFrame`
    /// </summary>
    /// <param name="session">spark session</param>
    /// <returns>empty frame</returns>
    [Pure]
    public static DataFrame CreateEmptyFrame(this SparkSession session) =>
        session.CreateDataFrame(
            new[] { new GenericRow(Array.Empty<object>()) },
            new StructType(Enumerable.Empty<StructField>())
        );

    /// <summary>
    /// Creates a `DataFrame` from a given TData loaded into spark as a JSON file
    /// </summary>
    /// <param name="session">session</param>
    /// <param name="first">first data item</param>
    /// <param name="rest">other data items</param>
    /// <typeparam name="TData">some TData reference type</typeparam>
    /// <returns>dataframe</returns>
    /// <exception cref="NotSupportedException">if the type is not supported in spark</exception>
    [Pure]
    public static DataFrame CreateDataFrameFromData<TData>(
        this SparkSession session,
        TData first,
        params TData[] rest
    )
        where TData : class => session.CreateDataFrameFromData(rest.Prepend(first));

    /// <summary>
    /// Creates a `DataFrame` from a given TData loaded into spark as a JSON file
    /// </summary>
    /// <param name="session">session</param>
    /// <param name="data">data</param>
    /// <typeparam name="TData">some TData reference type</typeparam>
    /// <returns>dataframe</returns>
    /// <exception cref="NotSupportedException">if the type is not supported in spark</exception>
    /// <exception cref="InvalidOperationException">if the enumerable is empty</exception>
    [Pure]
    public static DataFrame CreateDataFrameFromData<TData>(
        this SparkSession session,
        IEnumerable<TData> data
    )
        where TData : class
    {
        var lst = data.ToArray();
        if (lst.Length == 0)
            throw new InvalidOperationException("Enumerable must not be empty");

        var schema = CreateStructType<TData>();

        // write data to a temp file
        var fn = Guid.NewGuid().ToString();
        var pwd = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(pwd, fn);

        using (var fileStream = File.Create(filePath))
        {
            using (var utf8JsonWriter = new Utf8JsonWriter(fileStream))
            {
                JsonSerializer.Serialize(utf8JsonWriter, lst, Options);
            }
        }

        // load file into spark and cache and force execution
        var df = session.Read().Schema(schema).Json(filePath).Cache();
        _ = df.Collect().ToArray();

        // remove file
        File.Delete(filePath);
        return df;
    }

    /// <summary>
    /// Displays rows of the `DataFrame` in tabular form
    /// </summary>
    /// <param name="dataFrame">`DataFrame`</param>
    /// <param name="numRows">number of rows to show</param>
    /// <param name="truncate">
    /// if set to more than 0, truncates strings to `truncate`
    /// characters and all cells will be aligned right
    /// </param>
    /// <param name="vertical">
    /// If set to true, prints output rows vertically
    /// (one line per column value)
    /// </param>
    /// <returns>row data</returns>
    [Since("2.4.0")]
    public static string ShowString(
        this DataFrame dataFrame,
        int numRows = 20,
        int truncate = 20,
        bool vertical = false
    ) => (string)dataFrame.Reference.Invoke("showString", numRows, truncate, vertical);

    /// <summary>
    /// Displays rows of the `DataFrame` in tabular form in common mark format
    /// </summary>
    /// <remarks>Used to include `DataFrame` results in markdown documentation</remarks>
    /// <param name="dataFrame">`DataFrame`</param>
    /// <param name="numRows">number of rows to show</param>
    /// <param name="truncate">
    /// if set to more than 0, truncates strings to `truncate`
    /// characters and all cells will be aligned right
    /// </param>
    /// <param name="vertical">
    /// If set to true, prints output rows vertically
    /// (one line per column value)
    /// </param>
    /// <returns>row data in markdown format</returns>
    [Since("2.4.0")]
    public static string ShowMdString(
        this DataFrame dataFrame,
        int numRows = 20,
        int truncate = 0,
        bool vertical = false
    )
    {
        var sparkTabularFormat = dataFrame.ShowString(numRows, truncate, vertical);
        var parts = sparkTabularFormat.Split('\n');
        parts[2] = parts[2].Replace('+', '|');
        var table = parts
            .Skip(1)
            .Take(parts.Length - 3)
            .Aggregate(string.Empty, (s, s1) => $"{s}{s1}\n");

        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Results"] = $"{table}\n_(top = {numRows.ToString(CultureInfo.InvariantCulture)})_",
            ["Schema"] = dataFrame.PrintSchemaString().AsFencedResult("shell"),
            ["Plan"] = dataFrame
                .ExplainString()
                .ReIndexExplainPlan(removeIndexes: false)
                .AsFencedResult("shell"),
        }.AsTabResult();
    }

    /// <summary>
    /// Prints the schema to the console in a nice tree format
    /// </summary>
    /// <param name="dataFrame">`DataFrame`</param>
    /// <returns>schema</returns>
    [Since("2.4.0")]
    public static string PrintSchemaString(this DataFrame dataFrame) =>
        (string)((JvmObjectReference)dataFrame.Reference.Invoke("schema")).Invoke("treeString");

    /// <summary>
    /// Debugs a provided `DataFrame`
    /// </summary>
    /// <param name="dataFrame">`DataFrame`</param>
    /// <param name="numRows">number of rows to show</param>
    /// <param name="truncate">
    /// if set to more than 0, truncates strings to `truncate`
    /// characters and all cells will be aligned right
    /// </param>
    /// <param name="vertical">
    /// If set to true, prints output rows vertically
    /// (one line per column value)
    /// </param>
    /// <returns>`DataFrame` details</returns>
    [Since("2.4.0")]
    public static string Debug(
        this DataFrame dataFrame,
        int numRows = 20,
        int truncate = 0,
        bool vertical = false
    ) =>
        $"{dataFrame.PrintSchemaString()}\n\n(top = {numRows.ToString(CultureInfo.InvariantCulture)})\n{dataFrame.ShowString(numRows, truncate, vertical)}";

    /// <summary>
    /// Replaces the non-deterministic parts of an explain plan result
    /// </summary>
    /// <param name="result">result of calling explain</param>
    /// <param name="removeIndexes">flag to indicate that indexes should be removed, not re-indexed</param>
    /// <returns>re-indexed explain plan</returns>
    public static string ReIndexExplainPlan(this string result, bool removeIndexes)
    {
        // strip out all the # index values that increment over the life of a spark session
        var indexes = result
            .Split('#')
            .Select(part =>
            {
                var newIndex = new string(part.TakeWhile(char.IsDigit).ToArray());
                return newIndex.Length > 0 ? $"#{newIndex}" : null;
            })
            .OfType<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(_ => _, StringComparer.OrdinalIgnoreCase)
            .Select((index, i) => (Current: index, Replacement: $"#{i + 1}"))
            .OrderByDescending(_ => _);

        // replace them with stable indexes that are scoped to the current plan
        return indexes.Aggregate(
            result,
            (s, pair) =>
            {
                var target = $"({pair.Current})([^0-9]|$)";
                var replace = $"{(!removeIndexes ? pair.Replacement : string.Empty)}$2";
                return Regex.Replace(
                    s,
                    target,
                    replace,
                    RegexOptions.None,
                    TimeSpan.FromSeconds(1)
                );
            }
        );
    }

    /// <summary>
    /// Returns the plans (logical and physical) with a format specified by a given explain mode
    /// </summary>
    /// <param name="dataFrame">data frame</param>
    internal static string ExplainString(this DataFrame dataFrame)
    {
        var plan = (string)
            ((JvmObjectReference)dataFrame.Reference.Invoke("queryExecution")).Invoke(
                "explainString",
                (JvmObjectReference)
                    dataFrame.Reference.Jvm.CallStaticJavaMethod(
                        "org.apache.spark.sql.execution.ExplainMode",
                        "fromString",
                        "extended"
                    )
            );
        var parts = Directory
            .GetCurrentDirectory()
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return parts.Aggregate(plan, (p, pathPart) => p.Replace($"{pathPart}/", string.Empty));
    }
}
