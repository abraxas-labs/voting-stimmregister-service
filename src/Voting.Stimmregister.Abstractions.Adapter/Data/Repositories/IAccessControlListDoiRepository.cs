// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for domain of influences based access control list.
/// </summary>
public interface IAccessControlListDoiRepository : IDbRepository<DbContext, AccessControlListDoiEntity>
{
    /// <summary>
    /// Gets the canton abbreviation which has the municipality id (BFS number) in common.
    /// </summary>
    /// <param name="municipalityId">The municipality id (BFS number).</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Canton> GetCantonByBfsNumber(int municipalityId);

    /// <summary>
    /// Gets the bfs of a canton.
    /// </summary>
    /// <param name="canton">The canton.</param>
    /// <returns>The found bfs, or null if the bfs could not be resolved.</returns>
    Task<string?> GetCantonBfsByCanton(Canton canton);
}
