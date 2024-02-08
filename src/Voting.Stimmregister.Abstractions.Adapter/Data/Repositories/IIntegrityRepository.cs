// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for integrity table.
/// </summary>
public interface IIntegrityRepository : IDbRepository<DbContext, BfsIntegrityEntity>
{
    /// <summary>
    /// Creates or Updates the provided entity in the database by the property Bfs.
    /// </summary>
    /// <param name="integrityEntity">The entity to create or update.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing the asynchronous operation.</returns>
    Task CreateOrUpdate(BfsIntegrityEntity integrityEntity);
}
