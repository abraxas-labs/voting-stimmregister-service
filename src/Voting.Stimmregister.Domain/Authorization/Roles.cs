// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Authorization;

/// <summary>
/// Roles available within the voting stimmregister domain.
/// </summary>
public static class Roles
{
    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>read data</item>
    ///     <item>filter data</item>
    ///     <item>create snapshots</item>
    /// </list>
    /// </summary>
    public const string Manager = "Manager";

    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>read data</item>
    ///     <item>filter data</item>
    ///     <item>create snapshots</item>
    /// </list>
    /// </summary>
    public const string Reader = "Reader";

    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>import data via import APIs targeted from import source systems</item>
    /// </list>
    /// Only service users for automated imports should own this role.
    /// </summary>
    public const string ApiImporter = "ApiImporter";

    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>import data via import API targeted from UI</item>
    /// </list>
    /// </summary>
    public const string ManualImporter = "ManualImporter";

    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>view import statistic data</item>
    /// </list>
    /// </summary>
    public const string ImportObserver = "ImportObserver";

    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>register for E-Voting</item>
    ///     <item>unregister from E-Voting</item>
    ///     <item>read E-Voting status</item>
    /// </list>
    /// </summary>
    public const string EVoting = "EVoting";

    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>manually export filter data via web client</item>
    ///     <item>manually export filter version data via web client</item>
    /// </list>
    /// </summary>
    public const string ManualExporter = "Exporter";

    /// <summary>
    /// Is allowed to:
    /// <list type="bullet">
    ///     <item>export filter version data via api</item>
    /// </list>
    /// </summary>
    public const string ApiExporter = "ApiExporter";
}
