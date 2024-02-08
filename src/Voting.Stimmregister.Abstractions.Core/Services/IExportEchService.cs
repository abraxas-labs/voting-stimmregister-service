// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// eCH export service.
/// </summary>
public interface IExportEchService
{
    /// <summary>
    /// Get the ech-0045 export file as <see cref="FileModel"/> including the result set from the passed search criterias.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <returns>A task wrapping the <see cref="FileModel"/>.</returns>
    Task<FileModel> ExportEch0045(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria);

    /// <summary>
    /// Get the ech-0045 export file as <see cref="FileModel"/> including the result set from the passed search filter.
    /// </summary>
    /// <param name="filterId">The id of the filter.</param>
    /// <returns>A task wrapping the <see cref="FileModel"/>.</returns>
    Task<FileModel> ExportEch0045ByFilter(Guid filterId);

    /// <summary>
    /// Get the ech-0045 export file as <see cref="FileModel"/> including the result set from the passed search filter.
    /// </summary>
    /// <param name="filterVersionId">The id of the version of filter.</param>
    /// <returns>A task wrapping the <see cref="FileModel"/>.</returns>
    Task<FileModel> ExportEch0045ByFilterVersion(Guid filterVersionId);
}
