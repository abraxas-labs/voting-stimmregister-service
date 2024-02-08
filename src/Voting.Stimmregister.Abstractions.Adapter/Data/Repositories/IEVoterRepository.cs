// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for E-Voter.
/// </summary>
public interface IEVoterRepository : IDbRepository<DbContext, EVoterEntity>
{
    /// <summary>
    /// Returns a set of all ahvn13 which have evoting enabled.
    /// </summary>
    /// <param name="cantonBfs">The bfs number of the canton.</param>
    /// <returns>A set of ahvn13.</returns>
    Task<HashSet<long>> GetEnabledAhvN13(short cantonBfs);
}
