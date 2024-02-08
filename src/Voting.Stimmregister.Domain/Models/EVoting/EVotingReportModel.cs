// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;

namespace Voting.Stimmregister.Domain.Models.EVoting;

public class EVotingReportModel
{
    public string Ahvn13 { get; set; } = string.Empty;

    public short? BfsCanton { get; set; }

    public short? BfsMunicipality { get; set; }

    public bool? EVoterFlag { get; set; }

    public string? ContextId { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public ICollection<EVotingAuditModel> Audits { get; set; } = new HashSet<EVotingAuditModel>();
}
