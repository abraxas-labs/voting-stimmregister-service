// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Configuration;
using Voting.Stimmregister.Core.HostedServices;
using Voting.Stimmregister.Test.Utils.Helpers;
using Xunit;

namespace Voting.Stimmregister.Core.Unit.Tests.HostedServices;

public class CleanupScheduledJobTest : BaseWriteableDbTest
{
    public CleanupScheduledJobTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async void Run_WhenActive_ShouldAcquireDistributedLock()
    {
        var config = new CleanupConfig
        {
            IsActive = true,
        };
        var cleanupService = new Mock<IDatabaseCleanupService>();
        var logger = new Mock<ILogger<CleanupScheduledJob>>();
        var distributedLockProvider = GetService<IDistributedLockProvider>();

        var callOrder = 0;
        logger.Setup(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Start cleaning up...")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!))
            .Callback(() => Assert.Equal(0, callOrder++));
        cleanupService.Setup(x => x.RunCleanup()).Callback(() => Assert.Equal(1, callOrder++));
        logger.Setup(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully cleaned up.")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!))
            .Callback(() => Assert.Equal(2, callOrder++));

        // Simulate with 5 nodes
        var nodes = Enumerable.Range(1, 5).ToList();
        await Parallel.ForEachAsync(nodes, async (_, _) =>
        {
            var job = new CleanupScheduledJob(config, cleanupService.Object, logger.Object, distributedLockProvider);
            await job.Run(CancellationToken.None);
        });

        cleanupService.Verify(x => x.RunCleanup(), Times.Once);
    }

    [Fact]
    public async void Run_WhenInactive_ShouldNeverRun()
    {
        var config = new CleanupConfig
        {
            IsActive = false,
        };
        var cleanupService = new Mock<IDatabaseCleanupService>();
        var logger = new Mock<ILogger<CleanupScheduledJob>>();
        var distributedLockProvider = GetService<IDistributedLockProvider>();

        // Simulate with 5 nodes
        var nodes = Enumerable.Range(1, 5).ToList();
        await Parallel.ForEachAsync(nodes, async (_, _) =>
        {
            var job = new CleanupScheduledJob(config, cleanupService.Object, logger.Object, distributedLockProvider);
            await job.Run(CancellationToken.None);
        });

        cleanupService.Verify(x => x.RunCleanup(), Times.Never);
    }
}
