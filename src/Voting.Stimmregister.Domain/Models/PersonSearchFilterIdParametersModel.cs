// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Database.Models;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// The person search filter-id model.
/// </summary>
public class PersonSearchFilterIdParametersModel
{
    /// <summary>
    /// Gets or sets the pagination model.
    /// </summary>
    public Pageable? Paging { get; set; }

    /// <summary>
    /// Gets or sets the filter id.
    /// </summary>
    public Guid FilterId { get; set; }

    /// <summary>
    /// Gets or sets the version id. The version id has higher precedence than the <see cref="FilterId"/>.
    /// </summary>
    public Guid? VersionId { get; set; }
}
