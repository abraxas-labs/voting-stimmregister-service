// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Voting.Stimmregister.Test.Utils.Helpers;

/// <summary>
/// This base class uses the writeable db and the db xunit test collection to ensure only 1 test is run in parallel.
/// During initialization it resets the db and seeds the mock data (for each test).
/// </summary>
/// <typeparam name="TService">The grpc service client type.</typeparam>
[Collection(WriteableDbTestCollection.Name)]
public abstract class BaseWriteableDbGrpcTest<TService> : BaseGrpcTest<TService, TestApplicationFactory, TestStartup>
    where TService : ClientBase<TService>
{
    protected BaseWriteableDbGrpcTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await ResetDb();
    }

    protected Task ModifyDbEntities<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity> modifier)
        where TEntity : class
    {
        return RunOnDb(async db =>
        {
            var set = db.Set<TEntity>();
            var entities = await set.AsTracking().IgnoreQueryFilters().Where(predicate).ToListAsync();

            foreach (var entity in entities)
            {
                modifier(entity);
            }

            await db.SaveChangesAsync();
        });
    }

    protected Task ResetDb() => RunOnDb(DatabaseUtil.Truncate);
}
