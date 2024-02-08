// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Enums;

/// <summary>
/// The DOI types enumeration.
/// </summary>
public enum DomainOfInfluenceType
{
    /// <summary>
    /// Domain of influence type is unspecified.
    /// </summary>
    Unspecified,

    /// <summary>
    /// Switzerland / Confederation (de: Schweiz / Bund).
    /// </summary>
    Ch,

    /// <summary>
    /// The canto (de: Kanton).
    /// </summary>
    Ct,

    /// <summary>
    /// The district (de: Bezirk).
    /// </summary>
    Bz,

    /// <summary>
    /// The municipality (de: Gemeinde).
    /// </summary>
    Mu,

    /// <summary>
    /// The city district (de: Stadtkreis).
    /// </summary>
    Sk,

    /// <summary>
    /// The school circle (de: Schulkreis).
    /// </summary>
    Sc,

    /// <summary>
    /// The church circle (de: Kirchgemeinde).
    /// </summary>
    Ki,

    /// <summary>
    /// The local community (de: Ortsbürgergemeinde).
    /// </summary>
    Og,

    /// <summary>
    /// The coorperation (de: Koprorationen).
    /// </summary>
    Ko,

    /// <summary>
    /// Other types (de: Andere).
    /// </summary>
    An,

    /// <summary>
    /// The catholic church circle (de: Kirchgemeinde (katholisch)).
    /// </summary>
    KiKat,

    /// <summary>
    /// The evangelic church circle (de: Kirchgemeinde (evangelisch)).
    /// </summary>
    KiEva,

    /// <summary>
    /// Another type (de: Verkehrskreis).
    /// </summary>
    AnVek,

    /// <summary>
    /// Another type (de: Wohnviertel).
    /// </summary>
    AnWok,

    /// <summary>
    /// Another type (de: Volkskreis).
    /// </summary>
    AnVok,
}
