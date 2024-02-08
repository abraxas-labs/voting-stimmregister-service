// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Adapter.Data.Configuration;
using Voting.Stimmregister.Adapter.Data.Repositories;

namespace Voting.Stimmregister.Adapter.Data.DependencyInjection;

/// <summary>
/// Service collection extensions to register Adapter.Data services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the data services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="dataConfig">The data configuration which will be added as Singleton.</param>
    /// <param name="optionsBuilder">The db context options builder to configure additional db options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAdapterDataServices(
        this IServiceCollection services,
        DataConfig dataConfig,
        Action<DbContextOptionsBuilder> optionsBuilder)
    {
        services.AddDbContext<IDataContext, DataContext>(db =>
        {
            if (dataConfig.EnableDetailedErrors)
            {
                db.EnableDetailedErrors();
            }

            if (dataConfig.EnableSensitiveDataLogging)
            {
                db.EnableSensitiveDataLogging();
            }

            db.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            optionsBuilder(db);
        });

        return services
            .AddSingleton(dataConfig)
            .AddScoped<IBatchInserter, DataContextBatchInserter>()
            .AddScoped<IAccessControlListDoiRepository, AccessControlListDoiRepository>()
            .AddScoped<IDomainOfInfluenceRepository, DomainOfInfluenceRepository>()
            .AddScoped<IEVoterRepository, EVoterRepository>()
            .AddScoped<IBfsIntegrityRepository, BfsIntegrityRepository>()
            .AddScoped<IEVoterAuditRepository, EVoterAuditRepository>()
            .AddScoped<IFilterCriteriaRepository, FilterCriteriaRepository>()
            .AddScoped<IFilterRepository, FilterRepository>()
            .AddScoped<IFilterVersionPersonRepository, FilterVersionPersonRepository>()
            .AddScoped<IFilterVersionRepository, FilterVersionRepository>()
            .AddScoped<IImportStatisticRepository, ImportStatisticRepository>()
            .AddScoped<IIntegrityRepository, IntegrityRepository>()
            .AddScoped<IPersonRepository, PersonRepository>()
            .AddScoped<IPersonDoiRepository, PersonDoiRepository>()
            .AddScoped<ILastSearchParameterRepository, LastSearchParameterRepository>()
            .AddVotingLibDatabase<DataContext>();
    }
}
