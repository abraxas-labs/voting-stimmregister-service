// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.Models;

public class PersonIdModel
{
    public int ResidentNr { get; set; }

    public string Status { get; set; } = string.Empty; // I für Inaktiv / A Für Aktive

    public string Mv { get; set; } = string.Empty; // [HWS, NWS, KWS]

    public int OeId { get; set; }
}
