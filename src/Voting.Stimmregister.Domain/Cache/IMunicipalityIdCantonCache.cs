// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Cache;

/// <summary>
/// A cache to get the canton of a municipality.
/// </summary>
public interface IMunicipalityIdCantonCache
{
    /// <summary>
    /// Gets the canton resolved for a municipality id.
    /// </summary>
    /// <param name="municipalityId">The bfs of the municipality.</param>
    /// <returns>The canton.</returns>
    Task<Canton> GetCantonByMunicipalityId(int municipalityId);
}
