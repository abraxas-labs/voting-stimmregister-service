// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading;
using System.Threading.Tasks;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

public interface ISecondFactorTransactionRepository : IDbRepository<Microsoft.EntityFrameworkCore.DbContext, SecondFactorTransactionEntity>
{
    /// <summary>
    /// Marks a second factor transaction as verified against the identity provider.
    /// The identity provider verification is single-use, so the result is persisted to allow the
    /// subsequent action to confirm verification without re-verifying.
    /// </summary>
    /// <param name="transactionId">The id of the transaction.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task MarkVerified(Guid transactionId, CancellationToken ct = default);

    /// <summary>
    /// "Uses" a second factor transaction.
    /// This means ensuring the transaction is verified and not yet used.
    /// It is then marked as used so it cannot be used again.
    /// </summary>
    /// <param name="transactionId">The id of the transaction.</param>
    /// <param name="actionIdHash">The hash of the action the transaction must be bound to.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>Whether the transaction was verified, bound to the action and not yet used.</returns>
    Task<bool> Use(Guid transactionId, string actionIdHash, CancellationToken ct = default);
}
