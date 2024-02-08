// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Models;
using Voting.Stimmregister.Abstractions.Import.Mapping;
using Voting.Stimmregister.Abstractions.Import.Services;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Abstractions.Import.Extensions;

public static class ServiceCollectionExtensions
{
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
}
