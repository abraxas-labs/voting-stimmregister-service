// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;

namespace Voting.Stimmregister.Domain.Models;

public class FilterEntity : AuditedEntity
{
    /// <summary>
    /// Gets or sets the name of the filter.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the filter.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public int MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the tenant id of the creator's tenant.
    /// <c>null</c> if the filter was created, before this feature was introduced.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the latest version of the filter.
    /// This value is not persisted and serves for ease of access of the latest version (by deadline).
    /// </summary>
    public FilterVersionEntity? LatestVersion { get; set; }

    /// <summary>
    /// Gets or sets the list of filter criteria.
    /// </summary>
    public ICollection<FilterVersionEntity> FilterVersions { get; set; } = new List<FilterVersionEntity>();

    /// <summary>
    /// Gets or sets the list of filter versions.
    /// </summary>
    public ICollection<FilterCriteriaEntity> FilterCriterias { get; set; } = new List<FilterCriteriaEntity>();
}
