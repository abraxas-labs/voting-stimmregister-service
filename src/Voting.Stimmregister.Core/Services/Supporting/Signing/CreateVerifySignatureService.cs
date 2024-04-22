// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Stimmregister.Core.Diagnostics;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

/// <inheritdoc cref="ICreateSignatureService" see="IVerifySigningService"/>
public class CreateVerifySignatureService : ICreateSignatureService, IVerifySigningService
{
    private readonly SignaturePayloadBuilderFactory _signaturePayloadBuilderFactory;
    private readonly SignatureVerifier _signatureVerifier;
    private readonly SignatureCreator _signatureCreator;
    private readonly ActivityFactory<CreateVerifySignatureService> _activityFactory;

    public CreateVerifySignatureService(
        SignaturePayloadBuilderFactory signaturePayloadBuilderFactory,
        SignatureVerifier signatureVerifier,
        SignatureCreator signatureCreator,
        ActivityFactory<CreateVerifySignatureService> activityFactory)
    {
        _signaturePayloadBuilderFactory = signaturePayloadBuilderFactory;
        _signatureVerifier = signatureVerifier;
        _signatureCreator = signatureCreator;
        _activityFactory = activityFactory;
    }

    public void SignIntegrity(BfsIntegrityEntity integrity, IReadOnlyCollection<DomainOfInfluenceEntity> dois)
    {
        using var activity = _activityFactory.Start("sign-integrity-dois");
        var payload = _signaturePayloadBuilderFactory.Get(integrity).Build((integrity, dois));
        _signatureCreator.Sign(payload, integrity);
    }

    public void SignIntegrity(BfsIntegrityEntity integrity, IReadOnlyCollection<PersonEntity> persons)
    {
        using var activity = _activityFactory.Start("sign-integrity-persons");
        var payload = _signaturePayloadBuilderFactory.Get(integrity).Build((integrity, persons));
        _signatureCreator.Sign(payload, integrity);
    }

    public void SignFilterVersion(FilterVersionEntity filterVersion, IReadOnlyCollection<PersonEntity> persons)
    {
        using var activity = _activityFactory.Start("sign-filter-version");
        var payload = _signaturePayloadBuilderFactory.Get(filterVersion).Build((filterVersion, persons));
        _signatureCreator.Sign(payload, filterVersion);
    }

    public void EnsureBfsIntegritySignatureValid(BfsIntegrityEntity integrity, IReadOnlyCollection<PersonEntity> persons)
    {
        using var activity = _activityFactory.Start("assert-bfs-integrity-signature");
        var payload = _signaturePayloadBuilderFactory.Get(integrity).Build((integrity, persons));
        _signatureVerifier.EnsureValidSignature(integrity, payload);
    }

    public void EnsureFilterVersionSignatureValid(FilterVersionEntity filterVersion, IReadOnlyCollection<PersonEntity> persons)
    {
        using var activity = _activityFactory.Start("assert-filter-version-signature");
        var payload = _signaturePayloadBuilderFactory.Get(filterVersion).Build((filterVersion, persons));
        _signatureVerifier.EnsureValidSignature(filterVersion, payload);
    }

    public IIncrementalSignatureCreator<PersonEntity> CreateFilterVersionSignatureCreator(FilterVersionEntity filterVersion)
    {
        var payloadBuilder = _signaturePayloadBuilderFactory.Get(filterVersion);
        return IncrementalSignatureCreator<FilterVersionEntity, PersonEntity>.InitNew(filterVersion, _signatureCreator, payloadBuilder);
    }

    public IIncrementalSignatureVerifier<PersonEntity> CreateBfsIntegritySignatureVerifier(BfsIntegrityEntity integrity)
    {
        var payloadBuilder = _signaturePayloadBuilderFactory.Get(integrity);
        return IncrementalSignatureVerifier<BfsIntegrityEntity, PersonEntity>.InitNew(integrity, _signatureVerifier, payloadBuilder);
    }

    public IIncrementalSignatureVerifier<PersonEntity> CreateFilterVersionSignatureVerifier(FilterVersionEntity filterVersion)
    {
        var payloadBuilder = _signaturePayloadBuilderFactory.Get(filterVersion);
        return IncrementalSignatureVerifier<FilterVersionEntity, PersonEntity>.InitNew(filterVersion, _signatureVerifier, payloadBuilder);
    }
}
