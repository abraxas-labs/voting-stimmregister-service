// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

public interface IFilterVersionRepository : IDbRepository<DbContext, FilterVersionEntity>
{
    /// <summary>
    /// Deletes outdated versions of <see cref="FilterVersionEntity"/> from the database that were created before the specified threshold date.
    /// </summary>
    /// <param name="thresholdDate">The date before which the <see cref="FilterVersionEntity"/> should be considered outdated.</param>
    /// <param name="sqlCommandTimeoutInSeconds">The sql command timeout in seconds.</param>
    /// <returns>The number of deleted filter versions.</returns>
    Task<int> DeleteOutdatedFilterVersions(DateTime thresholdDate, int sqlCommandTimeoutInSeconds);
}
