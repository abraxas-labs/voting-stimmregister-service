// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.WebService.Utils;

namespace Voting.Stimmregister.WebService.Models.EVoting.Request;

public class RegistrationStatusRequest
{
    [Ahvn13(ErrorMessage = "Ungültige AHVN13 Nummer.")]
    public string Ahvn13 { get; set; } = string.Empty;

    public short BfsCanton { get; set; } = 0;
}
