// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.WebService.Models.EVoting.Response;

public class GetRegistrationInformationPerson
{
    public long Ahvn13 { get; set; }

    public bool AllowedToVote { get; set; }

    public short MunicipalityBfs { get; set; }

    public string? Nationality { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public SexType Sex { get; set; }

    public string OfficialName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public GetRegistrationInformationAddress? Address { get; set; }
}
