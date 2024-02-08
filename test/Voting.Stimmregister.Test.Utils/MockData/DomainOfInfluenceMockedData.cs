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
/// <para>Domain of influence mock data seeder. Use this class to add some static seeding data.</para>
/// <para>
/// Mock Data Set:
///  > 2 domain of influences of the municipality 'St.Gallen' with municipality id 3203.
///  > 2 domain of influences of the municipality 'Goldach' with municipality id 3213.
///  > 2 domain of influences of the municipality 'Jona' with municipality id 3340.
/// </para>
/// </summary>
public static class DomainOfInfluenceMockedData
{
    public static int MunicipalityIdStGallen => 3203;

    public static int MunicipalityIdGoldach => 3213;

    public static int MunicipalityIdJona => 3340;

    public static DomainOfInfluenceEntity DomainOfInfluence_3203_StGallen_1
        => new()
        {
            Id = Guid.NewGuid(),
            MunicipalityId = MunicipalityIdStGallen,
            DomainOfInfluenceId = 7354,
            Street = "Achslenstr.",
            HouseNumber = "3",
            HouseNumberAddition = "a",
            SwissZipCode = 9016,
            Town = "St. Gallen",
            IsValid = true,
            PoliticalCircleId = "O",
            PoliticalCircleName = "Osten",
        };

    public static DomainOfInfluenceEntity DomainOfInfluence_3203_StGallen_2
        => new()
        {
            Id = Guid.NewGuid(),
            MunicipalityId = MunicipalityIdStGallen,
            DomainOfInfluenceId = 330789,
            Street = "Sturzeneggstr.",
            HouseNumber = "31",
            HouseNumberAddition = "b",
            SwissZipCode = 9015,
            Town = "St. Gallen",
            IsValid = true,
            PoliticalCircleId = "W",
            PoliticalCircleName = "Westen",
        };

    public static DomainOfInfluenceEntity DomainOfInfluence_3213_Goldach_1
        => new()
        {
            Id = Guid.NewGuid(),
            MunicipalityId = MunicipalityIdGoldach,
            DomainOfInfluenceId = 858,
            Street = "Libellenstr.",
            HouseNumber = "6",
            HouseNumberAddition = null,
            SwissZipCode = 9403,
            Town = "Goldach",
            IsValid = true,
            PoliticalCircleId = "C",
            PoliticalCircleName = "Centrum",
        };

    public static DomainOfInfluenceEntity DomainOfInfluence_3340_Jona_1
        => new()
        {
            Id = Guid.NewGuid(),
            MunicipalityId = MunicipalityIdJona,
            DomainOfInfluenceId = 103671,
            Street = "Stampfstr.",
            HouseNumber = "23",
            HouseNumberAddition = "a",
            SwissZipCode = 8645,
            Town = "Goldach",
            IsValid = true,
            PoliticalCircleId = "M",
            PoliticalCircleName = "Mitte",
        };

    public static DomainOfInfluenceEntity DomainOfInfluence_3340_Jona_2
        => new()
        {
            Id = Guid.NewGuid(),
            MunicipalityId = MunicipalityIdJona,
            DomainOfInfluenceId = 387108,
            Street = "Meienbergstr.",
            HouseNumber = "9",
            HouseNumberAddition = "f",
            SwissZipCode = 8645,
            Town = "Goldach",
            IsValid = true,
            PoliticalCircleId = "N",
            PoliticalCircleName = "Nord",
        };

    public static DomainOfInfluenceEntity DomainOfInfluence_3213_Goldach_2
        => new()
        {
            Id = Guid.NewGuid(),
            MunicipalityId = MunicipalityIdGoldach,
            DomainOfInfluenceId = 375259,
            Street = "Hohrainweg",
            HouseNumber = "5",
            HouseNumberAddition = "a",
            SwissZipCode = 9403,
            Town = "Goldach",
            IsValid = true,
            PoliticalCircleId = "C",
            PoliticalCircleName = "Centrum",
        };

    public static IEnumerable<DomainOfInfluenceEntity> All
    {
        get
        {
            yield return DomainOfInfluence_3203_StGallen_1;
            yield return DomainOfInfluence_3203_StGallen_2;
            yield return DomainOfInfluence_3213_Goldach_1;
            yield return DomainOfInfluence_3213_Goldach_2;
            yield return DomainOfInfluence_3340_Jona_1;
            yield return DomainOfInfluence_3340_Jona_2;
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
            db.DomainOfInfluences.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
