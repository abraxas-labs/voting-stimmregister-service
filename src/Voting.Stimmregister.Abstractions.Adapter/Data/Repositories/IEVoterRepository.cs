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
    /// Returns a dictionary of all ahvn13 which have evoting enabled, paired with the optional e-voting email.
    /// </summary>
    /// <param name="cantonBfs">The bfs number of the canton.</param>
    /// <returns>A dictionary of ahvn13 paired with the email address.</returns>
    Task<Dictionary<long, string?>> GetEnabledAhvN13WithEmail(short cantonBfs);
}
