// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Core.Abstractions;

/// <summary>
/// Service for csv conversion.
/// </summary>
public interface ICsvService
{
    /// <summary>
    /// Writes a records collection to the passed <see cref="PipeWriter"/> as csv.
    /// </summary>
    /// <typeparam name="TRow">The type of the records for a single row.</typeparam>
    /// <param name="writer">The pipe writer.</param>
    /// <param name="records">The records collection.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the async operation.</returns>
    Task Write<TRow>(PipeWriter writer, IAsyncEnumerable<TRow> records, CancellationToken ct = default);
}
