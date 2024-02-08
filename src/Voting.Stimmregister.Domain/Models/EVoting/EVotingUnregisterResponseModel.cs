// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models.EVoting;

public class EVotingUnregisterResponseModel
{
    public short ReturnCode { get; set; }

    public string Message { get; set; } = string.Empty;
}
