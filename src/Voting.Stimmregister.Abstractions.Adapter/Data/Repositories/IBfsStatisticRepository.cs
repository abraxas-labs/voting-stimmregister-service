// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for domain of influence statistics.
/// </summary>
public interface IBfsStatisticRepository : IDbRepository<DbContext, BfsStatisticEntity>
{
    /// <summary>
    /// Creates or Updates the provided entity in the database by the property Bfs.
    /// </summary>
    /// <param name="bfsStatisticEntity">The entity to create or update.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing the asynchronous operation.</returns>
    Task CreateOrUpdate(BfsStatisticEntity bfsStatisticEntity);
}
