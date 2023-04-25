using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Spark.Sql.Types;

namespace SparkTest.NET.Converters;

[ExcludeFromCodeCoverage]
internal class SparkTimestampConverter : JsonConverter<Timestamp>
{
    public override Timestamp? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) =>
        DateTime.TryParse(
            reader.GetString(),
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var dt
        )
            ? new Timestamp(dt)
            : null;

    public override void Write(
        Utf8JsonWriter writer,
        Timestamp value,
        JsonSerializerOptions options
    ) =>
        writer.WriteStringValue(
            value.ToDateTime().ToString("O", CultureInfo.InvariantCulture)
        );
}
