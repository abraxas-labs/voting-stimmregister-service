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
/// ImportAcl mock data seeder. Use this class to add some static seeding data.
/// </summary>
public static class FilterVersionPersonMockedData
{
    public static int MunicipalityId => FilterVersionMockedData.MunicipalityId;

    public static int MunicipalityIdOther => FilterVersionMockedData.MunicipalityIdOther;

    public static FilterVersionPersonEntity SomeFilterVersionPerson_MunicipalityId
        => new()
        {
            Id = Guid.Parse("BA4425B4-0969-4F16-909F-76781DC0DC7A"),
            PersonId = Guid.Parse("12000000-0000-0000-0000-000000000000"),
            AuditInfo = MockedAuditInfo.Get(),
            FilterVersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityId.Id,
        };

    public static FilterVersionPersonEntity SomeFilterVersionPerson_MunicipalityIdOther
        => new()
        {
            Id = Guid.Parse("9F2405A5-CA25-4B64-863B-54B05ADEE09E"),
            PersonId = Guid.Parse("12000000-0000-0000-0000-000000000000"),
            AuditInfo = MockedAuditInfo.Get(),
            FilterVersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther.Id,
        };

    public static FilterVersionPersonEntity SomeFilterVersionPerson1_3203_SG
        => new()
        {
            Id = Guid.Parse("ffcf443b-ce6b-4385-9dfd-bfce86443c9f"),
            PersonId = PersonMockedData.Person_3203_StGallen_1.Id,
            AuditInfo = MockedAuditInfo.Get(),
            FilterVersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id,
        };

    public static FilterVersionPersonEntity SomeFilterVersionPerson3_3203_SG
        => new()
        {
            Id = Guid.Parse("163ec470-797e-4304-8bcf-c9aed55f41ef"),
            PersonId = PersonMockedData.Person_3203_StGallen_3_Foreigner.Id,
            AuditInfo = MockedAuditInfo.Get(),
            FilterVersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id,
        };

    public static FilterVersionPersonEntity SomeFilterVersionAuslandschweizer1_9170_SG
        => new()
        {
            Id = Guid.Parse("15AD8926-D700-4B18-A402-BD24562A8ED3"),
            PersonId = PersonMockedData.Person_9170_Auslandschweizer_1.Id,
            AuditInfo = MockedAuditInfo.Get(),
            FilterVersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id,
        };

    public static FilterVersionPersonEntity SomeFilterVersionPerson_WithFilterVersionPersons
        => new()
        {
            Id = Guid.Parse("333ec470-797e-4304-8bcf-c9aed55f41ef"),
            PersonId = PersonMockedData.Person_3203_StGallen_3_Foreigner.Id,
            AuditInfo = MockedAuditInfo.Get(),
            FilterVersionId = FilterVersionMockedData.SomeFilterVersion_WithFilterVersionPersons.Id,
        };

    public static IEnumerable<FilterVersionPersonEntity> All
    {
        get
        {
            yield return SomeFilterVersionPerson_MunicipalityId;
            yield return SomeFilterVersionPerson_MunicipalityIdOther;
            yield return SomeFilterVersionPerson1_3203_SG;
            yield return SomeFilterVersionPerson3_3203_SG;
            yield return SomeFilterVersionAuslandschweizer1_9170_SG;
            yield return SomeFilterVersionPerson_WithFilterVersionPersons;
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
            db.Persons.Add(new PersonEntity
            {
                Id = Guid.Parse("12000000-0000-0000-0000-000000000000"),
                MunicipalityId = 3214,
                CountryNameShort = "Schweiz",
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                FirstName = "Max",
                OfficialName = "Muster",
                Sex = SexType.Male,
            });
            db.FilterVersionPersons.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
