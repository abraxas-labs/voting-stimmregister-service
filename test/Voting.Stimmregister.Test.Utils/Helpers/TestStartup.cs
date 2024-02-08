// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voting.Lib.Ech;
using Voting.Lib.Testing.Mocks;
using Voting.Stimmregister.Adapter.VotingBasis.HostedServices;
using Voting.Stimmregister.Core.Services;
using Voting.Stimmregister.Test.Utils.MockData;
using Voting.Stimmregister.Test.Utils.Mocks;
using Voting.Stimmregister.WebService;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public class TestStartup : Startup
{
    public TestStartup(IConfiguration configuration, IWebHostEnvironment environment)
        : base(configuration, environment)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services
            .AddMockedClock()
            .AddVotingLibIamMocks()
            .AddVotingLibPkcs11Mock()
            .RemoveHostedServices()
            .RemoveAll<IAccessControlListDoiHostedService>()
            .AddSingleton<IHttpClientFactory, HttpClientFactoryMocked>()
            .AddMock<IEchMessageIdProvider, MockEchMessageIdProvider>()
            .AddMock<ImportFileService, MockImportFileService>()
            .AddMock<ImportWorkerTrigger, MockImportWorkerTrigger>();
    }

    protected override void ConfigureAuthentication(AuthenticationBuilder builder)
        => builder.AddMockedSecureConnectScheme();
}
