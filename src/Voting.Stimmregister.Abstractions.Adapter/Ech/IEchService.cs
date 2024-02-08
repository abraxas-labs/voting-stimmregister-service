// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Ech;

/// <summary>
/// eCH export services.
/// </summary>
public interface IEchService
{
    /// <summary>
    /// Converts a set of persons into an eCH-0045.
    /// </summary>
    /// <param name="writer">The target writer.</param>
    /// <param name="numberOfPersons">The number of persons.</param>
    /// <param name="voters">The persons.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task WriteEch0045(
        PipeWriter writer,
        int numberOfPersons,
        IAsyncEnumerable<PersonEntity> voters,
        CancellationToken ct);
}
