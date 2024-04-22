// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Models.RegistrationStatistic;

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// Service for import statistics.
/// </summary>
public interface IRegistrationStatisticService
{
    /// <summary>
    /// Gets a list of registration statistics according to passed search parameters.
    /// </summary>
    /// <returns>A <see cref="RegistrationStatisticResponseModel"/> containing a list of resolved bfs statistics.</returns>
    Task<RegistrationStatisticResponseModel> List();
}
