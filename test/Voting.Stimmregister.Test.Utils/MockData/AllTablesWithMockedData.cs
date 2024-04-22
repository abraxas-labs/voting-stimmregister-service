// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Testing.Mocks;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

public static class AllTablesWithMockedData
{
    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <param name="userAclWithData">The tenant id to seed data for.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped, int userAclWithData)
    {
        return runScoped(async sp =>
        {
            var db = sp.GetRequiredService<IDataContext>();

            db.AccessControlListDois.Add(AccessControlListDoi(userAclWithData));
            db.DomainOfInfluences.Add(DomainOfInfluence(userAclWithData));
            db.Filters.Add(Filter(userAclWithData));
            db.FilterCriteria.Add(FilterCriteria(userAclWithData));
            db.FilterVersions.Add(FilterVersion(userAclWithData));
            db.FilterVersionPersons.Add(FilterVersionPerson(userAclWithData));
            db.ImportStatistics.Add(ImportStatistic(userAclWithData));
            db.BfsIntegrities.Add(Integrities(userAclWithData));
            db.Persons.Add(Person(userAclWithData));

            await db.SaveChangesAsync();
        });
    }

    private static AccessControlListDoiEntity AccessControlListDoi(int bfs)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = "some",
            Bfs = bfs.ToString(),
            TenantName = "some",
            TenantId = "1112",
        };

    private static DomainOfInfluenceEntity DomainOfInfluence(int bfs)
            => new()
            {
                Id = Guid.NewGuid(),
                MunicipalityId = bfs,
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

    private static FilterEntity Filter(int bfs)
        => new()
        {
            Id = Guid.Parse("241BE10B-B4AA-4F36-B67D-E137D183906C"),
            AuditInfo = MockedAuditInfo.Get(),
            MunicipalityId = bfs,
        };

    private static FilterCriteriaEntity FilterCriteria(int bfs)
        => new()
        {
            Id = Guid.NewGuid(),
            AuditInfo = MockedAuditInfo.Get(),
            FilterId = Filter(bfs).Id,
            FilterOperator = FilterOperatorType.Contains,
            FilterType = FilterDataType.String,
            FilterValue = "Wert",
        };

    private static FilterVersionEntity FilterVersion(int bfs)
        => new()
        {
            Id = Guid.Parse("2DDF8718-BE9E-4730-A3AD-81253A16F5F7"),
            AuditInfo = MockedAuditInfo.Get(),
            Name = "My Filter Version",
            FilterId = Filter(bfs).Id,
        };

    private static FilterVersionPersonEntity FilterVersionPerson(int bfs)
        => new()
        {
            Id = Guid.NewGuid(),
            AuditInfo = MockedAuditInfo.Get(),
            FilterVersionId = FilterVersion(bfs).Id,
            PersonId = Guid.Parse("5457877c-b57b-46d8-926c-9d8b6d13705b"),
        };

    private static ImportStatisticEntity ImportStatistic(int bfs)
        => new()
        {
            Id = Guid.NewGuid(),
            AuditInfo = MockedAuditInfo.Get(),
            FileName = "DomainOfInfluenceSuccessMock.csv",
            ImportType = ImportType.DomainOfInfluence,
            SourceSystem = ImportSourceSystem.Loganto,
            ImportStatus = ImportStatus.FinishedSuccessfully,
            ImportRecordsCountTotal = 10,
            DatasetsCountCreated = 7,
            DatasetsCountUpdated = 2,
            DatasetsCountDeleted = 1,
            FinishedDate = MockedClock.GetDate(),
            HasValidationErrors = false,
            TotalElapsedMilliseconds = 100,
            MunicipalityId = bfs,
        };

    private static BfsIntegrityEntity Integrities(int bfs)
        => new()
        {
            Id = Guid.NewGuid(),
            AuditInfo = MockedAuditInfo.Get(),
            Bfs = bfs.ToString(),
            SignatureVersion = 1,
            SignatureKeyId = "SomeSignatureKeyId",
            Signature = new byte[] { 1, 0, 1 },
        };

    private static PersonEntity Person(int bfs)
        => new()
        {
            Id = Guid.Parse("5457877c-b57b-46d8-926c-9d8b6d13705b"),
            RegisterId = Guid.Parse("d13ea9e8-ce71-4d8f-9121-05bba13033a8"),
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = bfs,
            DomainOfInfluenceId = 7354,
            FirstName = "Natalie",
            OfficialName = "Lavigne",
            DateOfBirth = new DateOnly(1999, 12, 30),
            ResidenceAddressStreet = "Achslenstr.",
            ResidenceAddressHouseNumber = "3",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "St. Gallen",
            IsValid = true,
        };
}
