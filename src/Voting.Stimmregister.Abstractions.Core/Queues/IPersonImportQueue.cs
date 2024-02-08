// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Queues;

/// <summary>
/// Task queue for person imports.
/// </summary>
public interface IPersonImportQueue
{
    /// <summary>
    /// Enqueues a person file import.
    /// </summary>
    /// <param name="sourceSystem">The source system which provides the data.</param>
    /// <param name="fileName">The name of the file to import.</param>
    /// <param name="content">The contents of the import.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation resolving to the created import job.</returns>
    Task<ImportStatisticEntity> Enqueue(ImportSourceSystem sourceSystem, string fileName, Stream content);
}
