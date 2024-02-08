// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// The person search parameters model for a single person search.
/// </summary>
public class PersonSearchSingleParametersModel
{
    /// <summary>
    /// Gets or sets the person's register id to search for.
    /// </summary>
    public Guid RegisterId { get; set; }
}
