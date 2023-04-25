using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Spark.Sql.Types;

namespace SparkTest.NET.Converters;

[ExcludeFromCodeCoverage]
internal class SparkDateConverter : JsonConverter<Date>
{
    public override Date? Read(
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
            ? new Date(dt)
            : null;

    public override void Write(Utf8JsonWriter writer, Date value, JsonSerializerOptions options) =>
        writer.WriteStringValue(
            value.ToDateTime().ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        );
}
