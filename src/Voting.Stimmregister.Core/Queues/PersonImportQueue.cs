// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Core.Queues;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Queues;

/// <inheritdoc cref="IPersonImportQueue"/>
public class PersonImportQueue : IPersonImportQueue
{
    private readonly ImportQueue _queue;

    public PersonImportQueue(ImportQueue queue)
    {
        _queue = queue;
    }

    public Task<ImportStatisticEntity> Enqueue(ImportSourceSystem sourceSystem, string fileName, Stream content)
        => _queue.Enqueue(ImportType.Person, sourceSystem, fileName, content);
}
