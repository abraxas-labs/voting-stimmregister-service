// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Common;
using Voting.Stimmregister.Adapter.Data;

namespace Voting.Stimmregister.Test.Utils.Helpers;

/// <summary>
/// Test util to prepare database for unit and integration tests.
/// </summary>
public static class DatabaseUtil
{
    private static readonly ConcurrentDictionary<string, bool> _migratedDbConnectionStrings = new();
    private static readonly AsyncLock _migrationLocker = new();

    /// <summary>
    /// Truncates the test database tables.
    /// Truncating tables is much faster than recreating the database.
    /// Fastest would probably be postgres template db's with our mock data,
    /// but they don't work easily with x unit test parallelization.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "EF1002:Risk of vulnerability to SQL injection.", Justification = "Referencing hardened inerpolated string parameters.")]
    public static async Task Truncate(DataContext db)
    {
        await EnsureMigrated(db);
        var tableNames = db.Model.GetEntityTypes().Select(m => $@"""{m.GetTableName()}""");
        await db.Database.ExecuteSqlRawAsync($"TRUNCATE {string.Join(",", tableNames)} CASCADE");
    }

    private static async Task EnsureMigrated(DataContext db)
    {
        var connectionString = db.Database.GetDbConnection().ConnectionString;
        if (_migratedDbConnectionStrings.TryGetValue(connectionString, out var migrated) && migrated)
        {
            return;
        }

        using var locker = await _migrationLocker.AcquireAsync();
        if (_migratedDbConnectionStrings.TryGetValue(connectionString, out migrated) && migrated)
        {
            return;
        }

        await db.Database.MigrateAsync();
        _migratedDbConnectionStrings.TryAdd(connectionString, true);
    }
}
