// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

public class FilterRepository : DbRepository<DataContext, FilterEntity>, IFilterRepository
{
    public FilterRepository(DataContext context)
        : base(context)
    {
    }

    public async Task<List<FilterCriteriaEntity>> GetCriteriasByFilterId(Guid filterId)
    {
        if (!await ExistsByKey(filterId))
        {
            throw new EntityNotFoundException(typeof(FilterEntity), filterId);
        }

        return await Query()
            .Where(filter => filter.Id == filterId)
            .Include(filter => filter.FilterCriterias)
            .SelectMany(filter => filter.FilterCriterias)
            .ToListAsync();
    }
}
