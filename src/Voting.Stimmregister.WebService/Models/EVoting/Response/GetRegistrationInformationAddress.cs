// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.WebService.Models.EVoting.Response;

public class GetRegistrationInformationAddress
{
    public string Street { get; set; } = string.Empty;

    public string PostOfficeBoxText { get; set; } = string.Empty;

    public string HouseNumber { get; set; } = string.Empty;

    public string Town { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;
}
