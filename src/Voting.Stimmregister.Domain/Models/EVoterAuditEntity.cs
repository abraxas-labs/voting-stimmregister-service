// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

public class EVoterAuditEntity : AuditedEntity
{
    public short BfsCanton { get; set; }

    public short? BfsMunicipality { get; set; }

    public bool? EVoterFlag { get; set; }

    public string? ContextId { get; set; }

    public string? StatusMessage { get; set; }

    public short? StatusCode { get; set; }

    public Guid EVoterId { get; set; }

    public AuditInfo EVoterAuditInfo { get; set; } = new();
}
