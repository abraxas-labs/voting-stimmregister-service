// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voting.Stimmregister.Test.Utils.Converters.JsonSerializer.Net;

/// <summary>
/// Workaround to serialize DateOnly since it is not yet supported.
/// </summary>
public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.FromDateTime(reader.GetDateTime());
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString("O");
        writer.WriteStringValue(isoDate);
    }
}
