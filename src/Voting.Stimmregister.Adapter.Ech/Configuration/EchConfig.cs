// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Reflection;
using Ech0007_6_0;
using Voting.Lib.Ech.Ech0045_4_0.Models;

namespace Voting.Stimmregister.Adapter.Ech.Configuration;

/// <summary>
/// Configurations related to eCH.
/// </summary>
public class EchConfig : Lib.Ech.Configuration.EchConfig
{
    public EchConfig(Assembly assembly)
        : base(assembly)
    {
    }

    /// <summary>
    /// Gets or sets the register identification of eCH exports.
    /// </summary>
    public CantonAbbreviationType RegisterIdentification { get; set; }

    /// <summary>
    /// Gets or sets the name of the register of eCH exports.
    /// </summary>
    public string RegisterName { get; set; } = "Stehendes Stimmregister";

    /// <summary>
    /// Gets or sets the voting places by each bfs.
    /// </summary>
    public Dictionary<string, SwissAbroadPersonExtensionVotingPlace> VotingPlacesByBfs { get; set; } = new();
}
