// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Common;

namespace Voting.Stimmregister.Domain.Models.EVoting;

public class EVotingUnregisterRequestModel
{
    public EVotingUnregisterRequestModel(Ahvn13 ahvn13)
    {
        Ahvn13 = ahvn13;
    }

    public Ahvn13 Ahvn13 { get; set; }

    public int MunicipalityId { get; set; }

    public DateTime UnregisterOn { get; set; }
}
