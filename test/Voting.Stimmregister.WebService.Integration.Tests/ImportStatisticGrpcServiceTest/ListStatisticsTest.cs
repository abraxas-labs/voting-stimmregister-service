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

namespace Voting.Stimmregister.WebService.Integration.Tests.ImportStatisticGrpcServiceTest;

public class ListStatisticsTest : BaseWriteableDbGrpcTest<ImportStatisticService.ImportStatisticServiceClient>
{
    public ListStatisticsTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await ImportMockedData.Seed(RunScoped);
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldListStatisticsWhenAuthorizedByAcl()
    {
        var request = NewValidRequest();
        var response = await SgManagerClient.ListAsync(request);

        Assert.Single(response.ImportStatistics);
        Assert.All(response.ImportStatistics, i => Assert.Equal(3203, i.MunicipalityId));
        response.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldIgnoreInactiveStatisticsWhenAuthorizedByAcl()
    {
        var request = NewValidRequest();
        request.ImportSourceSystem = ImportSourceSystem.Loganto;
        var response = await SgManagerClient.ListAsync(request);

        Assert.Empty(response.ImportStatistics);
    }

    [Fact]
    public async Task ShouldListAllStatisticsWhenUserHasRoleImportObserverWithoutAuthroizedAcl()
    {
        var request = NewValidRequest();
        var response = await ImportObserverClient.ListAsync(request);

        Assert.True(response.ImportStatistics.Count > 0);
        response.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfStatisticsWhenNotAuthorizedByAcl()
    {
        var request = NewValidRequest();
        var response = await UnknownClient.ListAsync(request);

        Assert.Empty(response.ImportStatistics);
    }

    [Fact]
    public async Task WhenRoleUnauthorizedShouldReturnPermissionDenied()
    {
        var rolesArray = new[]
        {
            Roles.ApiImporter,
            Roles.ApiExporter,
            Roles.ManualImporter,
            Roles.ManualExporter,
            Roles.ImportObserver,
        };

        var request = NewValidRequest();
        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));
        await AssertStatus(
            async () => await client.ListAsync(request), StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(ImportStatisticService.ImportStatisticServiceClient service)
    {
        await service.ListAsync(NewValidRequest());
    }

    private static ListImportStatisticsRequest NewValidRequest(Action<ListImportStatisticsRequest>? customizer = null)
    {
        var request = new ListImportStatisticsRequest
        {
            ImportType = ImportType.DomainOfInfluence,
            ImportSourceSystem = ImportSourceSystem.Innosolv,
            ImportStatusSimple = ImportStatusSimple.All,
            ImportSource = ImportSource.All,
            Paging = new PagingModel
            {
                PageIndex = 0,
                PageSize = 10,
            },
        };

        customizer?.Invoke(request);
        return request;
    }
}
