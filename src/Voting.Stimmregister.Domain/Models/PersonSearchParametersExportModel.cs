// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// The export person filters model.
/// </summary>
public class PersonSearchParametersExportModel
{
    /// <summary>
    /// Gets or sets the filter id.
    /// </summary>
    public Guid? FilterId { get; set; }

    /// <summary>
    /// Gets or sets the version id.
    /// </summary>
    public Guid? VersionId { get; set; }

    /// <summary>
    /// Gets or sets the search filter criteria model.
    /// </summary>
    public List<PersonSearchFilterCriteriaModel> Criteria { get; set; } = new();
}
