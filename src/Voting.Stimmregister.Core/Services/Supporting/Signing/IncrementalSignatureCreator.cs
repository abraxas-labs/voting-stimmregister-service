// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

internal sealed class IncrementalSignatureCreator<TEntity, TContent> : IIncrementalSignatureCreator<TContent>
    where TEntity : BaseEntityWithSignature
{
    private readonly TEntity _entity;
    private readonly SignatureCreator _signer;
    private readonly IIncrementalSignaturePayloadBuilder<TEntity, TContent> _payloadBuilder;

    private IncrementalSignatureCreator(TEntity entity, SignatureCreator signer, IIncrementalSignaturePayloadBuilder<TEntity, TContent> payloadBuilder)
    {
        _entity = entity;
        _signer = signer;
        _payloadBuilder = payloadBuilder;
    }

    public void Append(TContent entity)
        => _payloadBuilder.Append(entity);

    public void Sign()
        => _signer.Sign(_payloadBuilder.BuildAndReset(), _entity);

    internal static IIncrementalSignatureCreator<TContent> InitNew(TEntity entity, SignatureCreator signer, IIncrementalSignaturePayloadBuilder<TEntity, TContent> payloadBuilder)
    {
        payloadBuilder.Init(entity);
        return new IncrementalSignatureCreator<TEntity, TContent>(entity, signer, payloadBuilder);
    }
}
