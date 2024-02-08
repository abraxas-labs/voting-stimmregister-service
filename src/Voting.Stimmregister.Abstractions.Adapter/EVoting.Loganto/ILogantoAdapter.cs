// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Models.EVoting;

namespace Voting.Stimmregister.Abstractions.Adapter.EVoting.Loganto;

public interface ILogantoAdapter
{
    Task<EVotingRegisterResponseModel> RegisterForEVoting(EVotingRegisterRequestModel model);

    Task<EVotingUnregisterResponseModel> UnregisterFromEVoting(EVotingUnregisterRequestModel model);
}
