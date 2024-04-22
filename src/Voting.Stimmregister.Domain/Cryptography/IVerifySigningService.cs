// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Stimmregister.Domain.Models;

// TODO: VOTING-2763 Move to Core Signing, after or with Adapter.Loganto is moved to Core.
namespace Voting.Stimmregister.Domain.Cryptography;

/// <summary>
/// Services for signature verification of provided business entities.
/// </summary>
public interface IVerifySigningService
{
    /// <summary>
    /// Ensures that the signature of the <see cref="BfsIntegrityEntity"/> is valid.
    /// </summary>
    /// <param name="integrity">The integrity entity.</param>
    /// <param name="persons">The persons.</param>
    /// <exception cref="SignatureInvalidException">If an invalid signature is detected.</exception>
    void EnsureBfsIntegritySignatureValid(BfsIntegrityEntity integrity, IReadOnlyCollection<PersonEntity> persons);

    /// <summary>
    /// Ensures the signature of a filter version is valid.
    /// </summary>
    /// <param name="filterVersion">The filter version.</param>
    /// <param name="persons">The persons of the filter version.</param>
    void EnsureFilterVersionSignatureValid(FilterVersionEntity filterVersion, IReadOnlyCollection<PersonEntity> persons);

    /// <summary>
    /// Creates an incremental bfs integrity signature verifier.
    /// </summary>
    /// <param name="integrity">The integrity.</param>
    /// <returns>The verifier.</returns>
    IIncrementalSignatureVerifier<PersonEntity> CreateBfsIntegritySignatureVerifier(BfsIntegrityEntity integrity);

    /// <summary>
    /// Creates an incremental filter version signature verifier.
    /// </summary>
    /// <param name="filterVersion">The filter version.</param>
    /// <returns>The verifier.</returns>
    IIncrementalSignatureVerifier<PersonEntity> CreateFilterVersionSignatureVerifier(FilterVersionEntity filterVersion);
}
