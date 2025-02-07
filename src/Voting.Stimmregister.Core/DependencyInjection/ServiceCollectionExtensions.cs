// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Security.Cryptography;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Scheduler;
using Voting.Stimmregister.Abstractions.Adapter.Models;
using Voting.Stimmregister.Abstractions.Core.Configuration;
using Voting.Stimmregister.Abstractions.Core.Import.Mapping;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Abstractions.Core.Queues;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Abstractions;
using Voting.Stimmregister.Core.Configuration;
using Voting.Stimmregister.Core.Diagnostics;
using Voting.Stimmregister.Core.HostedServices;
using Voting.Stimmregister.Core.Queues;
using Voting.Stimmregister.Core.Services;
using Voting.Stimmregister.Core.Services.Caching;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;
using Voting.Stimmregister.Core.Validators;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.DependencyInjection;

/// <summary>
/// Service collection extensions to register Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the core services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="eVotingConfig">The eVoting configuration.</param>
    /// <param name="memoryCacheConfig">The memory cache configuration.</param>
    /// <param name="filterConfig">The filter configuration.</param>
    /// <param name="personConfig">The person configuration.</param>
    /// <param name="importsConfig">The importers configuration.</param>
    /// <param name="cleanupConfig">The cleanup configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        EVotingConfig eVotingConfig,
        MemoryCacheConfig memoryCacheConfig,
        FilterConfig filterConfig,
        PersonConfig personConfig,
        ImportsConfig importsConfig,
        CleanupConfig cleanupConfig)
    {
        services
            .AddSingleton<ICantonBfsCache, CantonBfsCache>()
            .AddSingleton<IMunicipalityIdCantonCache, MunicipalityIdCantonCache>()
            .AddSingleton<IEVotersCache, EVotersCache>()
            .AddTransient(typeof(ActivityFactory<>))
            .AddSingleton(eVotingConfig)
            .AddSingleton(memoryCacheConfig)
            .AddSingleton(filterConfig)
            .AddSingleton(personConfig)
            .AddSingleton(cleanupConfig)
            .AddScoped<IDomainOfInfluenceImportQueue, DomainOfInfluenceImportQueue>()
            .AddScoped<ImportQueue>()
            .AddSingleton<ImportFileService>()
            .AddSingleton<ImportWorkerTrigger>()
            .AddSingleton<ImportServiceRegistry>()
            .AddScoped<ImportWorkerService>()
            .AddScoped<IPersonImportQueue, PersonImportQueue>()
            .AddSingleton<SignaturePayloadBuilderFactory>()
            .AddTransient<PersonSignaturePayloadBuilderV1>()
            .AddTransient<PersonSignaturePayloadBuilderV2>()
            .AddTransient<IntegritySignaturePayloadBuilderV1>()
            .AddTransient<IntegritySignaturePayloadBuilderV2>()
            .AddTransient<DomainOfInfluenceSignaturePayloadBuilderV1>()
            .AddTransient<FilterVersionSignaturePayloadBuilderV1>()
            .AddTransient<FilterVersionSignaturePayloadBuilderV2>()
            .AddSingleton<SignatureVerifier>()
            .AddSingleton<SignatureCreator>()
            .AddScoped<BfsIntegrityPersonsVerifier>()
            .AddSingleton<ICreateSignatureService, CreateVerifySignatureService>()
            .AddSingleton<IVerifySigningService, CreateVerifySignatureService>()
            .AddSingleton<IStreamEncryptionService, AesStreamEncryption>()
            .AddSingleton<IStreamDecryptionService, AesStreamEncryption>()
            .AddScoped<ICsvService, CsvService>()
            .AddScoped<IEVotingService, EVotingService>()
            .AddScoped<ILanguageService, LanguageService>()
            .AddScoped<IImportStatisticService, ImportStatisticService>()
            .AddScoped<IExportCsvService, ExportCsvService>()
            .AddScoped<IExportEchService, ExportEchService>()
            .AddScoped<IPersonService, PersonService>()
            .AddScoped<IFilterService, FilterService>()
            .AddScoped<ITracingService, TracingService>()
            .AddScoped<ILastSearchParameterService, LastSearchParameterService>()
            .AddScoped<IRegistrationStatisticService, RegistrationStatisticService>()
            .AddHashBuilderPool(HashAlgorithmName.SHA512)
            .AddSystemClock()
            .AddHostedService<ImportMetricsHostedService>()
            .AddScoped<IDatabaseCleanupService, DatabaseCleanupService>()
            .AddCronJob<CleanupScheduledJob>(cleanupConfig);

        if (importsConfig.RunImports)
        {
            services.AddScheduledJob<ImportScheduledJob>(importsConfig.Job);
        }

        return services;
    }

    public static IServiceCollection AddImportService<TService, TImplementation>(
        this IServiceCollection services,
        ImportSourceSystem sourceSystem,
        ImportType type)
        where TService : class, IImportService
        where TImplementation : class, TService
    {
        services.AddSingleton(new ImportServiceRegistration(sourceSystem, type, typeof(TService)));
        services.AddScoped<TService, TImplementation>();
        return services;
    }

    public static IServiceCollection AddPersonImportService<TRecord, TMapper, TImportService>(
        this IServiceCollection services,
        ImportSourceSystem sourceSystem)
        where TMapper : class, IPersonRecordEntityMapper<TRecord>
        where TImportService : PersonImportService<TRecord>
    {
        return services
            .AddScoped<IPersonRecordEntityMapper<TRecord>, TMapper>()
            .AddImportService<IPersonImportService<TRecord>, TImportService>(sourceSystem, ImportType.Person);
    }

    public static IServiceCollection AddCsvPersonImportService<TCsvRecord, TMapper>(
        this IServiceCollection services,
        ImportSourceSystem sourceSystem)
        where TCsvRecord : IPersonCsvRecord
        where TMapper : class, IPersonRecordEntityMapper<TCsvRecord>
        => services.AddPersonImportService<TCsvRecord, TMapper, PersonCsvImportService<TCsvRecord>>(sourceSystem);

    public static IServiceCollection AddImportDependencies(this IServiceCollection services)
    {
        services.AddScoped<IValidator<PersonEntity>, PersonEntityValidator>();
        services.AddScoped<IBfsStatisticService, BfsStatisticService>();
        return services;
    }
}
