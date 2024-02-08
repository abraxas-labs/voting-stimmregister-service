// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

internal sealed class IncrementalSignatureVerifier<TEntity, TContent> : IIncrementalSignatureVerifier<TContent>
    where TEntity : BaseEntityWithSignature
{
    private readonly TEntity _signedEntity;
    private readonly IIncrementalSignaturePayloadBuilder<TEntity, TContent> _payloadBuilder;
    private readonly SignatureVerifier _signatureVerifier;

    private IncrementalSignatureVerifier(
        TEntity signedEntity,
        IIncrementalSignaturePayloadBuilder<TEntity, TContent> payloadBuilder,
        SignatureVerifier signatureVerifier)
    {
        _signedEntity = signedEntity;
        _payloadBuilder = payloadBuilder;
        _signatureVerifier = signatureVerifier;
    }

    public void Append(TContent item)
        => _payloadBuilder.Append(item);

    public void EnsureValid()
        => _signatureVerifier.EnsureValidSignature(_signedEntity, _payloadBuilder.BuildAndReset());

    internal static IIncrementalSignatureVerifier<TContent> InitNew(
        TEntity integrity,
        SignatureVerifier signatureVerifier,
        IIncrementalSignaturePayloadBuilder<TEntity, TContent> payloadBuilder)
    {
        payloadBuilder.Init(integrity);
        return new IncrementalSignatureVerifier<TEntity, TContent>(integrity, payloadBuilder, signatureVerifier);
    }
}
