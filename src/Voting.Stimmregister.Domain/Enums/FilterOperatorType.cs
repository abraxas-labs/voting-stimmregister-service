// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Enums;

/// <summary>
/// The filter operator options enumeration.
/// Possible values are:
/// <list type="number">
///     <item>unspecified</item>
///     <item>contains</item>
///     <item>equals</item>
///     <item>startswith</item>
///     <item>endswith</item>
/// </list>
/// Default is 'unspecified'.
/// </summary>
public enum FilterOperatorType
{
    /// <summary>
    /// Filter type is unspecified.
    /// </summary>
    Unspecified,

    /// <summary>
    /// Filter type is 'Contains'.
    /// </summary>
    Contains,

    /// <summary>
    /// Filter type is 'Equals'.
    /// </summary>
    Equals,

    /// <summary>
    /// Filter type is 'Starts With'.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Filter operator is 'Ends With'.
    /// </summary>
    EndsWith,

    /// <summary>
    /// Filter operator is 'Less'.
    /// </summary>
    Less,

    /// <summary>
    /// Filter operator is 'Less Equal'.
    /// </summary>
    LessEqual,

    /// <summary>
    /// Filter operator is 'Greater'.
    /// </summary>
    Greater,

    /// <summary>
    /// Filter operator is 'Greater Equal'.
    /// </summary>
    GreaterEqual,
}
