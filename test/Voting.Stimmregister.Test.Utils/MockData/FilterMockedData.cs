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
/// ImportAcl mock data seeder. Use this class to add some static seeding data.
/// </summary>
public static class FilterMockedData
{
    public const int MunicipalityId = 1111;
    public const int MunicipalityIdOther = 2222;

    public const int MunicipalityId3203 = 3203;
    public const int MunicipalityId9170 = 9170;

    public static FilterEntity SomeFilter_MunicipalityId
        => new()
        {
            Id = Guid.Parse("241BE10B-B4AA-4F36-B67D-E137D183906C"),
            AuditInfo = MockedAuditInfo.Get(),
            Name = "My Filter",
            MunicipalityId = MunicipalityId,
        };

    public static FilterEntity SomeFilter_MunicipalityIdOther
        => new()
        {
            Id = Guid.Parse("C8C056C5-620E-44E8-A9C1-D9F42535E085"),
            AuditInfo = MockedAuditInfo.Get(),
            Name = "Other Filter",
            MunicipalityId = MunicipalityIdOther,
        };

    public static FilterEntity SomeFilter_MunicipalityIdOther2
        => new()
        {
            Id = Guid.Parse("C8C056C5-620E-44E8-A9C1-D9F42535E077"),
            AuditInfo = MockedAuditInfo.Get(),
            Name = "Filter_3203",
            MunicipalityId = MunicipalityId3203,
        };

    public static FilterEntity SomeFilter_MunicipalityIdOther3
        => new()
        {
            Id = Guid.Parse("a287456a-9007-418f-9eea-72f943119396"),
            AuditInfo = MockedAuditInfo.Get(),
            Name = "Filter_3203",
            MunicipalityId = MunicipalityId3203,
        };

    public static FilterEntity SomeFilter_MunicipalityId9170_SwissAbroad
        => new()
        {
            Id = Guid.Parse("3c423af7-bb20-4863-bff7-c782e96b9d71"),
            AuditInfo = MockedAuditInfo.Get(),
            Name = "Filter_9170",
            MunicipalityId = MunicipalityId9170,
        };

    public static IEnumerable<FilterEntity> All
    {
        get
        {
            yield return SomeFilter_MunicipalityId;
            yield return SomeFilter_MunicipalityIdOther;
            yield return SomeFilter_MunicipalityIdOther2;
            yield return SomeFilter_MunicipalityIdOther3;
            yield return SomeFilter_MunicipalityId9170_SwissAbroad;
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
            db.Filters.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
