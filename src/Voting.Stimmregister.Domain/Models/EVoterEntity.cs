// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models;

public class EVoterEntity : AuditedEntity
{
    public long Ahvn13 { get; set; }

    public short BfsCanton { get; set; }

    public short BfsMunicipality { get; set; }

    public bool? EVoterFlag { get; set; } = false;

    public string? ContextId { get; set; }
}
