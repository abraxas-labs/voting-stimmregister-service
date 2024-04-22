// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class DuplicateTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public DuplicateTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await FilterMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task Duplicate_SgManager_DuplicateExisting()
    {
        var response = await SgManagerClient.DuplicateAsync(NewValidDuplicateRequest(FilterMockedData.SomeFilter_MunicipalityIdOther2.Id));

        var filter = await RunOnDb(db => db.Filters.IgnoreQueryFilters().FirstAsync(x => Guid.Parse(response.Id) == x.Id));
        filter.Id.Should().NotBe(FilterMockedData.SomeFilter_MunicipalityIdOther2.Id);
        filter.Name.Should().Be("Filter_3203 (Kopie)");
    }

    [Fact]
    public Task Duplicate_SgManager_DuplicateNotExisting()
    {
        return AssertStatus(
            async () => await SgManagerClient.DuplicateAsync(new FilterServiceDuplicateFilterRequest
            {
                FilterId = "c6ac9746-9cdb-497f-9a12-8d4caaa47be4",
            }),
            StatusCode.NotFound);
    }

    [Fact]
    public Task Duplicate_SgManager_DuplicateOfOtherMunicipality()
    {
        return AssertStatus(
            async () => await SgManagerClient.DuplicateAsync(new FilterServiceDuplicateFilterRequest
            {
                FilterId = FilterMockedData.SomeFilter_MunicipalityId.Id.ToString(),
            }),
            StatusCode.NotFound);
    }

    [Fact]
    public Task Duplicate_RoleUnauthorized_DuplicateExisting_ShouldPermissionDenied()
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

        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));
        return AssertStatus(
            async () => await client.DuplicateAsync(new FilterServiceDuplicateFilterRequest
            {
                FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id.ToString(),
            }),
            StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        await service.DuplicateAsync(NewValidDuplicateRequest(FilterMockedData.SomeFilter_MunicipalityIdOther2.Id));
    }

    private static FilterServiceDuplicateFilterRequest NewValidDuplicateRequest(Guid filterId)
    {
        return new FilterServiceDuplicateFilterRequest
        {
            FilterId = filterId.ToString(),
        };
    }
}
