// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Core.Services;

namespace Voting.Stimmregister.Test.Utils.Mocks;

/// <summary>
/// A no-op import worker trigger.
/// Don't trigger imports automatically to prevent unpredictable timings.
/// </summary>
public class MockImportWorkerTrigger : ImportWorkerTrigger
{
    public MockImportWorkerTrigger(IServiceScopeFactory scopeFactory)
        : base(scopeFactory)
    {
    }

    internal override Task RunImports(CancellationToken ct)
        => Task.CompletedTask;
}
