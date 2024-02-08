// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Scheduler;
using Voting.Stimmregister.Abstractions.Core.Configuration;
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
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Cryptography;

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
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        EVotingConfig eVotingConfig,
        MemoryCacheConfig memoryCacheConfig,
        FilterConfig filterConfig,
        PersonConfig personConfig,
        ImportsConfig importsConfig)
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
            .AddScoped<IDomainOfInfluenceImportQueue, DomainOfInfluenceImportQueue>()
            .AddScoped<ImportQueue>()
            .AddSingleton<ImportFileService>()
            .AddSingleton<ImportWorkerTrigger>()
            .AddSingleton<ImportServiceRegistry>()
            .AddScoped<ImportWorkerService>()
            .AddScoped<IPersonImportQueue, PersonImportQueue>()
            .AddSingleton<SignaturePayloadBuilderFactory>()
            .AddTransient<PersonSignaturePayloadBuilderV1>()
            .AddTransient<IntegritySignaturePayloadBuilderV1>()
            .AddTransient<DomainOfInfluenceSignaturePayloadBuilderV1>()
            .AddTransient<FilterVersionSignaturePayloadBuilderV1>()
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
            .AddHashBuilderPool(HashAlgorithmName.SHA512)
            .AddSystemClock();

        if (importsConfig.RunImports)
        {
            services.AddScheduledJob<ImportScheduledJob>(importsConfig.Job);
        }

        return services;
    }
}
