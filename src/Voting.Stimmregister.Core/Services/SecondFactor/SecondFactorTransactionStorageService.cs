// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;
using LibSecondFactorTransaction = Voting.Lib.Iam.SecondFactor.Models.SecondFactorTransaction;

namespace Voting.Stimmregister.Core.Services.SecondFactor;

public class SecondFactorTransactionStorageService(
    ISecondFactorTransactionRepository repo,
    IMapper mapper,
    TimeProvider timeProvider)
    : Lib.Iam.SecondFactor.Services.ISecondFactorTransactionRepository
{
    public async Task<LibSecondFactorTransaction> GetById(Guid transactionId)
    {
        var entity = await repo.GetByKey(transactionId)
            ?? throw new EntityNotFoundException(nameof(SecondFactorTransactionEntity), transactionId);
        return mapper.Map<LibSecondFactorTransaction>(entity);
    }

    public Task Create(LibSecondFactorTransaction transaction) => repo.Create(mapper.Map<SecondFactorTransactionEntity>(transaction));

    public async Task Update(LibSecondFactorTransaction transaction)
    {
        var entity = await repo.GetByKey(transaction.Id)
            ?? throw new EntityNotFoundException(nameof(SecondFactorTransactionEntity), transaction.Id);
        mapper.Map(transaction, entity);
        await repo.Update(entity);
    }

    public Task DeleteExpired()
    {
        return repo.Query()
            .Where(x => x.ExpireAt < timeProvider.GetUtcNowDateTime())
            .ExecuteDeleteAsync();
    }
}
