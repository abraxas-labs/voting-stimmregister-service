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

namespace Voting.Stimmregister.WebService.Integration.Tests.PersonGrpcServiceTests;

public class GetSingleTest : BaseWriteableDbGrpcTest<PersonService.PersonServiceClient>
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
        await BfsIntegrityMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task WhenRegisterIdIsGuidEmpty_ShouldThrowInvalidArgument()
    {
        await AssertStatus(
            async () => await SgManagerClient.GetSingleAsync(
                NewValidGetSingleRequest(x => x.RegisterId = Guid.Empty.ToString())),
            StatusCode.InvalidArgument);
    }

    [Fact]
    public async Task WhenRegisterIdValid_ShouldResolvePerson()
    {
        var request = NewValidGetSingleRequest();
        var response = await SgManagerClient.GetSingleAsync(request);

        Assert.True(response.Latest != null);
        response.MatchSnapshot();
    }

    [Fact]
    public async Task WhenPersonIsDeleted_ShouldThrowInvalidArgument()
    {
        var request = new PersonServiceGetSingleRequest
        {
            RegisterId = PersonMockedData.Person_3203_StGallen_Deleted_1.RegisterId.ToString(),
        };

        await AssertStatus(async () => await SgManagerClient.GetSingleAsync(request), StatusCode.InvalidArgument);
    }

    [Fact]
    public async Task WhenRegisterIdIsGuidEmpty_RoleUnauthorized_ShouldPermissionDenied()
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
        await AssertStatus(
            async () => await client.GetSingleAsync(
                NewValidGetSingleRequest(x => x.RegisterId = Guid.Empty.ToString())),
            StatusCode.PermissionDenied);
    }

    protected override async Task AuthorizationTestCall(PersonService.PersonServiceClient service)
    {
        await service.GetSingleAsync(NewValidGetSingleRequest());
    }

    private static PersonServiceGetSingleRequest NewValidGetSingleRequest(Action<PersonServiceGetSingleRequest>? customizer = null)
    {
        var request = new PersonServiceGetSingleRequest
        {
            RegisterId = PersonMockedData.Person_3203_StGallen_1.RegisterId.ToString(),
        };

        customizer?.Invoke(request);
        return request;
    }
}
