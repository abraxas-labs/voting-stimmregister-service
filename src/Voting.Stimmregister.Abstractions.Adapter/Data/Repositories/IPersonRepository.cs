// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Models;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for person table.
/// </summary>
public interface IPersonRepository : IDbRepository<DbContext, PersonEntity>
{
    /// <summary>
    /// Gets a <see cref="Dictionary{TKey, TValue}"/> of latest persons by id, which have the passed municipality id (BFS number) in common.
    /// </summary>
    /// <param name="municipalityId">The municipality id (BFS number).</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
    Task<Dictionary<Guid, PersonEntity>> GetLatestPersonsByBfsNumberIgnoreAcl(int municipalityId);

    /// <summary>
    /// Streams the latest persons which aren't deleted by bfs.
    /// Ignores the acl.
    /// </summary>
    /// <param name="municipalityId">The bfs.</param>
    /// <returns>A stream of persons.</returns>
    IAsyncEnumerable<PersonEntity> StreamLatestByBfsIgnoreAclAndDeleted(int municipalityId);

    /// <summary>
    /// Gets a list of <see cref="PersonEntity"/> with filter by given criteria.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <param name="referenceKeyDate">The date with which all date relevant queries are to be executed. Example: At which date the age is calculated.</param>
    /// <param name="includeDois">Whether to load the domain of influences.</param>
    /// <param name="paging">Optional. If set, paging is applied with these parameters.</param>
    /// <returns>A page of resolved people.</returns>
    Task<PersonSearchResultPageModel<PersonEntity>> GetPersonByFilter(IReadOnlyCollection<FilterCriteriaEntity> criteria, DateOnly referenceKeyDate, bool includeDois, Pageable? paging);

    /// <summary>
    /// Gets the count of persons by given filter criteria.
    /// </summary>
    /// <param name="criteria">The criteria to apply.</param>
    /// <param name="referenceKeyDate">The date with which all date relevant queries are to be executed. Example: At which date the age is calculated.</param>
    /// <returns>The counts.</returns>
    Task<PersonCountsModel> GetCountsByFilter(IReadOnlyCollection<FilterCriteriaEntity> criteria, DateOnly referenceKeyDate);

    /// <summary>
    /// Gets a list of <see cref="PersonEntity"/> with filter by given criteria.
    /// </summary>
    /// <param name="filterVersionId">The id of the filter version.</param>
    /// <param name="paging">Optional. If set, paging is applied with these parameters.</param>
    /// <returns>A page of resolved people.</returns>
    Task<PersonSearchResultPageModel<PersonEntity>> GetPersonsByFilterVersionId(Guid filterVersionId, Pageable? paging);

    /// <summary>
    /// Gets the latest person with the specified register Id including all assigned domain of influences.
    /// Throws an exception if no person was found.
    /// </summary>
    /// <param name="personRegisterId">The register ID of the person to be searched.</param>
    /// <returns>The resolved person.</returns>
    Task<PersonEntity> GetPersonByRegisterIdIncludingDoIs(Guid personRegisterId);

    /// <summary>
    /// Gets the most recent person with voting rights based on the vn and canton bfs.
    /// If more than one person is found, the one with the most recent revision is selected.
    /// </summary>
    /// <param name="vn">The vn of the person to be searched.</param>
    /// <param name="cantonBfs">The canton bfs number of the person to be searched.</param>
    /// <returns>The resolved person.</returns>
    Task<PersonEntity?> GetMostRecentWithVotingRightsByVnAndCantonBfsIgnoreAcl(long vn, short cantonBfs);

    /// <summary>
    /// Returns all persons found by a filter version as an <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <param name="filterVersionId">The versionId of the filter.</param>
    /// <returns>The persons.</returns>
    IAsyncEnumerable<PersonEntity> StreamPersonsByFilterVersion(Guid filterVersionId);

    /// <summary>
    /// Returns all persons found by a filter version as an <see cref="IAsyncEnumerable{T}"/>
    /// Includes the number of entries.
    /// </summary>
    /// <param name="filterVersionId">The versionId of the filter.</param>
    /// <returns>The result container including the count and the data enumerable.</returns>
    Task<PersonSearchStreamedResultModel<PersonEntity>> StreamPersonsByFilterVersionWithCount(Guid filterVersionId);

    /// <summary>
    /// Lists all person ids including invalids matching provided filter criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="referenceKeyDate">The reference date.</param>
    /// <returns>The list of ids.</returns>
    Task<IReadOnlySet<Guid>> ListPersonIdsInclInvalid(IReadOnlyCollection<FilterCriteriaEntity> criteria, DateOnly referenceKeyDate);

    /// <summary>
    /// Gets a stream of <see cref="PersonEntity"/> filtered by given criteria.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <param name="referenceKeyDate">The date with which all date relevant queries are to be executed. Example: At which date the age is calculated.</param>
    /// <returns>The persons.</returns>
    IAsyncEnumerable<PersonEntity> StreamPersons(IReadOnlyCollection<FilterCriteriaEntity> criteria, DateOnly referenceKeyDate);

    /// <summary>
    /// Gets a stream of <see cref="PersonEntity"/> filtered by given criteria.
    /// Includes the number of entries.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <param name="referenceKeyDate">The date with which all date relevant queries are to be executed. Example: At which date the age is calculated.</param>
    /// <returns>The result container including the count and the data enumerable.</returns>
    Task<PersonSearchStreamedResultModel<PersonEntity>> StreamPersonsWithCounts(IReadOnlyCollection<FilterCriteriaEntity> criteria, DateOnly referenceKeyDate);
}
