// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// The import statistics search parameters model.
/// </summary>
public class ImportStatisticSearchParametersModel
{
    /// <summary>
    /// Gets or sets the import type id.
    /// If not set, it will not be included into search query.
    /// </summary>
    public ImportType? ImportType { get; set; }

    /// <summary>
    /// Gets or sets the import source system.
    /// </summary>
    public ImportSourceSystem? ImportSourceSystem { get; set; }

    /// <summary>
    /// Gets or sets a list of import status to filter for.
    /// If list is empty, it will not be included into search query.
    /// </summary>
    public IReadOnlyCollection<ImportStatus>? ImportStatus { get; set; }

    /// <summary>
    /// Gets or sets a value indicating wether the source is manual upload or automated.
    /// True means 'manual'.
    /// False means 'automated'.
    /// Null means 'all'.
    /// </summary>
    public bool? IsManualUpload { get; set; }

    /// <summary>
    /// Gets or sets the municipality id to search for.
    /// If not set, it will not be included into search query.
    /// </summary>
    public int? MunicipalityId { get; set; }
}
