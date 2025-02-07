// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Domain.Models.BfsStatistic;

namespace Voting.Stimmregister.Abstractions.Core.Import.Services;

public interface IBfsStatisticService
{
    Task<BfsStatisticModel> GetStatistics(bool disableQueryFilter = false, short? cantonBfs = null);

    Task CreateOrUpdateStatistics(PersonImportStateModel state);
}
