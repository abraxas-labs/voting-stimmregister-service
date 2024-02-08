// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

public interface IFilterRepository : IDbRepository<DbContext, FilterEntity>
{
    Task<List<FilterCriteriaEntity>> GetCriteriasByFilterId(Guid filterId);
}
