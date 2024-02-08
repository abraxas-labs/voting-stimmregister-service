// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Enums;

/// <summary>
/// The eCH-0044:sexType enumeration.
/// <list type="bullet">
///     <item>1: male (M)</item>
///     <item>2: female (W)</item>
///     <item>3: undefined (?)</item>
/// </list>
/// </summary>
public enum SexType
{
    /// <summary>
    /// 1: Male.
    /// </summary>
    Male = 1,

    /// <summary>
    /// 2: Female.
    /// </summary>
    Female = 2,

    /// <summary>
    /// 3: Undefined.
    /// </summary>
    Undefined = 3,
}
