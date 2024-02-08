// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class GetAllTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public GetAllTest(TestApplicationFactory factory)
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
    public async Task GetAll_Unauthorized()
    {
        await GetAll_Test(UnauthorizedClient, 0, expectThrows: true, expectedStatusCode: StatusCode.Unauthenticated);
    }

    [Fact]
    public async Task GetAll_SgManager()
    {
        var filters = await GetAll_Test(SgManagerClient, 3, "Filter_9170");
        filters.MatchSnapshot();
    }

    [Fact]
    public async Task GetAll_ExporterImporter()
    {
        await GetAll_Test(ApiImporterClient, 0, expectThrows: true, expectedStatusCode: StatusCode.PermissionDenied);
        await GetAll_Test(ManualImporterClient, 0, expectThrows: true, expectedStatusCode: StatusCode.PermissionDenied);
        await GetAll_Test(ApiExporterClient, 0, expectThrows: true, expectedStatusCode: StatusCode.PermissionDenied);
        await GetAll_Test(ManualExporterClient, 0, expectThrows: true, expectedStatusCode: StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        await service.GetAllAsync(NewValidGetAllRequest());
    }

    private static async Task<RepeatedField<FilterDefinitionModel>?> GetAll_Test(
        FilterService.FilterServiceClient client,
        int? expectedFiltersCount = null,
        string? expectedFirstFilterName = null,
        bool expectThrows = false,
        StatusCode? expectedStatusCode = null)
    {
        var request = NewValidGetAllRequest();
        if (expectThrows)
        {
            var exception = await Assert.ThrowsAsync<RpcException>(async () =>
                await client.GetAllAsync(request));
            if (expectedStatusCode != null)
            {
                Assert.Equal(expectedStatusCode, exception.StatusCode);
            }

            return null;
        }
        else
        {
            var response = await client.GetAllAsync(request);
            if (expectedFiltersCount != null)
            {
                Assert.Equal(expectedFiltersCount, response.Filters.Count);
            }

            if (response.Filters.Count > 0 && expectedFirstFilterName != null)
            {
                Assert.Equal(expectedFirstFilterName, response.Filters?.First().Name);
            }

            return response.Filters;
        }
    }

    private static FilterServiceGetAllRequest NewValidGetAllRequest()
    {
        var request = new FilterServiceGetAllRequest();
        return request;
    }
}
