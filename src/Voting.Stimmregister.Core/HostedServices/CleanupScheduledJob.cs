// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading;
using Microsoft.Extensions.Logging;
using Voting.Lib.Scheduler;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Configuration;

namespace Voting.Stimmregister.Core.HostedServices;

/// <summary>
/// Hosted service for cleanup of unused/unneeded data.
/// </summary>
public class CleanupScheduledJob(CleanupConfig config, IDatabaseCleanupService cleanupService, ILogger<CleanupScheduledJob> logger, IDistributedLockProvider distributedLockProvider) : IScheduledJob
{
    private const string LockKey = nameof(CleanupScheduledJob);

    public async Task Run(CancellationToken ct)
    {
        if (!config.IsActive)
        {
            logger.LogInformation("Cleanup is disabled and has been skipped.");

            return;
        }

        var @lock = distributedLockProvider.CreateLock(LockKey);
        await using var handle = await @lock.TryAcquireAsync(cancellationToken: ct);
        if (handle != null)
        {
            logger.LogInformation("Start cleaning up...");
            await cleanupService.RunCleanup();
            logger.LogInformation("Successfully cleaned up.");
        }
        else
        {
            logger.LogInformation("Cleanup has been skipped because another cleanup is running right now.");
        }
    }
}
