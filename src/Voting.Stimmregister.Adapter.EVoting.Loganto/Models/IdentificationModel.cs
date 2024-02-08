// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Text.Json.Serialization;

namespace Voting.Stimmregister.Adapter.EVoting.Loganto.Models;

public class IdentificationModel
{
    [JsonPropertyName("Ahvn13")]
    public long Ahvn13 { get; set; }

    [JsonPropertyName("MunicipalityOeid")]
    public int MunicipalityOeid { get; set; }
}
