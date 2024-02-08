// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Testing;
using Voting.Stimmregister.Adapter.Data;
using Xunit;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public class BaseDbTest<TFactory, TStartup> : BaseTest<TFactory, TStartup>
    where TFactory : BaseTestApplicationFactory<TStartup>
    where TStartup : class
{
    protected BaseDbTest(TFactory factory)
        : base(factory)
    {
    }

    protected void RunOnDb(Action<DataContext> action)
        => RunScoped(action);

    protected Task RunOnDb(Func<DataContext, Task> action, string? language = null)
    {
        return RunScoped<DataContext>(db =>
        {
            db.Language = language;
            return action(db);
        });
    }

    protected Task<TResult> RunOnDb<TResult>(Func<DataContext, Task<TResult>> action, string? language = null)
    {
        return RunScoped<DataContext, TResult>(db =>
        {
            db.Language = language;
            return action(db);
        });
    }

    protected Task<TEntity> GetDbEntity<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
        => RunOnDb(db => db.Set<TEntity>().FirstAsync(predicate));

    protected async Task ResetDb()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        await DatabaseUtil.Truncate(db);
    }

    protected async Task<T> AssertException<T>(Func<Task> testCode, string message)
        where T : Exception
    {
        var ex = await Assert.ThrowsAsync<T>(testCode);
        ex.Message.Should().Contain(message);
        return ex;
    }
}
