// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Constants.EVoting;

namespace Voting.Stimmregister.Domain.Models.EVoting;

public class EVotingInformationModel
{
    public VotingStatus Status { get; set; }

    public EVotingPersonDataModel? Person { get; set; }

    public BfsStatisticEntity CantonStatistic { get; set; } = new();

    public BfsStatisticEntity MunicipalityStatistic { get; set; } = new();
}
