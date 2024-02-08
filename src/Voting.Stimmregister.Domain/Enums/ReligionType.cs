// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Enums;

/// <summary>
/// The eCH-0011:religionType enumeration.
/// <list type="bullet">
///     <item>000: unknown</item>
///     <item>111: evangelic</item>
///     <item>121: catholic</item>
///     <item>122: christ-catholic / old-catholic (valid for cantons ZH, BE, LU, SO,BS, BL, SH, SG, AG, NE, GE)</item>
/// </list>
/// </summary>
public enum ReligionType
{
    /// <summary>
    /// 000: unknown.
    /// </summary>
    Unknown = 000,

    /// <summary>
    /// 111: evangelic.
    /// </summary>
    Evangelic = 111,

    /// <summary>
    /// 121: catholic.
    /// </summary>
    Catholic = 121,

    /// <summary>
    /// 122: christ-catholic / old-catholic (valid for cantons ZH, BE, LU, SO,BS, BL, SH, SG, AG, NE, GE).
    /// </summary>
    ChristCatholic = 122,
}
