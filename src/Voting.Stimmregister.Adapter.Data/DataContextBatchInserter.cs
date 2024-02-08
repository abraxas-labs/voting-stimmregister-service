// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;

namespace Voting.Stimmregister.Adapter.Data;

public class DataContextBatchInserter : IBatchInserter
{
    // Value determined by local manual measurements
    private const int BatchSize = 100;

    private readonly IPermissionService _permissionService;
    private readonly DataContext _parent;
    private readonly DbContextOptions<DataContext> _dbContextOptions;

    public DataContextBatchInserter(
        IPermissionService permissionService,
        DataContext parent,
        DbContextOptions<DataContext> dbContextOptions)
    {
        _permissionService = permissionService;
        _parent = parent;
        _dbContextOptions = dbContextOptions;
    }

    public async Task InsertBatched<T>(IEnumerable<T> entities, CancellationToken ct)
        where T : class
    {
        var transaction = _parent.Database.CurrentTransaction
            ?? throw new InvalidOperationException("Insert batched requires an active transaction");

        // Because we are inserting a lot of entities
        // we need to take care that we do not allocate too much memory.
        // Using the same DbContext for all saves does not perform well, as all entries are being "cached".
        // Creating a new DbContext for each batch allows the GC to clean up data related to previous batches
        foreach (var entitiesChunk in entities.Chunk(BatchSize))
        {
            await using var dbContext = new DataContext(_dbContextOptions, _permissionService);
            dbContext.Database.SetDbConnection(_parent.Database.GetDbConnection());
            await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), ct);
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbContext.Set<T>().AddRange(entitiesChunk);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
