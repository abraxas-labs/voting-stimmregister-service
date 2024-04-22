// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class DeleteVersionTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public DeleteVersionTest(TestApplicationFactory factory)
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
    public async Task DeleteVersion_SgManager_DeleteExisting()
    {
        await DeleteVersion_Test(SgManagerClient, FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id);
    }

    [Fact]
    public async Task DeleteVersion_SgManager_DeleteExistingWithFilterVersionPersons()
    {
        await DeleteVersion_Test(SgManagerClient, FilterVersionMockedData.SomeFilterVersion_WithFilterVersionPersons.Id, expectFilterVersionPersons: true);
    }

    [Fact]
    public async Task DeleteVersion_SgManager_DeleteNonExisting()
    {
        await DeleteVersion_Test(
            SgManagerClient,
            Guid.Parse("821baaf9-7c83-47b6-985c-7538a4699eaa"),
            expectThrows: true,
            expectedStatusCode: StatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteVersion_SgReader_DeleteExisting_ShouldFail()
    {
        await DeleteVersion_Test(
            SgReaderClient,
            FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id,
            expectThrows: true,
            expectedStatusCode: StatusCode.PermissionDenied);
    }

    [Fact]
    public async Task DeleteVersion_RoleUnauthorized_ShouldReturnPermissionDenied()
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
        await DeleteVersion_Test(
            client,
            Guid.Empty,
            expectThrows: true,
            expectedStatusCode: StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        var request = NewValidDeleteVersionRequest(FilterMockedData.SomeFilter_MunicipalityIdOther2.Id);
        await service.DeleteVersionAsync(request);
    }

    private static FilterServiceDeleteFilterVersionRequest NewValidDeleteVersionRequest(Guid versionId)
    {
        var request = new FilterServiceDeleteFilterVersionRequest
        {
            FilterVersionId = versionId.ToString(),
        };
        return request;
    }

    private async Task DeleteVersion_Test(
        FilterService.FilterServiceClient client,
        Guid versionId,
        bool expectThrows = false,
        bool expectFilterVersionPersons = false,
        StatusCode? expectedStatusCode = null)
    {
        var request = NewValidDeleteVersionRequest(versionId);
        if (expectThrows)
        {
            var exception = await Assert.ThrowsAsync<RpcException>(async () =>
                await client.DeleteVersionAsync(request));
            if (expectedStatusCode != null)
            {
                Assert.Equal(expectedStatusCode, exception.StatusCode);
            }
        }
        else
        {
            var filterVersionBeforeDelete = await GetFilterVersionAsync(versionId);
            if (expectFilterVersionPersons)
            {
                var filterVersionPersonsBeforeDelete = await GetFilterVersionPersonsByVersionId(versionId);
                Assert.NotEmpty(filterVersionPersonsBeforeDelete);
            }

            Assert.NotNull(filterVersionBeforeDelete);
            await client.DeleteVersionAsync(request);
            var filterVersionAfterDelete = await GetFilterVersionAsync(versionId);
            Assert.Null(filterVersionAfterDelete);

            if (expectFilterVersionPersons)
            {
                var filterVersionPersonsAfterDelete = await GetFilterVersionPersonsByVersionId(versionId);
                Assert.Empty(filterVersionPersonsAfterDelete);
            }
        }
    }

    private async Task<FilterVersionEntity?> GetFilterVersionAsync(Guid versionId)
    {
        return await RunOnDb(async db =>
            await db.FilterVersions
                .Include(version => version.FilterVersionPersons)
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(version => version.Id == versionId));
    }

    private async Task<IEnumerable<FilterVersionPersonEntity>> GetFilterVersionPersonsByVersionId(Guid versionId)
    {
        return await RunOnDb(async db =>
            await db.FilterVersionPersons
                .IgnoreQueryFilters()
                .Where(versionPerson => versionPerson.FilterVersionId == versionId)
                .ToArrayAsync());
    }
}
