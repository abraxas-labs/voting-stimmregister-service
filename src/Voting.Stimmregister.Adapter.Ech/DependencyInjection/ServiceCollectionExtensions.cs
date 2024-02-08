// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Ech.Ech0045_4_0.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Ech;
using Voting.Stimmregister.Adapter.Ech.Configuration;
using Voting.Stimmregister.Adapter.Ech.Converter;
using Voting.Stimmregister.Adapter.Ech.Mapping;

namespace Voting.Stimmregister.Adapter.Ech.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdapterEch(this IServiceCollection services, EchConfig config)
    {
        return services
            .AddSingleton(config)
            .AddVotingLibEch(config)
            .AddEch0045()
            .AddSingleton<IEchService, EchService>()
            .AddSingleton<IPersonVoterMapping, PersonVoterMapping>();
    }
}
