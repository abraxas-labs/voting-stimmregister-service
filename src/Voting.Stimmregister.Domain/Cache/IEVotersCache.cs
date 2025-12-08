// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Domain.Cache;

public interface IEVotersCache
{
    /// <summary>
    /// Returns a dictionary of all ahvn13 which have evoting enabled, together with their optional e-voting email,
    /// in the canton of the given municipality id.
    /// </summary>
    /// <param name="municipalityId">The municipality id.</param>
    /// <returns>A dictionary of ahvn13 and optional emails.</returns>
    Task<Dictionary<long, string?>> GetEnabledAhvN13WithEmailForCantonWithMunicipalityId(int municipalityId);
}
