// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Medallion.Threading;
using Medallion.Threading.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Database.Interceptors;
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
        services.AddDbContext<IDataContext, DataContext>((serviceProvider, db) =>
        {
            if (dataConfig.EnableDetailedErrors)
            {
                db.EnableDetailedErrors();
            }

            if (dataConfig.EnableSensitiveDataLogging)
            {
                db.EnableSensitiveDataLogging();
            }

            if (dataConfig.EnableMonitoring)
            {
                db.AddInterceptors(serviceProvider.GetRequiredService<DatabaseQueryMonitoringInterceptor>());
            }

            db.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            optionsBuilder(db);
        });

        if (dataConfig.EnableMonitoring)
        {
            services.AddDataMonitoring(dataConfig.Monitoring);
        }

        return services
            .AddSingleton(dataConfig)
            .AddSingleton<IDistributedLockProvider>(_ => new PostgresDistributedSynchronizationProvider(dataConfig.ConnectionString))
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
            .AddScoped<IBfsStatisticRepository, BfsStatisticRepository>()
            .AddVotingLibDatabase<DataContext>();
    }
}
