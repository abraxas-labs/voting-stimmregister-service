// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Net.Http;
using Abraxas.Voting.Basis.Services.V1;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Adapter.VotingBasis.HostedServices;
using Voting.Stimmregister.Adapter.VotingBasis.Services;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.Adapter.VotingBasis.DependencyInjection;

/// <summary>
/// Service collection extensions to register Adapter.VotingBasis services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the adapter VOTING Basis services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="config">The voting basis configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAdapterVotingBasisServices(this IServiceCollection services, VotingBasisConfig config)
    {
        services
            .AddGrpcClient<AdminManagementService.AdminManagementServiceClient>(opts => opts.Address = config.ApiEndpoint)
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return config.EnableGrpcWeb ?
                    new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler()) :
                    new HttpClientHandler();
            })
            .AddHttpMessageHandler(_ => new AdminManagementServiceClientHandler(config))
            .AddSecureConnectServiceToken(config.IdpServiceAccount);

        // AddHostedService (Generic) only supports specifying the implementation, but not the interface.
        // This is done to Be able to replace the HostedService with a mock in integration tests
        services.AddSingleton<IAccessControlListDoiHostedService, AccessControlListDoiHostedService>();
        services.AddTransient<IAccessControlListDoiService, AccessControlListDoiService>();
        services.AddTransient<IAccessControlListImportService, AccessControlListImportService>();
        services.AddHostedService(sp => sp.GetService<IAccessControlListDoiHostedService>());

        return services;
    }
}
