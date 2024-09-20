// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// Service to search person data.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Lists the last used search parameters of the current user on the current tenant.
    /// </summary>
    /// <param name="searchType">The search type intention.</param>
    /// <returns>The list of criteria.</returns>
    Task<LastSearchParameterEntity> GetLastUsedParameters(PersonSearchType searchType);

    /// <summary>
    /// Gets a list of persons according to passed filters wrapped in a search result model containing also the full results count.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <param name="requiredValidPageSize">if set to true, a page validation value is checked, which must be between 1 and 100.</param>
    /// <param name="includeDois">Whether to include the domain of influences.</param>
    /// <returns>A list of resolved people.</returns>
    Task<PersonSearchResultPageModel<PersonEntityModel>> GetAll(PersonSearchParametersModel searchParameters, bool requiredValidPageSize = true, bool includeDois = false);

    /// <summary>
    /// Gets the data of a single person based on the register Id including all it's assigned domain of influences.
    /// </summary>
    /// <param name="personRegisterId">The register Id of the person to be searched.</param>
    /// <returns>A resolved person.</returns>
    Task<PersonEntityModel> GetPersonModelIncludingDoIs(Guid personRegisterId);

    /// <summary>
    /// Gets the data of a single person based on a PersonEntity object.
    /// </summary>
    /// <param name="person">The person entity.</param>
    /// <param name="includeComputedInfos">Whether to include computed infos.</param>
    /// <param name="includeActuality">Whether to include actuality.</param>
    /// <returns>The person.</returns>
    Task<PersonEntityModel> GetPersonModelFromEntity(PersonEntity person, bool includeComputedInfos, bool includeActuality);

    /// <summary>
    /// Gets the data of a single person with voting rights based on the vn and canton bfs.
    /// </summary>
    /// <param name="vn">The vn of the person to be searched.</param>
    /// <param name="cantonBfs">The canton bfs number of the person to be searched.</param>
    /// <returns>A resolved person.</returns>
    Task<PersonEntityModel?> GetMostRecentWithVotingRightsByVnAndCantonBfs(long vn, short cantonBfs);

    /// <summary>
    /// Gets a list of persons with filter version id.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <param name="requiredValidPageSize">if set to true, a page validation value is checked, which must be between 1 and 100.</param>
    /// <param name="includeDois">Whether to include the domain of influences.</param>
    /// <returns>A list of resolved people.</returns>
    Task<PersonSearchResultPageModel<PersonEntityModel>> GetByFilterVersionId(PersonSearchFilterIdParametersModel searchParameters, bool requiredValidPageSize = true, bool includeDois = false);

    /// <summary>
    /// Gets a stream of <see cref="PersonEntity"/> filtered by given criteria.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <returns>The result container including the count and the data enumerable.</returns>
    IAsyncEnumerable<PersonEntity> StreamAll(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria);

    /// <summary>
    /// Gets a stream of <see cref="PersonEntity"/> filtered by given criteria.
    /// Includes the total number of matched persons and the number of invalid persons.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <returns>The result container including the count and the data enumerable.</returns>
    Task<PersonSearchStreamedResultModel<PersonEntity>> StreamAllWithCounts(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria);

    /// <summary>
    /// Gets a stream of <see cref="PersonEntity"/> filtered by the given filter.
    /// </summary>
    /// <param name="filterId">The id of the filter.</param>
    /// <returns>The result container including the count and the data enumerable.</returns>
    Task<IAsyncEnumerable<PersonEntity>> StreamAllByFilter(Guid filterId);

    /// <summary>
    /// Gets a stream of <see cref="PersonEntity"/> filtered by the given filter.
    /// Includes the total number of matched persons and the number of invalid persons.
    /// </summary>
    /// <param name="filterId">The id of the filter.</param>
    /// <returns>The result container including the count and the data enumerable.</returns>
    Task<PersonSearchStreamedResultModel<PersonEntity>> StreamAllWithCountsByFilter(Guid filterId);

    /// <summary>
    /// Gets a stream of <see cref="PersonEntity"/> filtered by the given version of the filter.
    /// </summary>
    /// <param name="filterVersionId">The id of the version of filter.</param>
    /// <returns>The persons.</returns>
    IAsyncEnumerable<PersonEntity> StreamAllByFilterVersion(Guid filterVersionId);

    /// <summary>
    /// Gets a stream of <see cref="PersonEntity"/> filtered by the given version of the filter.
    /// Includes the total number of matched persons and the number of invalid persons.
    /// </summary>
    /// <param name="filterVersionId">The id of the version of filter.</param>
    /// <returns>The result container including the count and the data enumerable.</returns>
    Task<PersonSearchStreamedResultModel<PersonEntity>> StreamAllWithCountsByFilterVersion(Guid filterVersionId);
}
