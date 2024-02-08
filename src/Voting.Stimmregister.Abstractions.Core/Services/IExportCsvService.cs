// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// Service for file-donwload export csv.
/// </summary>
public interface IExportCsvService
{
    /// <summary>
    /// Get the csv export file as <see cref="FileModel"/> including the result set from the passed search criteria.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <returns>The <see cref="FileModel"/> representing the file download.</returns>
    Task<FileModel> ExportCsv(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria);

    /// <summary>
    /// Get the csv export file as <see cref="FileModel"/> including the result set from the filter resolved by its id.
    /// </summary>
    /// <param name="filterId">The filter id.</param>
    /// <returns>A task wrapping the <see cref="FileModel"/>.</returns>
    Task<FileModel> ExportCsvByFilter(Guid filterId);

    /// <summary>
    /// Get the csv export file as <see cref="FileModel"/> including the result set from the filter version resolved by its id.
    /// </summary>
    /// <param name="filterVersionId">The filter version id.</param>
    /// <returns>A task wrapping the <see cref="FileModel"/>.</returns>
    Task<FileModel> ExportCsvByFilterVersion(Guid filterVersionId);
}
