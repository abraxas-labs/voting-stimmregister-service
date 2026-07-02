// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading;
using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Services;

public interface IManualImportSecondFactorTransactionService
{
    Task<SecondFactorTransactionInfo> Prepare();

    Task Verify(Guid transactionId, string? otpCode, CancellationToken ct);

    Task EnsureVerifiedAndUse(Guid transactionId, CancellationToken ct);
}
