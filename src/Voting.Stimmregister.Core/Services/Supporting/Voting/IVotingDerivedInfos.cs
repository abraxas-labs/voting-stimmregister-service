// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Voting;

public interface IVotingDerivedInfos
{
    void ComputeInfos(IReadOnlyCollection<PersonEntityModel> persons, DateOnly referenceKeyDate);

    void ComputeInfos(PersonEntityModel person, DateOnly referenceKeyDate);
}
