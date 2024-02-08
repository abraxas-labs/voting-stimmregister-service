// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Import.Extensions;
using Voting.Stimmregister.Abstractions.Import.Mapping;
using Voting.Stimmregister.Abstractions.Import.Services.Loganto;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Import.Loganto.Mapping;
using Voting.Stimmregister.Import.Loganto.Services;
using Voting.Stimmregister.Import.Loganto.Validators;

namespace Voting.Stimmregister.Import.Loganto.DependencyInjection;

/// <summary>
/// Service collection extensions to register Adapter.Loganto services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the adapter loganto services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAdapterLogantoServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IValidator<CobraPersonCsvRecord>, CobraPersonRecordValidator>()
            .AddScoped<IValidator<LogantoPersonCsvRecord>, LogantoPersonRecordValidator>()
            .AddScoped<IValidator<LogantoDomainOfInfluenceCsvRecord>, LogantoDomainOfInfluenceRecordValidator>()
            .AddScoped<IValidator<DomainOfInfluenceEntity>, DomainOfInfluenceEntityValidator>()
            .AddScoped<IValidator<PersonEntity>, PersonEntityValidator>()
            .AddLogantoDoiImportService()
            .AddCsvPersonImportService<LogantoPersonCsvRecord, LogantoPersonMapper>(ImportSourceSystem.Loganto)
            .AddCsvPersonImportService<CobraPersonCsvRecord, CobraPersonMapper>(ImportSourceSystem.Cobra);
    }

    private static IServiceCollection AddLogantoDoiImportService(this IServiceCollection services)
    {
        return services
            .AddScoped<IDomainOfInfluenceRecordEntityMapper<LogantoDomainOfInfluenceCsvRecord>, LogantoDomainOfInfluenceMapper>()
            .AddImportService<IDomainOfInfluenceImportService<LogantoDomainOfInfluenceCsvRecord>, LogantoDomainOfInfluenceImportService>(ImportSourceSystem.Loganto, ImportType.DomainOfInfluence);
    }
}
