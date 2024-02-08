// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.EVoting.Kewr;
using Voting.Stimmregister.Adapter.EVoting.Kewr.Services;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.DependencyInjection;

/// <summary>
/// Service collection extensions to register Adapter.EVoting.Loganto services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the adapter loganto services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="kewrServiceUrl">The URL to the kewr service.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAdapterEVotingKewrServices(this IServiceCollection services, string kewrServiceUrl)
    {
        services.AddHttpClient<IKewrAdapter, KewrAdapter>(client =>
            {
                client.BaseAddress = new Uri(kewrServiceUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddSecureConnectServiceToken(EVotingConfig.SecureConnectSharedEVotingOptionKey);

        return services.AddSystemClock();
    }
}
