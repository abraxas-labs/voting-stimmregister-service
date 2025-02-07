// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;

namespace Voting.Stimmregister.Domain.Models.BfsStatistic;

/// <summary>
/// The import statistics search parameters model.
/// </summary>
public class RegistrationStatisticResponseModel
{
    public IEnumerable<BfsStatisticEntity> MunicipalityStatistics { get; set; } = new List<BfsStatisticEntity>();

    public BfsStatisticEntity TotalStatistic { get; set; } = new();

    public bool IsTopLevelAuthority { get; set; }
}
