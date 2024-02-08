// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Enums;

/// <summary>
/// The search intention type of a person search.
/// </summary>
public enum PersonSearchType
{
    /// <summary>
    /// No specified intention.
    /// </summary>
    Unspecified,

    /// <summary>
    /// The intention to look up people directly.
    /// </summary>
    Person,

    /// <summary>
    /// The intention to lookup people based on a filter.
    /// </summary>
    Filter,
}
