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

public class GetHistoryTest : BaseWriteableDbGrpcTest<ImportStatisticService.ImportStatisticServiceClient>
{
    public GetHistoryTest(TestApplicationFactory factory)
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
    public async Task ShouldGetHistoryWhenAuthorizedByAcl()
    {
        var request = NewValidGetAllRequest();
        var response = await SgManagerClient.GetHistoryAsync(request);

        Assert.True(response.ImportStatistics.Count > 0);
        response.MatchSnapshot();
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

        var request = NewValidGetAllRequest();
        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));
        await AssertStatus(
            async () => await client.GetHistoryAsync(request), StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(ImportStatisticService.ImportStatisticServiceClient service)
    {
        await service.GetHistoryAsync(NewValidGetAllRequest());
    }

    private static GetImportStatisticHistoryRequest NewValidGetAllRequest(Action<GetImportStatisticHistoryRequest>? customizer = null)
    {
        var request = new GetImportStatisticHistoryRequest
        {
            ImportType = ImportType.DomainOfInfluence,
            ImportSourceSystem = ImportSourceSystem.Innosolv,
            MunicipalityId = ImportMockedData.MunicipalityId,
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
