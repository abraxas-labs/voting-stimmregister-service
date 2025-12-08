// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Enums;

/// <summary>
/// The source system of an import enumeration.
/// <list type="bullet">
///     <item>0: Unspecified</item>
///     <item>1: Loganto</item>
///     <item>2: Cobra (SG)</item>
///     <item>3: VotingBasis</item>
///     <item>4: Innosolv</item>
///     <item>5: Cobra (TG)</item>
/// </list>
/// </summary>
public enum ImportSourceSystem
{
    Unspecified,
    Loganto,
    Cobra,
    VotingBasis,
    Innosolv,
    CobraTg,
}
