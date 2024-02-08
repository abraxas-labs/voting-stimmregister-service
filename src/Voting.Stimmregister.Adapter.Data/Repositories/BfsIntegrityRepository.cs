// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

/// <inheritdoc cref="IBfsIntegrityRepository"/>
public class BfsIntegrityRepository : DbRepository<DataContext, BfsIntegrityEntity>,
    IBfsIntegrityRepository
{
    public BfsIntegrityRepository(DataContext context)
        : base(context)
    {
    }

    /// <inheritdoc cref="IBfsIntegrityRepository.ListForBfs"/>
    public async Task<IReadOnlyDictionary<string, BfsIntegrityEntity>> ListForBfs(ImportType importType, IReadOnlyCollection<string> bfs, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(x => x.ImportType == importType && bfs.Contains(x.Bfs))
            .ToDictionaryAsync(x => x.Bfs, cancellationToken);
    }

    /// <inheritdoc cref="IBfsIntegrityRepository.Get"/>
    public async Task<BfsIntegrityEntity?> Get(ImportType importType, string bfs)
        => await Query().Where(x => x.ImportType == importType && x.Bfs == bfs).FirstOrDefaultAsync();
}
