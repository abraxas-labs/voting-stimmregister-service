// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// <para>Access Control List Doi mock data seeder. Use this class to add some static seeding data.</para>
/// <para>
/// Mock Data Set:
///  1x AccessControlListDoi of the municipality 'St.Gallen' with municipality id 3203.
///  1x AccessControlListDoi of the municipality 'Goldach' with municipality id 3213.
/// </para>
/// </summary>
public static class AccessControlListDoiMockedData
{
    public const int MunicipalityIdStGallen = 3203;

    public const int MunicipalityIdGoldach = 3213;

    public const int MunicipalityIdSwissAbroad = 9170;

    public static AccessControlListDoiEntity AccessControlListDoi_StGallen
        => new()
        {
            Id = Guid.Parse("C0DD069F-AB67-4B85-A3A1-7D7B116FE790"),
            Bfs = MunicipalityIdStGallen.ToString(),
            IsValid = true,
            Name = "St. Gallen",
            TenantId = "B8F28054-11C7-4927-822E-F20F67CDA3C4",
            TenantName = "SomeTenant",
            Type = DomainOfInfluenceType.Ct,
            Canton = Canton.SG,
        };

    public static AccessControlListDoiEntity AccessControlListDoi_Goldach
        => new()
        {
            Id = Guid.Parse("DB8B8A7C-4A5D-455B-B42F-4762DCE01088"),
            Bfs = MunicipalityIdGoldach.ToString(),
            IsValid = true,
            Name = "Goldach",
            TenantId = "2F4F98A2-C48E-4E18-9DFB-68B5404015EE",
            TenantName = "SomeTenant",
            Type = DomainOfInfluenceType.Mu,
            Canton = Canton.SG,
        };

    public static AccessControlListDoiEntity AccessControlListDoi_Auslandschweizer
        => new()
        {
            Id = Guid.Parse("E4448A7C-4A5D-455B-B42F-4762DCE01088"),
            Bfs = MunicipalityIdSwissAbroad.ToString(),
            IsValid = true,
            Name = "Auslandschweizer",
            TenantId = "664F98A2-C48E-4E18-9DFB-68B5404015EE",
            TenantName = "Auslandschweizer Tenant",
            Type = DomainOfInfluenceType.Mu,
        };

    public static IEnumerable<AccessControlListDoiEntity> All
    {
        get
        {
            yield return AccessControlListDoi_StGallen;
            yield return AccessControlListDoi_Goldach;
            yield return AccessControlListDoi_Auslandschweizer;
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
            db.AccessControlListDois.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
