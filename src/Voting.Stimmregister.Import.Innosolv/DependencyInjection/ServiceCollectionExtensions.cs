// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using ABX_Voting_1_0;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Ech.AbxVoting_1_0.DependencyInjection;
using Voting.Stimmregister.Abstractions.Import.Extensions;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Import.Innosolv.Mapping;
using Voting.Stimmregister.Import.Innosolv.Services;

namespace Voting.Stimmregister.Import.Innosolv.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdapterInnosolv(this IServiceCollection services)
    {
        return services
            .AddPersonImportService<PersonInfoType, InnosolvPersonMapper, InnosolvPersonImportService>(ImportSourceSystem.Innosolv)
            .AddAbxVoting();
    }
}
