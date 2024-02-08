// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.WebService.Models;

public class ImportRestApiResponse
{
    public ImportRestApiResponse(Guid jobId)
    {
        JobId = jobId;
    }

    public Guid JobId { get; }
}
