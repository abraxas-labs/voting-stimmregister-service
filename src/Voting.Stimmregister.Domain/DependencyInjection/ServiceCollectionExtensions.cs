// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Utils;

namespace Voting.Stimmregister.Domain.DependencyInjection;

/// <summary>
/// Service collection extensions to register Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the domain services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="importsConfig">The imports configuration which will be added as Singleton.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDomainServices(
        this IServiceCollection services,
        ImportsConfig importsConfig)
    {
        services
            .AddSingleton(importsConfig)
            .AddSingleton(importsConfig.DomainOfInfluence)
            .AddSingleton(importsConfig.Person)
            .AddSingleton(importsConfig.Person.Loganto)
            .AddSingleton(importsConfig.VotingBasis)
            .AddSingleton<ICountryHelperService, CountryHelperService>();

        return services;
    }
}
