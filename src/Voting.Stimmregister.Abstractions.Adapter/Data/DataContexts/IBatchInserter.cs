// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;

/// <summary>
/// Batch inserter to insert batches of data into the db context.
/// </summary>
public interface IBatchInserter
{
    /// <summary>
    /// Inserts entities in batches into the <see cref="IDataContext"/>.
    /// </summary>
    /// <param name="entities">All entities to be inserted.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <typeparam name="T">The type of the entities.</typeparam>
    /// <returns>A task.</returns>
    Task InsertBatched<T>(IEnumerable<T> entities, CancellationToken ct)
        where T : class;
}
