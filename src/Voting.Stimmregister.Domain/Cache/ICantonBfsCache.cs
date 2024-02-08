// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Cache;

public interface ICantonBfsCache
{
    /// <summary>
    /// Gets the bfs of a canton.
    /// </summary>
    /// <param name="canton">The canton.</param>
    /// <returns>The bfs of the canton or null if it couldn't be resolved.</returns>
    Task<string?> GetBfsOfCanton(Canton canton);
}
