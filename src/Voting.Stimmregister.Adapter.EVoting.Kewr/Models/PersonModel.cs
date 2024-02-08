// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.Models;

public class PersonModel
{
    public int ResidentNr { get; set; }

    public string? Ahvn13 { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person is allowed to vote. If this is false, the person can vote.
    /// </summary>
    public bool NonVoting { get; set; }

    /// <summary>
    /// Gets or sets the municipality BFS.
    /// </summary>
    public short LocalCommunityBfs { get; set; }

    public string? Nationality { get; set; }

    public DateOfBirthModel DateOfBirth { get; set; } = new();

    public GenderModel Gender { get; set; } = new();

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string OfficialName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public AddressModel[] LivingAddress { get; set; } = Array.Empty<AddressModel>();
}
