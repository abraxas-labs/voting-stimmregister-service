// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Core.DependencyInjection;
using Voting.Stimmregister.Core.Import.Cobra.Tg.Mapping;
using Voting.Stimmregister.Core.Import.Cobra.Tg.Validators;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.Import.Cobra.Tg.DependencyInjection;

/// <summary>
/// Service collection extensions to register Adapter.Cobra services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the adapter cobra services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAdapterCobraTgServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IValidator<CobraTgPersonCsvRecord>, CobraPersonRecordValidator>()
            .AddCsvPersonImportService<CobraTgPersonCsvRecord, CobraPersonMapper>(ImportSourceSystem.CobraTg);
    }
}
