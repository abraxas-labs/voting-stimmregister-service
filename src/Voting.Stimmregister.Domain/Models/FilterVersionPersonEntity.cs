// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

public class FilterVersionPersonEntity : AuditedEntity
{
    // References
    public Guid FilterVersionId { get; set; }

    public FilterVersionEntity? FilterVersion { get; set; }

    public Guid PersonId { get; set; }

    public PersonEntity? Person { get; set; }
}
