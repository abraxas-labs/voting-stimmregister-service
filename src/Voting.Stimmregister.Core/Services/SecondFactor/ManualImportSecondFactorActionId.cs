// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Common;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.SecondFactor;

/// <summary>
/// Builds the <see cref="IActionId"/> bound to the manual import second factor action.
/// The active tenant id is included as an action parameter so a second factor transaction
/// is bound to the tenant it was prepared for and cannot be reused to authorize an import
/// for another tenant the user happens to be authorized for.
/// </summary>
public static class ManualImportSecondFactorActionId
{
    public static IActionId Create(string tenantId)
        => DescriptorActionId.Create(SecondFactorTransactionActionTypes.ManualImport, tenantId);
}
