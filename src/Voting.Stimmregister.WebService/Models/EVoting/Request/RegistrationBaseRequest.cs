// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.WebService.Models.EVoting.Request;

public class RegistrationBaseRequest
{
    public string Ahvn13 { get; set; } = string.Empty;

    public short BfsCanton { get; set; }
}
