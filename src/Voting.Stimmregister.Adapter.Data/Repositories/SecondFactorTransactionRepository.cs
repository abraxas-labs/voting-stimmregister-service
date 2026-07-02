// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

public class SecondFactorTransactionRepository(DataContext context)
    : DbRepository<DataContext, SecondFactorTransactionEntity>(context), ISecondFactorTransactionRepository
{
    public Task MarkVerified(Guid transactionId, CancellationToken ct = default)
    {
        return Set
            .Where(t => t.Id == transactionId)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.Verified, true), ct);
    }

    public async Task<bool> Use(Guid transactionId, string actionIdHash, CancellationToken ct = default)
    {
        var affectedRows = await Set
            .Where(t => t.Id == transactionId && t.Verified && !t.Used && t.ActionIdHash == actionIdHash)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.Used, true), ct);
        return affectedRows > 0;
    }
}
