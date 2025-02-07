// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// Service for database cleanup of unused/unneeded data.
/// </summary>
public interface IDatabaseCleanupService
{
    /// <summary>
    /// Runs the cleanup process.
    /// </summary>
    /// <returns>Cleanup result information.</returns>
    Task<DatabaseCleanupResultModel> RunCleanup();
}
