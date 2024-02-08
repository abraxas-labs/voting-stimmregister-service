// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Lib.Common;
using Voting.Stimmregister.Domain.Models.EVoting;

namespace Voting.Stimmregister.Abstractions.Adapter.EVoting.Kewr;

public interface IKewrAdapter
{
    Task<EVotingPersonDataModel> GetPersonWithMainResidenceByAhvn13(Ahvn13 ahvn13, short bfsCanton);
}
