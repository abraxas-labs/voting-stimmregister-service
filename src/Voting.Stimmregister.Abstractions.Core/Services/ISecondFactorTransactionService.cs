// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading;
using System.Threading.Tasks;
using Voting.Lib.Common;

namespace Voting.Stimmregister.Abstractions.Core.Services;

public interface ISecondFactorTransactionService
{
    Task Verify(IActionId actionId, Guid transactionId, string? otpCode, CancellationToken cancellationToken);

    /// <summary>
    /// Ensures a previously verified second factor transaction is used exactly once for the given action,
    /// without re-verifying against the identity provider.
    /// </summary>
    /// <param name="actionId">The action the transaction must be bound to.</param>
    /// <param name="transactionId">The id of the transaction.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether the transaction was verified, bound to the action and not yet used.</returns>
    Task<bool> Use(IActionId actionId, Guid transactionId, CancellationToken cancellationToken);
}
