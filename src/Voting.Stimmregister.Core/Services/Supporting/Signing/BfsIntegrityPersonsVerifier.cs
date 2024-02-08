// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

public class BfsIntegrityPersonsVerifier
{
    private readonly IPersonRepository _personRepository;
    private readonly IVerifySigningService _verifySigningService;
    private readonly IBfsIntegrityRepository _bfsIntegrityRepository;

    public BfsIntegrityPersonsVerifier(IPersonRepository personRepository, IVerifySigningService verifySigningService, IBfsIntegrityRepository bfsIntegrityRepository)
    {
        _personRepository = personRepository;
        _verifySigningService = verifySigningService;
        _bfsIntegrityRepository = bfsIntegrityRepository;
    }

    /// <summary>
    /// Verifies the integrity of all persons in the provided municipality ids.
    /// </summary>
    /// <param name="municipalityIds">The municipality ids.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <exception cref="EntityNotFoundException">If no integrity entity was found for a given bfs.</exception>
    /// <returns>A Task representing the async operation.</returns>
    public Task Verify(
        IReadOnlyCollection<int> municipalityIds,
        CancellationToken ct)
    {
        return Verify(municipalityIds, static _ => { }, ct);
    }

    /// <summary>
    /// Verifies the integrity of all persons in the provided municipality ids.
    /// </summary>
    /// <param name="municipalityIds">The municipality ids.</param>
    /// <param name="afterPersonAppended">A callback to perform additional logic after a person is processed (added to the signature verification).</param>
    /// <param name="ct">The cancellation token.</param>
    /// <exception cref="EntityNotFoundException">If no integrity entity was found for a given bfs.</exception>
    /// <returns>A Task representing the async operation.</returns>
    public async Task Verify(
        IReadOnlyCollection<int> municipalityIds,
        Action<PersonEntity> afterPersonAppended,
        CancellationToken ct)
    {
        var bfs = municipalityIds.Select(x => x.ToString()).ToHashSet();
        var bfsIntegrityByBfs = await _bfsIntegrityRepository.ListForBfs(ImportType.Person, bfs, ct);
        foreach (var municipalityId in municipalityIds)
        {
            if (!bfsIntegrityByBfs.TryGetValue(municipalityId.ToString(), out var bfsIntegrity))
            {
                throw new EntityNotFoundException(nameof(BfsIntegrityEntity), new { ImportType.Person, bfs });
            }

            await Verify(municipalityId, bfsIntegrity, afterPersonAppended, ct);
            ct.ThrowIfCancellationRequested();
        }
    }

    private async Task Verify(
        int bfs,
        BfsIntegrityEntity bfsIntegrity,
        Action<PersonEntity> afterPersonAppended,
        CancellationToken ct)
    {
        var bfsPersons = _personRepository.StreamLatestByBfsIgnoreAclAndDeleted(bfs);
        var signatureVerifier = _verifySigningService.CreateBfsIntegritySignatureVerifier(bfsIntegrity);
        await foreach (var person in bfsPersons.WithCancellation(ct))
        {
            signatureVerifier.Append(person);
            afterPersonAppended(person);
        }

        signatureVerifier.EnsureValid();
    }
}
