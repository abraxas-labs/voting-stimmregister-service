// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Grpc.Core;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.RegistrationStatisticGrpcServiceTest;

public class ListStatisticsTest : BaseWriteableDbGrpcTest<RegistrationStatisticService.RegistrationStatisticServiceClient>
{
    public ListStatisticsTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await BfsStatisticMockedData.Seed(RunScoped);
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldListStatisticsWhenAuthorizedByAcl()
    {
        var request = NewValidRequest();
        var response = await RegistrationStatisticReaderClient.ListAsync(request);

        Assert.Equal(2, response.MunicipalityRegistrationStatistics.Count);
        Assert.True(response.IsTopLevelAuthority);
        response.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfStatisticsWhenNotAuthorizedByAcl()
    {
        var request = NewValidRequest();
        var response = await UnknownRegistrationStatisticReaderClient.ListAsync(request);

        Assert.Empty(response.MunicipalityRegistrationStatistics);
        Assert.False(response.IsTopLevelAuthority);
        Assert.Equal(0, response.TotalRegistrationStatistic.VoterTotalCount);
    }

    [Fact]
    public async Task WhenRoleUnauthorizedShouldReturnPermissionDenied()
    {
        var rolesArray = new[]
        {
            Roles.Reader,
            Roles.Manager,
            Roles.ApiImporter,
            Roles.ApiExporter,
            Roles.ManualImporter,
            Roles.ManualExporter,
            Roles.ImportObserver,
            Roles.EVoting,
        };

        var request = NewValidRequest();
        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));
        await AssertStatus(
            async () => await client.ListAsync(request), StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(RegistrationStatisticService.RegistrationStatisticServiceClient service)
    {
        await service.ListAsync(NewValidRequest());
    }

    private static ListRegistrationStatisticRequest NewValidRequest(Action<ListRegistrationStatisticRequest>? customizer = null)
    {
        var request = new ListRegistrationStatisticRequest();

        customizer?.Invoke(request);
        return request;
    }
}
