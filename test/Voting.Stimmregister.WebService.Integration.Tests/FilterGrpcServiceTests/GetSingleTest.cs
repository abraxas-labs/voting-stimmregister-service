// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Grpc.Core;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class GetSingleTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public GetSingleTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await PersonMockedData.Seed(RunScoped);
        await FilterVersionMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task GetSingle_Unauthorized()
    {
        await GetSingle_Test(UnauthorizedClient, Guid.Empty, expectThrows: true, expectedStatusCode: StatusCode.Unauthenticated);
    }

    [Fact]
    public async Task GetSingle_SgManager()
    {
        var filter = await GetSingle_Test(SgManagerClient, FilterMockedData.SomeFilter_MunicipalityIdOther2.Id, expectedFilterName: "Filter_3203");
        filter.MatchSnapshot();
    }

    [Fact]
    public async Task GetSingle_SgReader()
    {
        var filter = await GetSingle_Test(SgReaderClient, FilterMockedData.SomeFilter_MunicipalityIdOther2.Id, expectedFilterName: "Filter_3203");
        filter.MatchSnapshot();
    }

    [Fact]
    public async Task GetSingle_SgManager_IdNotExists()
    {
        await GetSingle_Test(
            SgManagerClient,
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            expectThrows: true,
            expectedStatusCode: StatusCode.NotFound);
    }

    [Fact]
    public async Task GetSingle_RoleUnauthorized_ShouldReturnPermissionDenied()
    {
        var rolesArray = new[]
        {
            Roles.ApiImporter,
            Roles.ApiExporter,
            Roles.ManualImporter,
            Roles.ManualExporter,
            Roles.ImportObserver,
        };

        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));
        await GetSingle_Test(
            client,
            Guid.Empty,
            expectThrows: true,
            expectedStatusCode: StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        await service.GetSingleAsync(NewValidGetSingleRequest());
    }

    private static async Task<FilterDefinitionModel?> GetSingle_Test(
        FilterService.FilterServiceClient client,
        Guid filterId,
        bool expectFilter = true,
        bool expectThrows = false,
        StatusCode? expectedStatusCode = null,
        string? expectedFilterName = null)
    {
        var request = NewValidGetSingleRequest(filter => filter.FilterId = filterId.ToString());
        if (expectThrows)
        {
            var exception = await Assert.ThrowsAsync<RpcException>(async () =>
                await client.GetSingleAsync(request));

            if (expectedStatusCode != null)
            {
                Assert.Equal(expectedStatusCode, exception.StatusCode);
            }

            return null;
        }
        else
        {
            var response = await client.GetSingleAsync(request);
            var filter = response.Filter;
            if (expectFilter)
            {
                Assert.NotNull(filter);
            }

            if (expectedFilterName != null)
            {
                Assert.Equal(expectedFilterName, filter.Name);
            }

            return filter;
        }
    }

    private static FilterServiceGetSingleRequest NewValidGetSingleRequest(Action<FilterServiceGetSingleRequest>? customizer = null)
    {
        var request = new FilterServiceGetSingleRequest();
        customizer?.Invoke(request);
        return request;
    }
}
