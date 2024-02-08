// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Xunit;

// add a using directive to ArchUnitNET.Fluent.ArchRuleDefinition to easily define ArchRules
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Voting.Stimmregister.Architecture.Unit.Tests;

public class PlantUmlTests
{
    private static readonly ArchUnitNET.Domain.Architecture _architecture =
        new ArchLoader().LoadAssemblies(
                typeof(Abstractions.Import.Markers.ArchMarker).Assembly,
                typeof(Abstractions.Core.Markers.ArchMarker).Assembly,
                typeof(Abstractions.Adapter.Markers.ArchMarker).Assembly,
                typeof(Adapter.Ech.Markers.ArchMarker).Assembly,
                typeof(Adapter.EVoting.Kewr.Markers.ArchMarker).Assembly,
                typeof(Adapter.EVoting.Loganto.Markers.ArchMarker).Assembly,
                typeof(Adapter.Hsm.Markers.ArchMarker).Assembly,
                typeof(Adapter.VotingBasis.Markers.ArchMarker).Assembly,
                typeof(Adapter.VotingIam.Markers.ArchMarker).Assembly,
                typeof(Core.Markers.ArchMarker).Assembly,
                typeof(Import.Innosolv.Markers.ArchMarker).Assembly,
                typeof(Import.Loganto.Markers.ArchMarker).Assembly,
                typeof(Domain.Markers.ArchMarker).Assembly,
                typeof(WebService.Markers.ArchMarker).Assembly).Build();

    [Fact]
    public void SolutionArchitectureShouldMatchPlantUml()
    {
        const string solutionArchitectureDiagram = "./solution-architecture.puml";
        var solutionArchitectureRule = Types().That().ResideInNamespace("Voting.Stimmregister.*", true).Should().AdhereToPlantUmlDiagram(solutionArchitectureDiagram);
        solutionArchitectureRule.Check(_architecture);
    }
}
