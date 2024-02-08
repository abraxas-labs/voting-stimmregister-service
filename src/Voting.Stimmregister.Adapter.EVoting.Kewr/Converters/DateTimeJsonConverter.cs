// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.Converters;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // The default JsonSerializer implementation only supports DateTimes that conform to ISO 8601-1:2019
        // This converter supports reading of all DateTimes that are parseable
        return DateTime.Parse(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}
