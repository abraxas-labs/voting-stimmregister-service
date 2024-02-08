// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// <see cref="BfsIntegrityEntity"/> database repository.
/// </summary>
public interface IBfsIntegrityRepository : IDbRepository<DbContext, BfsIntegrityEntity>
{
    /// <summary>
    /// Lists integrity entries for an <see cref="ImportType"/> and several bfs.
    /// </summary>
    /// <param name="importType">The import type.</param>
    /// <param name="bfs">The bfs to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>The integrity entries by bfs.</returns>
    Task<IReadOnlyDictionary<string, BfsIntegrityEntity>> ListForBfs(ImportType importType, IReadOnlyCollection<string> bfs, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a <see cref="BfsIntegrityEntity"/> for a certain bfs / <see cref="ImportType"/> combination.
    /// </summary>
    /// <param name="importType">The import type.</param>
    /// <param name="bfs">The bfs.</param>
    /// <returns>The found entity or <c>null</c> if none was found.</returns>
    Task<BfsIntegrityEntity?> Get(ImportType importType, string bfs);
}
