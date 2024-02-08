// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Voting.Stimmregister.Test.Utils.Helpers;

public abstract class BaseWriteableDbRestTest : BaseRestTest<TestApplicationFactory, TestStartup>
{
    protected BaseWriteableDbRestTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await ResetDb();
    }

    protected Task ResetDb() => RunOnDb(DatabaseUtil.Truncate);

    protected Task ModifyDbEntities<TEntity>(Expression<Func<TEntity, bool>> predicate, Action<TEntity> modifier)
        where TEntity : class
    {
        return RunOnDb(async db =>
        {
            var set = db.Set<TEntity>();
            var entities = await set.AsTracking().Where(predicate).ToListAsync();

            foreach (var entity in entities)
            {
                modifier(entity);
            }

            await db.SaveChangesAsync();
        });
    }
}
