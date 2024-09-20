// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// Service to search filter data.
/// </summary>
public interface IFilterService
{
    /// <summary>
    /// Gets a list of filters according to passed filters wrapped in a search result model containing also the full results count.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <returns>A list of resolved filters.</returns>
    Task<List<FilterEntity>> GetAll(FilterSearchParametersModel searchParameters);

    /// <summary>
    /// Get a single filter by id.
    /// </summary>
    /// <param name="id">id of the filter.</param>
    /// <returns>A single filter.</returns>
    Task<FilterEntity> GetSingleById(Guid id);

    /// <summary>
    /// Previews metadata of a filter at a given deadline.
    /// </summary>
    /// <param name="id">The id of the filter.</param>
    /// <param name="deadline">The deadline.</param>
    /// <returns>The metadata.</returns>
    Task<FilterMetadataModel> GetMetadata(Guid id, DateOnly deadline);

    /// <summary>
    /// Get a single filter version including the associated filter by the id of the filter version.
    /// </summary>
    /// <param name="versionId">id of the filter version.</param>
    /// <returns>A single filter version including the associated filter.</returns>
    Task<FilterVersionEntity> GetSingleVersionInclFilterByVersionId(Guid versionId);

    /// <summary>
    /// Delete a single filter by id.
    /// </summary>
    /// <param name="id">id of the filter.</param>
    /// <param name="municipalityId">main municipality id of the user.</param>
    /// <returns>task for async action.</returns>
    Task DeleteSingleById(Guid id, int municipalityId);

    /// <summary>
    /// Duplicate a single filter by id.
    /// </summary>
    /// <param name="id">id of the filter.</param>
    /// <param name="municipalityId">main municipality id of the user.</param>
    /// <returns>The id of the new filter.</returns>
    Task<Guid> DuplicateSingleById(Guid id, int municipalityId);

    /// <summary>
    /// Save a filter.
    /// </summary>
    /// <param name="filter">filter to save.</param>
    /// <param name="municipalityId">main municipality id of the user.</param>
    /// <returns>task for async action.</returns>
    Task<Guid> Save(FilterEntity filter, int municipalityId);

    /// <summary>
    /// Create a filter version.
    /// </summary>
    /// <param name="filterVersionEntity">filter version model to save.</param>
    /// <param name="ct">A <see cref="CancellationToken"/>.</param>
    /// <returns>The id of the created version.</returns>
    Task<Guid> CreateVersion(FilterVersionEntity filterVersionEntity, CancellationToken ct);

    /// <summary>
    /// Rename a filter version.
    /// </summary>
    /// <param name="filterVersionId">the filter version id.</param>
    /// <param name="name">the new filter version name.</param>
    /// <param name="ct">A <see cref="CancellationToken"/>.</param>
    /// <returns>task for async action.</returns>
    Task RenameVersion(Guid filterVersionId, string name, CancellationToken ct);

    /// <summary>
    /// Delete a filter version by id.
    /// </summary>
    /// <param name="id">id of the filter version.</param>
    /// <returns>task for async action.</returns>
    Task DeleteVersionById(Guid id);
}
