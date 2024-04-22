// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;

namespace Voting.Stimmregister.Domain.Models;

public class FilterVersionEntity : BaseEntityWithSignature, IAuditedEntity
{
    /// <summary>
    /// Gets or sets the audit info of this entity.
    /// </summary>
    public AuditInfo AuditInfo { get; set; } = new();

    /// <summary>
    /// Gets or sets the name of the filter version.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the deadline of the filter version.
    /// </summary>
    public DateTime Deadline { get; set; }

    /// <summary>
    /// Gets or sets the count of the filter version.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the count of invalid persons assigned to this filter version.
    /// </summary>
    public int CountOfInvalidPersons { get; set; }

    public HashSet<FilterVersionPersonEntity> FilterVersionPersons { get; set; } = new();

    public HashSet<FilterCriteriaEntity> FilterCriterias { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter id.
    /// </summary>
    public Guid FilterId { get; set; }

    public FilterEntity? Filter { get; set; }
}
