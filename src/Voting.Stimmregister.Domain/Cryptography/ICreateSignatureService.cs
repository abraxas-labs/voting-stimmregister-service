// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Stimmregister.Domain.Models;

// TODO: VOTING-2763 Move to Core Signing, after or with Adapter.Loganto is moved to Core.
namespace Voting.Stimmregister.Domain.Cryptography;

/// <summary>
/// Services for signing provided business entities.
/// </summary>
public interface ICreateSignatureService
{
    /// <summary>
    /// Bulk signing provided person entities.
    /// </summary>
    /// <param name="entities">The person entities to sign.</param>
    void BulkSignPersons(IReadOnlyCollection<PersonEntity> entities);

    /// <summary>
    /// Bulk signing provided domain of influence entities.
    /// </summary>
    /// <param name="domainOfInfluenceEntities">The domain of influence entities to sign.</param>
    void BulkSignDomainOfInfluences(IReadOnlyCollection<DomainOfInfluenceEntity> domainOfInfluenceEntities);

    /// <summary>
    /// Signing provided integrity entity for domain of influences.
    /// </summary>
    /// <param name="integrity">The integrity entity to sign.</param>
    /// <param name="dois">The domain of influences.</param>
    void SignIntegrity(BfsIntegrityEntity integrity, IReadOnlyCollection<DomainOfInfluenceEntity> dois);

    /// <summary>
    /// Signing provided integrity entity for persons.
    /// </summary>
    /// <param name="integrity">The integrity entity to sign.</param>
    /// <param name="persons">The persons to be taken into account including the DOIs.</param>
    void SignIntegrity(BfsIntegrityEntity integrity, IReadOnlyCollection<PersonEntity> persons);

    /// <summary>
    /// Signs a filter version.
    /// </summary>
    /// <param name="filterVersion">The filter version.</param>
    /// <param name="persons">The persons.</param>
    void SignFilterVersion(FilterVersionEntity filterVersion, IReadOnlyCollection<PersonEntity> persons);

    /// <summary>
    /// Creates an incremental signature creator for a filter version.
    /// </summary>
    /// <param name="filterVersion">The filter version.</param>
    /// <returns>The incremental creator.</returns>
    IIncrementalSignatureCreator<PersonEntity> CreateFilterVersionSignatureCreator(FilterVersionEntity filterVersion);
}
