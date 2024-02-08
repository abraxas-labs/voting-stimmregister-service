// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading;
using System.Threading.Tasks;
using Voting.Lib.Scheduler;
using Voting.Stimmregister.Core.Services;

namespace Voting.Stimmregister.Core.HostedServices;

/// <summary>
/// Hosted service to process imports.
/// </summary>
public sealed class ImportScheduledJob : IScheduledJob
{
    private readonly ImportWorkerTrigger _importWorkerTrigger;

    public ImportScheduledJob(ImportWorkerTrigger importWorkerTrigger)
    {
        _importWorkerTrigger = importWorkerTrigger;
    }

    public Task Run(CancellationToken ct)
        => _importWorkerTrigger.RunImports(ct);
}
