// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Adapter.EVoting.Loganto.Models;

public class StatusResponseModel
{
    public short ReturnCode { get; set; }

    public string Message { get; set; } = string.Empty;
}
