// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Diagnostics;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Core.HostedServices;

/// <summary>
/// Hosted service to initialize import metrics on application start.
/// </summary>
public class ImportMetricsHostedService(
    ILogger<ImportMetricsHostedService> logger,
    IServiceScopeFactory serviceScopeFactory,
    ImportsConfig importsConfig,
    IClock clock)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Start refreshing import metrics.");

        try
        {
            using var scope = serviceScopeFactory.CreateScope();

            var importStatisticRepository = scope.ServiceProvider.GetRequiredService<IImportStatisticRepository>();
            var latestImports = importStatisticRepository.Query().Where(i =>
                    i.ImportStatus.Equals(ImportStatus.FinishedSuccessfully) &&
                    (i.ImportType.Equals(ImportType.DomainOfInfluence) || i.ImportType.Equals(ImportType.Person)) &&
                    (i.SourceSystem.Equals(ImportSourceSystem.Innosolv) || i.SourceSystem.Equals(ImportSourceSystem.Loganto)) &&
                    i.IsLatest)
                .GroupBy(i => new { i.MunicipalityId, i.MunicipalityName, i.ImportType, i.SourceSystem })
                .Select(g => new
                {
                    g.Key.MunicipalityId,
                    g.Key.MunicipalityName,
                    g.Key.ImportType,
                    g.Key.SourceSystem,
                    LatestFinishedDate = g.Max(i => i.FinishedDate),
                })
                .ToList();

            foreach (var import in latestImports)
            {
                if (importsConfig.AllowedPersonImportSourceSystemByMunicipalityId.TryGetValue(import.MunicipalityId!.Value, out var config) &&
                    config.StartingDate < clock.UtcNow &&
                    config.ImportSourceSystem != import.SourceSystem)
                {
                    logger.LogInformation(
                        "Skip writing metrics of latest import for '{MunicipalityId} ({ImportType} / {SourceSystem})' from '{Date}' because it's from a disabled source system.",
                        import.MunicipalityId,
                        import.ImportType,
                        import.SourceSystem,
                        import.LatestFinishedDate);

                    continue;
                }

                DiagnosticsConfig.SetImportLatestTimestamp(
                    import.ImportType.ToString(),
                    import.MunicipalityId,
                    import.MunicipalityName,
                    import.LatestFinishedDate);
            }

            logger.LogInformation("Successfully refreshed import metrics.");
        }
        catch (OperationCanceledException)
        {
            // Prevent throwing if cancellation was signaled
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred executing import metrics hosted service.");
        }

        return Task.CompletedTask;
    }
}
