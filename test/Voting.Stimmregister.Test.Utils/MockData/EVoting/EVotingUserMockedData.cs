// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData.EVoting;

public static class EVotingUserMockedData
{
    public static EVoterEntity EVoter1 => new()
    {
        Id = Guid.Parse("10000000-0000-0000-0000-000000000000"),
        Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1,
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsAllowedForEVoting,
        ContextId = "1",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };

    public static EVoterEntity EVoter2 => new()
    {
        Id = Guid.Parse("20000000-0000-0000-0000-000000000000"),
        Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid2,
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsAllowedForEVoting,
        ContextId = "2",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };

    public static EVoterEntity EVoter3 => new()
    {
        Id = Guid.Parse("30000000-0000-0000-0000-000000000000"),
        Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid3,
        BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        BfsMunicipality = EVotingBfsMunicipalityMockedData.BfsAllowedForEVoting,
        ContextId = "3",
        AuditInfo = MockedAuditInfo.Get(),
        EVoterFlag = true,
    };
}
