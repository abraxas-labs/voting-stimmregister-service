// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// BfsStatistic mock data seeder. Use this class to add some static seeding data.
/// </summary>
public static class BfsStatisticMockedData
{
    public const string MunicipalityIdStGallen = "3203";
    public const string MunicipalityNameStGallen = "St. Gallen";

    public const string MunicipalityIdGoldach = "3213";
    public const string MunicipalityNameGoldach = "Goldach";

    public const string MunicipalityIdFrauenfeld = "4566";
    public const string MunicipalityNameFrauenfeld = "Frauenfeld";

    public static BfsStatisticEntity BfsStatistic_3203_StGallen
        => new()
        {
            Id = Guid.Parse("78b398bf-3c30-40b7-b014-a89a01a1408e"),
            AuditInfo = MockedAuditInfo.Get(),
            Bfs = MunicipalityIdStGallen,
            BfsName = MunicipalityNameStGallen,
            VoterTotalCount = 90000,
            EVoterTotalCount = 18000,
            EVoterRegistrationCount = 20000,
            EVoterDeregistrationCount = 200,
        };

    public static BfsStatisticEntity BfsStatistic_3213_Goldach
        => new()
        {
            Id = Guid.Parse("c101bf6b-1b77-4131-a78c-8a2efbddbc34"),
            AuditInfo = MockedAuditInfo.Get(),
            Bfs = MunicipalityIdGoldach,
            BfsName = MunicipalityNameGoldach,
            VoterTotalCount = 9000,
            EVoterTotalCount = 0,
            EVoterRegistrationCount = 0,
            EVoterDeregistrationCount = 0,
        };

    public static BfsStatisticEntity BfsStatistic_4566_Frauenfeld
        => new()
        {
            Id = Guid.Parse("64ce8b83-b19f-4b60-8546-1108edd4ac78"),
            AuditInfo = MockedAuditInfo.Get(),
            Bfs = MunicipalityIdFrauenfeld,
            BfsName = MunicipalityNameFrauenfeld,
            VoterTotalCount = 20000,
            EVoterTotalCount = 0,
            EVoterRegistrationCount = 0,
            EVoterDeregistrationCount = 0,
        };

    public static IEnumerable<BfsStatisticEntity> All
    {
        get
        {
            yield return BfsStatistic_3203_StGallen;
            yield return BfsStatistic_3213_Goldach;
            yield return BfsStatistic_4566_Frauenfeld;
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
            db.BfsStatistics.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
