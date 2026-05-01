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

public class GetByFilterVersionIdTest : BaseWriteableDbGrpcTest<PersonService.PersonServiceClient>
{
    public GetByFilterVersionIdTest(TestApplicationFactory factory)
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
    public async Task ShouldGetPersonsInCorrectOrder()
    {
        var response = await SgManagerClient.GetByFilterVersionIdAsync(NewValidGetByFilterVersionIdRequest(x => x.VersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id.ToString()));

        // this should ensure that all persons are sorted correctly (official name then first name then date of birth)
        response.People.MatchSnapshot();
    }

    [Fact]
    public async Task WhenRoleUnauthorizedShouldPermissionDenied()
    {
        var rolesArray = new[]
        {
            Roles.ApiImporter,
            Roles.ApiExporter,
            Roles.ManualImporter,
            Roles.ManualExporter,
            Roles.ImportObserver,
        };

        var request = NewValidGetByFilterVersionIdRequest();
        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));

        var exception = await Assert.ThrowsAsync<RpcException>(async () =>
            await client.GetByFilterVersionIdAsync(request));
        Assert.Equal(StatusCode.PermissionDenied, exception.StatusCode);
    }

    protected override async Task AuthorizationTestCall(PersonService.PersonServiceClient service)
    {
        await service.GetByFilterVersionIdAsync(NewValidGetByFilterVersionIdRequest());
    }

    private static PersonServiceGetByFilterVersionIdRequest NewValidGetByFilterVersionIdRequest(Action<PersonServiceGetByFilterVersionIdRequest>? customizer = null)
    {
        var request = new PersonServiceGetByFilterVersionIdRequest();
        customizer?.Invoke(request);
        return request;
    }
}
