// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Enums;

/// <summary>
/// The filter type options enumeration.
/// Possible values are:
/// <list type="number">
///     <item>unspecified</item>
///     <item>string</item>
///     <item>date</item>
///     <item>boolean</item>
///     <item>numeric</item>
///     <item>select</item>
///     <item>multiselect</item>
/// </list>
/// Default is 'unspecified'.
/// </summary>
public enum FilterDataType
{
    /// <summary>
    /// Filter type is unspecified.
    /// </summary>
    Unspecified,

    /// <summary>
    /// Filter type is string.
    /// </summary>
    String,

    /// <summary>
    /// Filter type is date in format 'yyyy-mm-dd'.
    /// </summary>
    Date,

    /// <summary>
    /// Filter type is boolean.
    /// </summary>
    Boolean,

    /// <summary>
    /// Filter type is numeric.
    /// </summary>
    Numeric,

    /// <summary>
    /// Filter type is select.
    /// </summary>
    Select,

    /// <summary>
    /// Filter type is multiselect.
    /// </summary>
    Multiselect,
}
