// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

public class ECollectingPeopleSearchByIdsParametersModel
{
    public int MunicipalityId { get; set; }

    public Guid[] RegisterIds { get; set; } = [];

    public DateOnly ActualityDate { get; set; }
}
