// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Text.Json.Serialization;

namespace Voting.Stimmregister.Adapter.EVoting.Loganto.Models;

public class EVoterFlagModel
{
    [JsonPropertyName("From")]
    public string? From { get; set; }

    [JsonPropertyName("To")]
    public string? To { get; set; }
}
