// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Services;

/// <summary>
/// Service for import statistics.
/// </summary>
public interface IImportStatisticService
{
    /// <summary>
    /// Gets a list of import statistics according to passed search parameters.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <returns>A <see cref="ImportStatisticEntity"/> containing a list of resolved import statistics.</returns>
    Task<Page<ImportStatisticEntity>> List(ImportStatisticSearchParametersModel searchParameters);

    /// <summary>
    /// Gets a history list of import statistics for a specific import statistic.
    /// </summary>
    /// <param name="searchParameters">The search parameters.</param>
    /// <returns>A <see cref="ImportStatisticEntity"/> containing a list of resolved import statistics.</returns>
    Task<Page<ImportStatisticEntity>> GetHistory(ImportStatisticSearchParametersModel searchParameters);
}
