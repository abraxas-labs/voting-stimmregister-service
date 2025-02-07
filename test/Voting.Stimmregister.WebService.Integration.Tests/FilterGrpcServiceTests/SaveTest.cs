// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class SaveTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public SaveTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await PersonMockedData.Seed(RunScoped);
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await FilterMockedData.Seed(RunScoped);
        await FilterVersionMockedData.Seed(RunScoped, seedWithFilterMockedData: false);
        await FilterCriteriaMockedData.Seed(RunScoped, seedWithFilterMockedData: false);
    }

    [Fact]
    public async Task Save_SgManager_CreateNew_WithoutCriteria()
    {
        const string name = "Save SgManager CreateNew WithoutCriteria";
        var filterEntity = await Save_Test(SgManagerClient, name: name, addCriteria: false);
        filterEntity.Should().NotBeNull();
        filterEntity.MatchSnapshot(filter => filter!.Id);
    }

    [Fact]
    public async Task Save_SgManager_CreateNew_WithCriteria()
    {
        const string name = "Save SgManager CreateNew WithCriteria";
        var filterEntity = await Save_Test(SgManagerClient, name: name, addCriteria: true);
        filterEntity.Should().NotBeNull();
        FixFilterCriteriaChangingFields(filterEntity!);
        filterEntity.MatchSnapshot(filter => filter!.Id);
    }

    [Fact]
    public async Task Save_SgManager_CreateNew_WithDuplicateCriteria_ShouldFailDueToConstraint()
    {
        var request = NewValidSaveRequest(
            name: "Save SgManager CreateNew WithDuplicateCriteria ShouldFailDueToConstraint",
            addCriteria: true);

        request.Criteria.Add(request.Criteria[0]);

        var exception = await Assert.ThrowsAsync<RpcException>(async () =>
            await SgManagerClient.SaveAsync(request));

        exception.StatusCode.Should().Be(StatusCode.Internal);
    }

    [Fact]
    public async Task Save_SgManager_UpdateExistingWithCriteria_OverwritesCriteria()
    {
        const string name = "Save SgManager UpdateExistingWithCriteria OverwritesCriteria";
        var filterEntity = await Save_Test(SgManagerClient, name: name, filterId: FilterMockedData.SomeFilter_MunicipalityIdOther2.Id, addCriteria: true);
        filterEntity.Should().NotBeNull();
        FixFilterCriteriaChangingFields(filterEntity!);
        filterEntity.MatchSnapshot(filter => filter!.Id);
    }

    [Fact]
    public async Task Save_SgManager_UpdateExisting_WithoutCriteria()
    {
        const string name = "Save SgManager UpdateExisting WithoutCriteria";
        var filterEntity = await Save_Test(SgManagerClient, filterId: FilterMockedData.SomeFilter_MunicipalityIdOther2.Id, name: name, addCriteria: false);
        filterEntity.Should().NotBeNull();
        filterEntity.MatchSnapshot();
    }

    [Fact]
    public async Task Save_SgManager_UpdateExisting_WithCriteria()
    {
        const string name = "Save SgManager UpdateExisting WithCriteria";
        var filterEntity = await Save_Test(SgManagerClient, filterId: FilterMockedData.SomeFilter_MunicipalityIdOther2.Id, name: name, addCriteria: true);
        filterEntity.Should().NotBeNull();
        FixFilterCriteriaChangingFields(filterEntity!);
        filterEntity.MatchSnapshot(filter => filter!.Id);
    }

    [Fact]
    public async Task Save_RoleUnauthorized_UpdateExisting_ShouldPermissionDenied()
    {
        var rolesArray = new[]
        {
            Roles.ApiImporter,
            Roles.ApiExporter,
            Roles.ManualImporter,
            Roles.ManualExporter,
            Roles.ImportObserver,
            Roles.Reader,
        };

        const string name = "Save_RoleUnauthorized_ShouldPermissionDenied";
        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));
        await Save_Test(
            client,
            expectThrows: true,
            expectedStatusCode: StatusCode.PermissionDenied,
            filterId: Guid.Empty,
            name: name);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        await service.SaveAsync(NewValidSaveRequest());
    }

    private static void FixFilterCriteriaChangingFields(FilterEntity filterEntity)
    {
        foreach (var criteria in filterEntity.FilterCriterias)
        {
            criteria.Id = default;
            criteria.FilterId = default;
        }
    }

    private static FilterServiceSaveFilterRequest NewValidSaveRequest(
        Guid? filterId = null,
        bool addCriteria = false,
        string name = "name",
        string description = "description")
    {
        var request = new FilterServiceSaveFilterRequest
        {
            FilterId = filterId == null ? Guid.Empty.ToString() : filterId.ToString(),
            Name = name,
            Description = description,
        };
        if (addCriteria)
        {
            request.Criteria.Add(new FilterCriteriaModel
            {
                Id = Guid.Empty.ToString(),
                FilterDataType = FilterDataType.String,
                FilterOperator = FilterOperator.Contains,
                FilterValue = "Kurt",
                ReferenceId = FilterReference.FirstName,
            });
        }

        return request;
    }

    private async Task<FilterEntity?> Save_Test(
        FilterService.FilterServiceClient client,
        bool expectThrows = false,
        StatusCode? expectedStatusCode = null,
        Guid? filterId = null,
        string name = "name",
        string description = "description",
        bool addCriteria = false)
    {
        var request = NewValidSaveRequest(filterId: filterId, addCriteria: addCriteria, name: name, description: description);
        if (expectThrows)
        {
            var exception = await Assert.ThrowsAsync<RpcException>(async () =>
                await client.SaveAsync(request));
            if (expectedStatusCode != null)
            {
                Assert.Equal(expectedStatusCode, exception.StatusCode);
            }

            return null;
        }
        else
        {
            var oldFilterCount = await CountAsync<FilterEntity>();
            await client.SaveAsync(request);
            var newFilterCount = await CountAsync<FilterEntity>();
            var filterEntity = await GetFilterByNameAsync(name);
            if (!request.HasFilterId)
            {
                Assert.Equal(newFilterCount, oldFilterCount + 1);
            }

            return filterEntity;
        }
    }

    private async Task<int> CountAsync<TEntity>()
        where TEntity : class
    {
        return await RunOnDb(async db =>
            await db.Set<TEntity>().IgnoreQueryFilters().CountAsync());
    }

    private async Task<FilterEntity?> GetFilterByNameAsync(string filterName)
    {
        return await RunOnDb(async db =>
            await db.Filters.IgnoreQueryFilters()
                .Include(filter => filter.FilterCriterias)
                .SingleOrDefaultAsync(filter => filter.Name == filterName));
    }
}
