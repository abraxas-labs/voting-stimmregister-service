// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for E-Voter audit trail.
/// </summary>
public interface IEVoterAuditRepository : IDbRepository<DbContext, EVoterAuditEntity>
{
}
