// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData.EVoting;

public static class EVotingAuditMockedData
{
    public static EVoterAuditEntity EVoterAudit1 => new()
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsStGallen,
        ContextId = "1",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterAuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };

    public static EVoterAuditEntity EVoterAudit2 => new()
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsStGallen,
        ContextId = "1",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterAuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = false,
    };

    public static EVoterAuditEntity EVoterAudit3 => new()
    {
        Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsStGallen,
        ContextId = "2",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterAuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };

    public static EVoterAuditEntity EVoterAudit4 => new()
    {
        Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsGoldach,
        ContextId = "3",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterAuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };

    public static EVoterAuditEntity EVoterAudit5 => new()
    {
        Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsGoldach,
        ContextId = "3",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterAuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = false,
    };

    public static EVoterAuditEntity EVoterAudit6 => new()
    {
        Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsGoldach,
        ContextId = "4",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterAuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };

    public static IEnumerable<EVoterAuditEntity> All
    {
        get
        {
            yield return EVoterAudit1;
            yield return EVoterAudit2;
            yield return EVoterAudit3;
            yield return EVoterAudit4;
            yield return EVoterAudit5;
            yield return EVoterAudit6;
        }
    }

    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped)
    {
        return runScoped(async sp =>
        {
            var db = sp.GetRequiredService<IDataContext>();
            db.EVoterAudits.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
