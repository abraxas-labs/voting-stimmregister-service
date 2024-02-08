// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Models.EVoting;

namespace Voting.Stimmregister.WebService.Models.EVoting.Response;

public class GetReportResponse : ProcessStatusResponseBase
{
    public string Ahvn13 { get; set; } = string.Empty;

    public short? BfsCanton { get; set; }

    public short? BfsMunicipality { get; set; }

    public bool? EVoterFlag { get; set; }

    public string? ContextId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public ICollection<EVotingAuditModel> Audits { get; set; } = new HashSet<EVotingAuditModel>();
}
