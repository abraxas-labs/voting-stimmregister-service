// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models.EVoting;

public class EVotingAuditModel
{
    public short? BfsCanton { get; set; }

    public short? BfsMunicipality { get; set; }

    public bool? EVoterFlag { get; set; }

    public string? ContextId { get; set; }

    public string? StatusMessage { get; set; }

    public short? StatusCode { get; set; }

    public DateTime EVoterCreatedAt { get; set; }

    public string EVoterCreatedBy { get; set; } = string.Empty;

    public DateTime? EVoterModifiedAt { get; set; }

    public string? EVoterModifiedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}
