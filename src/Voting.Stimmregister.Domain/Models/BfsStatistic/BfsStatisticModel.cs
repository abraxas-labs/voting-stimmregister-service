// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;

namespace Voting.Stimmregister.Domain.Models.BfsStatistic;

public class BfsStatisticModel
{
    public IEnumerable<BfsStatisticEntity> MunicipalityStatistics { get; set; } = new List<BfsStatisticEntity>();

    public BfsStatisticEntity TotalStatistic { get; set; } = new();
}
