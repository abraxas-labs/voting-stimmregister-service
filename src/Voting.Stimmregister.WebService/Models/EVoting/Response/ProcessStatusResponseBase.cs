// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Constants.EVoting;

namespace Voting.Stimmregister.WebService.Models.EVoting.Response;

public class ProcessStatusResponseBase
{
    public ProcessStatusCode ProcessStatusCode { get; set; }

    public string ProcessStatusMessage { get; set; } = string.Empty;
}
