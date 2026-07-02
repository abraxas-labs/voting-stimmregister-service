// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Voting.Lib.Common;
using Voting.Lib.Iam.SecondFactor.Exceptions;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Resources;
using CoreSecondFactorTransactionService = Voting.Stimmregister.Abstractions.Core.Services.ISecondFactorTransactionService;
using ISecondFactorTransactionService = Voting.Lib.Iam.SecondFactor.Services.ISecondFactorTransactionService;
using SecondFactorTransactionInfo = Voting.Stimmregister.Domain.Models.SecondFactorTransactionInfo;

namespace Voting.Stimmregister.Core.Services.SecondFactor;

/// <inheritdoc cref="IManualImportSecondFactorTransactionService"/>
public class ManualImportSecondFactorService(
    ISecondFactorTransactionService secondFactorService,
    CoreSecondFactorTransactionService secondFactorTransactionService,
    IPermissionService permissionService,
    IMapper mapper)
    : IManualImportSecondFactorTransactionService
{
    public async Task<SecondFactorTransactionInfo> Prepare()
    {
        var actionId = ManualImportActionId();
        var info = await secondFactorService.Create(actionId, Strings.ManualImport_Prepare);
        return mapper.Map<SecondFactorTransactionInfo>(info);
    }

    public Task Verify(Guid transactionId, string? otpCode, CancellationToken ct)
        => secondFactorTransactionService.Verify(ManualImportActionId(), transactionId, otpCode, ct);

    public async Task EnsureVerifiedAndUse(Guid transactionId, CancellationToken ct)
    {
        // The transaction was already verified against the identity provider during the verify step.
        // The action id is bound to the action and the active tenant, but not to the uploaded file.
        // We therefore ensure the transaction was verified for this action and is only used once.
        if (!await secondFactorTransactionService.Use(ManualImportActionId(), transactionId, ct))
        {
            throw new SecondFactorTransactionNotVerifiedException();
        }
    }

    private IActionId ManualImportActionId() => ManualImportSecondFactorActionId.Create(permissionService.TenantId);
}
