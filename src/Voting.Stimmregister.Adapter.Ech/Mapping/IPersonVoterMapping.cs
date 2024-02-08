// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Ech0045_4_0;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Ech.Mapping;

public interface IPersonVoterMapping
{
    VotingPersonType ToEchVoter(PersonEntity person);
}
