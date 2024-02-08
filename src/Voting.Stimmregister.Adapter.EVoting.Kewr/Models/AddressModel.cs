// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.Models;

public class AddressModel
{
    public string Street { get; set; } = string.Empty;

    public string HouseNumber { get; set; } = string.Empty;

    public string Town { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public DateTime ValidFrom { get; set; }

    public DateTime ValidUntil { get; set; }
}
