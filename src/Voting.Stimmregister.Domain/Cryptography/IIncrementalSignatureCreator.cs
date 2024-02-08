// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Cryptography;

/// <summary>
/// An incremental signature creator.
/// Builds up the signature data in an incremental way,
/// by appending entities (by calling <see cref="Append"/>).
/// Finally signs the source object (for which this creator was initialized)
/// by calling <see cref="Sign"/>).
/// </summary>
/// <typeparam name="TEntity">The type of the incrementally added entity.</typeparam>
public interface IIncrementalSignatureCreator<in TEntity>
{
    /// <summary>
    /// Appends an entity to the signature data state.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Append(TEntity entity);

    /// <summary>
    /// Signs the underlying source entity by calculating the signature,
    /// and storing it and its parameters on the entity which initialized this <see cref="IIncrementalSignatureCreator{TEntity}"/> instance.
    /// </summary>
    void Sign();
}
