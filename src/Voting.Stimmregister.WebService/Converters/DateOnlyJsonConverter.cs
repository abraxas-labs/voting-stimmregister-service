// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voting.Stimmregister.WebService.Converters;

// Parsing DateOnly is only supported from .NET 7 onwards
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.Parse(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("O"));
    }
}
