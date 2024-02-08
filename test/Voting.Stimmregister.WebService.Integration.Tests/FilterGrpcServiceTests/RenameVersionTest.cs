// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class RenameVersionTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public RenameVersionTest(TestApplicationFactory factory)
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
    }

    [Fact]
    public async Task RenameVersion_SgManager_ExistingVersion()
    {
        const string name = "RenameVersion SgManager ExistingVersion";
        var versionEntity = await RenameVersion_Test(SgManagerClient, filterVersionId: FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id, name: name);
        FixFilterCriteriaChangingFields(versionEntity!);
        versionEntity!.Signature = Array.Empty<byte>();
        versionEntity.MatchSnapshot(version => version.Id);
    }

    [Fact]
    public async Task RenameVersion_SgManager_NotExistingVersion()
    {
        const string name = "RenameVersion SgManager NotExistingVersion";
        _ = await RenameVersion_Test(SgManagerClient, filterVersionId: Guid.Empty, name: name, expectThrows: true, expectedStatusCode: StatusCode.NotFound);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        await service.RenameVersionAsync(NewValidRenameVersionRequest(Guid.Empty));
    }

    private static FilterServiceRenameFilterVersionRequest NewValidRenameVersionRequest(
        Guid filterVersionId,
        string name = "name")
    {
        return new FilterServiceRenameFilterVersionRequest
        {
            Name = name,
            FilterVersionId = filterVersionId.ToString(),
        };
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

    private async Task<FilterVersionEntity?> RenameVersion_Test(
        FilterService.FilterServiceClient client,
        Guid filterVersionId,
        string name = "name",
        bool expectThrows = false,
        StatusCode? expectedStatusCode = null)
    {
        var request = NewValidRenameVersionRequest(filterVersionId: filterVersionId, name: name);
        if (expectThrows)
        {
            var exception = await Assert.ThrowsAsync<RpcException>(async () =>
                await client.RenameVersionAsync(request));
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
            await client.RenameVersionAsync(request);
            var newFilterVersionCount = await CountAsync<FilterVersionEntity>();
            var newFilterCriteriaCount = await CountAsync<FilterCriteriaEntity>();
            Assert.Equal(oldFilterVersionCount, newFilterVersionCount);
            Assert.Equal(oldFilterCriteriaCount, newFilterCriteriaCount);
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
