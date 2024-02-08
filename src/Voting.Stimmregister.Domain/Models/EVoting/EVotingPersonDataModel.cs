// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Common;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models.EVoting;

public class EVotingPersonDataModel
{
    public EVotingPersonDataModel(Ahvn13 ahvn13)
    {
        Ahvn13 = ahvn13;
    }

    public Ahvn13 Ahvn13 { get; set; }

    public bool AllowedToVote { get; set; }

    public short BfsMunicipality { get; set; }

    public int OeidMunicipality { get; set; }

    public string? Nationality { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public SexType Sex { get; set; }

    public string OfficialName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public EVotingAddressModel? Address { get; set; }
}
