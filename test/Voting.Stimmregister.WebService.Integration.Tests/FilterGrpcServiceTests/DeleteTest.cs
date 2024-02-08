// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class DeleteTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public DeleteTest(TestApplicationFactory factory)
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
    public async Task Delete_SgManager_DeleteExisting()
    {
        await Delete_Test(SgManagerClient, FilterMockedData.SomeFilter_MunicipalityIdOther2.Id);
    }

    [Fact]
    public async Task Delete_SgManager_DeleteFilterOfOtherMunicipality()
    {
        await Delete_Test(
            SgManagerClient,
            FilterMockedData.SomeFilter_MunicipalityId.Id,
            expectThrows: true,
            expectedStatusCode: StatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_SgManager_DeleteNonExisting()
    {
        await Delete_Test(
            SgManagerClient,
            Guid.Parse("821baaf9-7c83-47b6-985c-7538a4699e0d"),
            expectThrows: true,
            expectedStatusCode: StatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_SgReader_DeleteExisting_ShouldFail()
    {
        await Delete_Test(
            SgReaderClient,
            FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
            expectThrows: true,
            expectedStatusCode: StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        var request = NewValidDeleteRequest(FilterMockedData.SomeFilter_MunicipalityIdOther2.Id);
        await service.DeleteAsync(request);
    }

    private static FilterServiceDeleteFilterRequest NewValidDeleteRequest(Guid filterId)
    {
        var request = new FilterServiceDeleteFilterRequest
        {
            FilterId = filterId.ToString(),
        };
        return request;
    }

    private async Task Delete_Test(
        FilterService.FilterServiceClient client,
        Guid filterId,
        bool expectThrows = false,
        StatusCode? expectedStatusCode = null)
    {
        var request = NewValidDeleteRequest(filterId);
        if (expectThrows)
        {
            var exception = await Assert.ThrowsAsync<RpcException>(async () =>
                await client.DeleteAsync(request));
            if (expectedStatusCode != null)
            {
                Assert.Equal(expectedStatusCode, exception.StatusCode);
            }
        }
        else
        {
            var oldCount = await CountAsync<FilterEntity>();
            await client.DeleteAsync(request);
            var newCount = await CountAsync<FilterEntity>();
            Assert.Equal(oldCount - 1, newCount);
        }
    }

    private async Task<int> CountAsync<TEntity>()
        where TEntity : class
    {
        return await RunOnDb(async db =>
            await db.Set<TEntity>().IgnoreQueryFilters().CountAsync());
    }
}
