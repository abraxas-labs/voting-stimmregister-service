// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Constants.EVoting;

namespace Voting.Stimmregister.WebService.Models.EVoting.Response;

public class GetRegistrationInformationResponse : ProcessStatusResponseBase
{
    public VotingStatus VotingStatus { get; init; }

    public GetRegistrationInformationPerson? Person { get; set; }

    public GetRegistrationInformationBfsStatistic CantonStatistic { get; set; } = new();

    public GetRegistrationInformationBfsStatistic MunicipalityStatistic { get; set; } = new();
}
