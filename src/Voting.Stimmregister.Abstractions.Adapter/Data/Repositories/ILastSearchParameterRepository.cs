// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for <see cref="LastSearchParameterEntity"/>.
/// </summary>
public interface ILastSearchParameterRepository : IDbRepository<DbContext, LastSearchParameterEntity>
{
    /// <summary>
    /// Creates or replaces a last search parameter entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>A <see cref="Task"/> representing the async operation.</returns>
    Task CreateOrReplace(LastSearchParameterEntity entity);

    /// <summary>
    /// Fetches the last used parameter entities.
    /// No acl checks are made.
    /// </summary>
    /// <param name="searchType">The type of search.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>The found parameters or <c>null</c>.</returns>
    Task<LastSearchParameterEntity?> Fetch(PersonSearchType searchType, string userId, string tenantId);
}
