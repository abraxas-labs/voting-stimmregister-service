// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Lib.Common;
using Voting.Stimmregister.Domain.Models.EVoting;

namespace Voting.Stimmregister.Abstractions.Core.Services;

public interface IEVotingService
{
    public Task<EVotingInformationModel> GetEVotingInformation(Ahvn13 ahvn13, short bfsCanton);

    public Task<EVotingReportModel?> GetEVotingReport(Ahvn13 ahvn13);

    public Task RegisterForEVoting(Ahvn13 ahvn13, short bfsCanton);

    public Task UnregisterFromEVoting(Ahvn13 ahvn13, short bfsCanton);
}
