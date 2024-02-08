// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// The extended person entity.
/// </summary>
public class PersonEntityModel : PersonEntity
{
    /// <summary>
    /// Gets or sets a value indicating whether the person has the right to vote based on age.
    /// </summary>
    public bool IsBirthDateValidForVotingRights { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person has the right to vote based on the nationality.
    /// </summary>
    public bool IsNationalityValidForVotingRights { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether voting is allowed.
    /// </summary>
    public bool IsVotingAllowed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dataset's actuality is given or not.
    /// </summary>
    public bool Actuality { get; set; }

    /// <summary>
    /// Gets or sets the actuality date.
    /// </summary>
    public DateTime? ActualityDate { get; set; }
}
