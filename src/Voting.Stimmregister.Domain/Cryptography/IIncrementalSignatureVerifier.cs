// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Cryptography;

/// <summary>
/// An incremental signature verifier.
/// Builds up the signature data in an incremental way,
/// by appending entities (by calling <see cref="Append"/>).
/// Finally ensures the signature of the source object (for which this verifier was initialized)
/// is valid by calling <see cref="EnsureValid"/>).
/// </summary>
/// <typeparam name="T">The type of the incrementally added entity.</typeparam>
public interface IIncrementalSignatureVerifier<in T>
{
    /// <summary>
    /// Appends an entity to the signature data state.
    /// </summary>
    /// <param name="item">The entity.</param>
    void Append(T item);

    /// <summary>
    /// Ensures the signature of the underlying source entity by is valid.
    /// </summary>
    void EnsureValid();
}
