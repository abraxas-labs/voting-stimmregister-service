// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Common;
using Voting.Stimmregister.Adapter.Data;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public class TestReadOnlyApplicationFactory : TestApplicationFactory<TestReadOnlyDbStartup>, IAsyncLifetime
{
    private static readonly AsyncLock _mockDataSeederLock = new();
    private static volatile bool _mockDataLoaded;

    public async Task InitializeAsync()
    {
        // this method is executed once per test class
        // but we want too seed data only once.
        if (_mockDataLoaded)
        {
            return;
        }

        using var locker = await _mockDataSeederLock.AcquireAsync();
        if (_mockDataLoaded)
        {
            return;
        }

        await RunScoped(sp => DatabaseUtil.Truncate(sp.GetRequiredService<DataContext>()));
        await MockDataSeeder.Seed(RunScoped);

        _mockDataLoaded = true;
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
