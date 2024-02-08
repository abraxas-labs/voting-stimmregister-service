// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Testing.Mocks;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class CreateVersionTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public CreateVersionTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await PersonMockedData.Seed(RunScoped);
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await FilterMockedData.Seed(RunScoped);
        await FilterCriteriaMockedData.Seed(RunScoped, seedWithFilterMockedData: false);
        await FilterVersionMockedData.Seed(RunScoped, seedWithFilterMockedData: false);
        await BfsIntegrityMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task CreateVersion_SgManager_CreateNew()
    {
        const string name = "CreateVersion SgManager CreateNew";
        var deadline = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2012, 4, 12), DateTimeKind.Utc));
        var versionEntity = await CreateVersion_Test(SgManagerClient, name: name, deadline: deadline);
        FixFilterCriteriaChangingFields(versionEntity!);
        versionEntity!.Signature = Array.Empty<byte>();
        versionEntity.MatchSnapshot(version => version.Id);
    }

    [Fact]
    public async Task CreateVersion_SgManager_ShouldFailValidtionName()
    {
        const string name = nameof(CreateVersion_SgManager_CreateNew);
        var deadline = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2012, 4, 12), DateTimeKind.Utc));
        await CreateVersion_Test(SgManagerClient, name: name, expectThrows: true, expectedStatusCode: StatusCode.InvalidArgument, deadline: deadline);
    }

    [Fact]
    public async Task CreateVersion_SgManager_WithManipulatedPerson_ShouldThrow()
    {
        await ModifyDbEntities<PersonEntity>(p => p.Id == PersonMockedData.Person_3203_StGallen_1.Id, p => p.OfficialName += "-modified");
        await CreateVersion_Test(SgManagerClient, expectThrows: true, expectedStatusCode: StatusCode.Internal);
    }

    [Fact]
    public async Task CreateVersion_SgManager_WithAdditionalPerson_ShouldThrow()
    {
        await RunOnDb(async db =>
        {
            db.Persons.Add(new PersonEntity
            {
                Id = Guid.Parse("4f7bc960-ebcd-4b4d-a624-03b0f27e45e2"),
                RegisterId = Guid.Parse("7822d87b-d999-48d1-a8bb-710b875e89bd"),
                CreatedDate = MockedClock.GetDate(),
                MunicipalityId = PersonMockedData.MunicipalityIdStGallen,
                DomainOfInfluenceId = 7354,
                FirstName = "Natalie2",
                OfficialName = "Lavigne2",
                DateOfBirth = new DateOnly(1999, 12, 30),
                ResidenceAddressStreet = "Achslenstr.",
                ResidenceAddressHouseNumber = "3",
                ResidenceAddressZipCode = "9016",
                ResidenceAddressTown = "St. Gallen",
                Country = "CH",
                CountryNameShort = "Schweiz",
                RestrictedVotingAndElectionRightFederation = false,
                IsValid = true,
                IsLatest = true,
                DeletedDate = null,
                ResidenceCantonAbbreviation = "SG",
                SourceSystemName = ImportSourceSystem.Loganto,
            });
            await db.SaveChangesAsync();
        });
        await CreateVersion_Test(SgManagerClient, expectThrows: true, expectedStatusCode: StatusCode.Internal);
    }

    [Fact]
    public async Task CreateVersion_SgManager_WithRemovedPerson_ShouldThrow()
    {
        await RunOnDb(async db =>
        {
            var person = await db.Persons.IgnoreQueryFilters().FirstAsync(x => x.Id == PersonMockedData.Person_3203_StGallen_1.Id);
            db.Persons.Remove(person);
            await db.SaveChangesAsync();
        });
        await CreateVersion_Test(SgManagerClient, expectThrows: true, expectedStatusCode: StatusCode.Internal);
    }

    [Fact]
    public async Task CreateVersion_SgManager_ShouldFailIfFilterIdNotInAcl()
    {
        var request = NewValidCreateVersionRequest(
            name: "CreateVersion SgManager CreateNew",
            deadline: Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2012, 4, 12), DateTimeKind.Utc)));

        request.FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther.Id.ToString();

        var exception = await Assert.ThrowsAsync<RpcException>(async () =>
            await SgManagerClient.CreateVersionAsync(request));

        exception.StatusCode.Should().Be(StatusCode.NotFound);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        await service.CreateVersionAsync(NewValidCreateVersionRequest());
    }

    private static FilterServiceCreateFilterVersionRequest NewValidCreateVersionRequest(
        string name = "name",
        Timestamp? deadline = null)
    {
        return new FilterServiceCreateFilterVersionRequest
        {
            Name = name,
            Deadline = deadline ?? CreateTimestamp(2020, 1, 1),
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id.ToString(),
        };
    }

    private static Timestamp CreateTimestamp(int year, int month, int day)
    {
        return Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc));
    }

    private static void FixFilterCriteriaChangingFields(FilterVersionEntity filterVersionEntity)
    {
        foreach (var criteria in filterVersionEntity.FilterCriterias)
        {
            criteria.Id = default;
            criteria.FilterId = default;
            criteria.FilterVersionId = default;
        }
    }

    private async Task<FilterVersionEntity?> CreateVersion_Test(
        FilterService.FilterServiceClient client,
        string name = "name",
        bool expectThrows = false,
        StatusCode? expectedStatusCode = null,
        Timestamp? deadline = null)
    {
        var request = NewValidCreateVersionRequest(name: name, deadline: deadline);
        if (expectThrows)
        {
            var exception = await Assert.ThrowsAsync<RpcException>(async () =>
                await client.CreateVersionAsync(request));
            if (expectedStatusCode != null)
            {
                Assert.Equal(expectedStatusCode, exception.StatusCode);
            }

            return null;
        }
        else
        {
            var oldFilterVersionCount = await CountAsync<FilterVersionEntity>();
            var oldFilterCriteriaCount = await CountAsync<FilterCriteriaEntity>();
            await client.CreateVersionAsync(request);
            var newFilterVersionCount = await CountAsync<FilterVersionEntity>();
            var newFilterCriteriaCount = await CountAsync<FilterCriteriaEntity>();
            Assert.Equal(newFilterVersionCount, oldFilterVersionCount + 1);
            Assert.True(newFilterCriteriaCount > oldFilterCriteriaCount);
            return await GetFilterVersionByNameAsync(name);
        }
    }

    private async Task<int> CountAsync<TEntity>()
        where TEntity : class
    {
        return await RunOnDb(async db =>
            await db.Set<TEntity>().IgnoreQueryFilters().CountAsync());
    }

    private async Task<FilterVersionEntity?> GetFilterVersionByNameAsync(string versionName)
    {
        return await RunOnDb(async db =>
            await db.FilterVersions.IgnoreQueryFilters()
                .Include(version => version.FilterCriterias)
                .SingleOrDefaultAsync(version => version.Name == versionName));
    }
}
