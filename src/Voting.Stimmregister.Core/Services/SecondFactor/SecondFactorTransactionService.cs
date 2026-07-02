// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading;
using System.Threading.Tasks;
using Voting.Lib.Common;
using Voting.Lib.Iam.Services.ApiClient.Identity;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Core.Services;
using LibSecondFactorTransactionService = Voting.Lib.Iam.SecondFactor.Services.ISecondFactorTransactionService;

namespace Voting.Stimmregister.Core.Services.SecondFactor;

public class SecondFactorTransactionService(
    LibSecondFactorTransactionService libService,
    ISecondFactorTransactionRepository repo)
    : ISecondFactorTransactionService
{
    public async Task Verify(IActionId actionId, Guid transactionId, string? otpCode, CancellationToken cancellationToken)
    {
        await libService.EnsureVerified(
            transactionId,
            string.IsNullOrEmpty(otpCode) ? V1SecondFactorProvider.NEVIS : V1SecondFactorProvider.OTP,
            () => Task.FromResult(actionId),
            otpCode,
            cancellationToken);

        await repo.MarkVerified(transactionId, cancellationToken);
    }

    public Task<bool> Use(IActionId actionId, Guid transactionId, CancellationToken cancellationToken)
        => repo.Use(transactionId, actionId.ComputeHash(), cancellationToken);
}
