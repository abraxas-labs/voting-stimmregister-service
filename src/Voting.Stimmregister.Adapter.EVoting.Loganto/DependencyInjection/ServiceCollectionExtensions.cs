// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.EVoting.Loganto;
using Voting.Stimmregister.Adapter.EVoting.Loganto.Services;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.Adapter.EVoting.Loganto.DependencyInjection;

/// <summary>
/// Service collection extensions to register Adapter.Loganto services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the adapter loganto services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="logantoServiceUrl">The URL to the loganto service.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAdapterEvotingLogantoServices(this IServiceCollection services, string logantoServiceUrl)
    {
        services.AddHttpClient<ILogantoAdapter, LogantoAdapter>(client =>
            {
                client.BaseAddress = new Uri(logantoServiceUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddSecureConnectServiceToken(EVotingConfig.SecureConnectSharedEVotingOptionKey);

        return services.AddSystemClock();
    }
}
