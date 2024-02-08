// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Domain.Cache;

public interface IEVotersCache
{
    /// <summary>
    /// Returns a set of all ahvn13 which have evoting enabled
    /// in the canton of the given municipality id.
    /// </summary>
    /// <param name="municipalityId">The municipality id.</param>
    /// <returns>A set of ahvn13.</returns>
    Task<HashSet<long>> GetEnabledAhvN13ForCantonWithMunicipalityId(int municipalityId);
}
