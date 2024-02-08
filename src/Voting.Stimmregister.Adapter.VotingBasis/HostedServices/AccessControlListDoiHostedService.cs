// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.Adapter.VotingBasis.HostedServices;

/// <inheritdoc cref="IAccessControlListDoiHostedService"/>
internal class AccessControlListDoiHostedService : BackgroundService, IAccessControlListDoiHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly VotingBasisConfig _config;
    private readonly ILogger<AccessControlListDoiHostedService> _logger;
    private CrontabSchedule? _schedule;
    private DateTime _nextRun = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessControlListDoiHostedService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="config">The VOTING Basis configuration.</param>
    /// <param name="logger">The logger.</param>
    public AccessControlListDoiHostedService(
        IServiceProvider serviceProvider,
        VotingBasisConfig config,
        ILogger<AccessControlListDoiHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _config = config;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _schedule = CrontabSchedule.Parse(_config.CronScheduleDoiAclSync, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            if (DateTime.UtcNow >= _nextRun)
            {
                await Process();
                _nextRun = _schedule!.GetNextOccurrence(DateTime.UtcNow);
            }

            await Task.Delay(1000, stoppingToken);
        }
        while (!stoppingToken.IsCancellationRequested);
    }

    private async Task Process()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<IPermissionService>().SetAbraxasAuthIfNotAuthenticated();
            var service = scope.ServiceProvider.GetRequiredService<IAccessControlListImportService>();
            await service.ImportAcl();
        }
        catch (OperationCanceledException)
        {
            // Prevent throwing if cancellation was signaled
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred executing access control list DOI import.");
        }
    }
}
