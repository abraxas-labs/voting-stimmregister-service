// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

public class FilterVersionRepository : DbRepository<DataContext, FilterVersionEntity>, IFilterVersionRepository
{
    public FilterVersionRepository(DataContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<int> DeleteOutdatedFilterVersions(DateTime thresholdDate, int sqlCommandTimeoutInSeconds)
    {
        Context.Database.SetCommandTimeout(sqlCommandTimeoutInSeconds);

        return await Set
            .IgnoreQueryFilters()
            .Where(f => f.AuditInfo.CreatedAt < thresholdDate)
            .ExecuteDeleteAsync();
    }
}
