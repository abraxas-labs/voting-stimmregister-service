// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Text.Json.Serialization;

namespace Voting.Stimmregister.Adapter.EVoting.Loganto.Models;

public class RegisterModel
{
    [JsonPropertyName("Identification")]
    public IdentificationModel Identification { get; set; } = new IdentificationModel();

    [JsonPropertyName("EVoterFlag")]
    public EVoterFlagModel EVoterFlag { get; set; } = new EVoterFlagModel();
}
