// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AbxVoting_1_5;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Ech.AbxVoting_1_5.DependencyInjection;
using Voting.Stimmregister.Core.DependencyInjection;
using Voting.Stimmregister.Core.Import.Innosolv.Mapping;
using Voting.Stimmregister.Core.Import.Innosolv.Services;
using Voting.Stimmregister.Domain.Enums;

using AbxVoting10Extensions = Voting.Lib.Ech.AbxVoting_1_0.DependencyInjection.ServiceCollectionExtensions;

namespace Voting.Stimmregister.Core.Import.Innosolv.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdapterInnosolv(this IServiceCollection services)
    {
        services = services
            .AddPersonImportService<PersonInfoType, InnosolvPersonMapper, InnosolvPersonImportService>(ImportSourceSystem.Innosolv)
            .AddAbxVoting();

        AbxVoting10Extensions.AddAbxVoting(services);
        return services;
    }
}
