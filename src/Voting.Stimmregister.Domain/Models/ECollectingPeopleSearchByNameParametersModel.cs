// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Database.Models;

namespace Voting.Stimmregister.Domain.Models;

public class ECollectingPeopleSearchByNameParametersModel
{
    public int MunicipalityId { get; set; }

    public string? OfficialName { get; set; }

    public string? FirstName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? AddressStreet { get; set; }

    public string? AddressHouseNumber { get; set; }

    public DateOnly ActualityDate { get; set; }

    public Pageable? Pageable { get; set; }
}
