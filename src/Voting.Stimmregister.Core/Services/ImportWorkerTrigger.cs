// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Common;

namespace Voting.Stimmregister.Core.Services;

/// <summary>
/// Import worker trigger.
/// </summary>
public class ImportWorkerTrigger
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AsyncLock _lock = new();

    public ImportWorkerTrigger(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Ensures the imports are running in the background.
    /// </summary>
    internal async void TriggerImportWorker()
        => await RunImports(default);

    /// <summary>
    /// Triggers the <see cref="ImportWorkerService"/> if not already working on imports.
    /// If the <see cref="ImportWorkerService"/> is already working on imports,
    /// the method returns straight away.
    /// Otherwise the method returns as soon as all imports are done or the cancellation token requests cancellation.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    internal virtual async Task RunImports(CancellationToken ct)
    {
        if (!_lock.TryAcquireImmediately(out var locker))
        {
            // importer already working...
            return;
        }

        using var locker2 = locker;
        await using var scope = _scopeFactory.CreateAsyncScope();
        var worker = scope.ServiceProvider.GetRequiredService<ImportWorkerService>();
        await worker.ProcessImports(ct);
    }
}
