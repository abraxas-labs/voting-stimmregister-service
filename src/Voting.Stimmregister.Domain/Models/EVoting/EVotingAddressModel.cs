// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models.EVoting;

public class EVotingAddressModel
{
    public string Street { get; set; } = string.Empty;

    public string PostOfficeBoxText { get; set; } = string.Empty;

    public string HouseNumber { get; set; } = string.Empty;

    public string Town { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;
}
