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
    /// <typeparam name="T">The export model type.</typeparam>
    /// <param name="criteria">The search criteria.</param>
    /// <param name="options">The export options.</param>
    /// <returns>The <see cref="FileModel"/> representing the file download.</returns>
    Task<FileModel> ExportCsv<T>(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria, ExportCsvOptions options);

    /// <summary>
    /// Get the csv export file as <see cref="FileModel"/> including the result set from the filter resolved by its id.
    /// </summary>
    /// <typeparam name="T">The export model type.</typeparam>
    /// <param name="filterId">The filter id.</param>
    /// <param name="options">The export options.</param>
    /// <returns>A task wrapping the <see cref="FileModel"/>.</returns>
    Task<FileModel> ExportCsvByFilter<T>(Guid filterId, ExportCsvOptions options);

    /// <summary>
    /// Get the csv export file as <see cref="FileModel"/> including the result set from the filter version resolved by its id.
    /// </summary>
    /// <typeparam name="T">The export model type.</typeparam>
    /// <param name="filterVersionId">The filter version id.</param>
    /// <param name="options">The export options.</param>
    /// <returns>A task wrapping the <see cref="FileModel"/>.</returns>
    Task<FileModel> ExportCsvByFilterVersion<T>(Guid filterVersionId, ExportCsvOptions options);
}
