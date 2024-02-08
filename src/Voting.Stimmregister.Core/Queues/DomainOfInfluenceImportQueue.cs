// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Core.Queues;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Queues;

/// <inheritdoc cref="IDomainOfInfluenceImportQueue"/>
public class DomainOfInfluenceImportQueue : IDomainOfInfluenceImportQueue
{
    private readonly ImportQueue _queue;

    public DomainOfInfluenceImportQueue(ImportQueue queue)
    {
        _queue = queue;
    }

    public Task<ImportStatisticEntity> Enqueue(ImportSourceSystem sourceSystem, string fileName, Stream content)
        => _queue.Enqueue(ImportType.DomainOfInfluence, sourceSystem, fileName, content);
}
