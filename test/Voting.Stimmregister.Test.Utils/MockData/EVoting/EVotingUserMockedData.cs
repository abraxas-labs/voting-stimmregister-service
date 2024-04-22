// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData.EVoting;

public static class EVotingUserMockedData
{
    public static EVoterEntity EVoter1 => new()
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000000"),
        Ahvn13 = 756_0000_0000_00,
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsStGallen,
        ContextId = "1",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = false,
    };

    public static EVoterEntity EVoter2 => new()
    {
        Id = Guid.Parse("20000000-0000-0000-0000-000000000000"),
        Ahvn13 = 756_1302_6191_07,
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsStGallen,
        ContextId = "2",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };

    public static EVoterEntity EVoter3 => new()
    {
        Id = Guid.Parse("30000000-0000-0000-0000-000000000000"),
        Ahvn13 = 756_1110_0000_01,
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsGoldach,
        ContextId = "3",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = false,
    };

    public static EVoterEntity EVoter4 => new()
    {
        Id = Guid.Parse("40000000-0000-0000-0000-000000000000"),
        Ahvn13 = 756_2220_0000_01,
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsGoldach,
        ContextId = "4",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };

    public static IEnumerable<EVoterEntity> All
    {
        get
        {
            yield return EVoter1;
            yield return EVoter2;
            yield return EVoter3;
            yield return EVoter4;
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
            db.EVoters.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
