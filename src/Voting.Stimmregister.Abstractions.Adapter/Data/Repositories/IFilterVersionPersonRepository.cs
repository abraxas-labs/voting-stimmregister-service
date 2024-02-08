// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

public interface IFilterVersionPersonRepository : IDbRepository<DbContext, FilterVersionPersonEntity>
{
    /// <summary>
    /// Lists all bfs of all persons in the filter version.
    /// </summary>
    /// <param name="filterVersionId">The filter version id.</param>
    /// <returns>A list of all bfs.</returns>
    Task<IReadOnlyCollection<int>> ListBfsIgnoreAcl(Guid filterVersionId);

    /// <summary>
    /// Counts the total number of persons and the number of invalid persons.
    /// </summary>
    /// <param name="filterVersionId">The filter version id.</param>
    /// <returns>The counts.</returns>
    Task<(int Count, int InvalidCount)> CountIgnoreAcl(Guid filterVersionId);
}
