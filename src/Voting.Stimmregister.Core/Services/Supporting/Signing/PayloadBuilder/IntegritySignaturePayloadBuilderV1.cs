// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

internal class IntegritySignaturePayloadBuilderV1 : IncrementalPersonPayloadBuilderV1<BfsIntegrityEntity>, IIntegritySignaturePayloadBuilder
{
    private readonly DomainOfInfluenceSignaturePayloadBuilderV1 _doiBuilder;
    private bool _disposedValue;

    public IntegritySignaturePayloadBuilderV1(
        ObjectPool<HashBuilder> hashBuilderPool,
        IHsmCryptoAdapter hsmCryptoAdapter,
        PersonSignaturePayloadBuilderV1 personPayloadBuilder,
        DomainOfInfluenceSignaturePayloadBuilderV1 doiBuilder)
        : base(nameof(BfsIntegrityEntity), hashBuilderPool, hsmCryptoAdapter, personPayloadBuilder)
    {
        _doiBuilder = doiBuilder;
    }

    public override void Init(BfsIntegrityEntity item)
    {
        HashBuilder
            .AppendDelimited(item.Bfs)
            .AppendDelimited(item.LastUpdated);
    }

    public SignaturePayload Build((BfsIntegrityEntity InitializationEntity, IEnumerable<DomainOfInfluenceEntity> ContentEntities) entity)
    {
        Init(entity.InitializationEntity);

        foreach (var item in entity.ContentEntities)
        {
            Append(item);
        }

        return BuildAndReset();
    }

    public void Append(DomainOfInfluenceEntity item)
        => HashBuilder.AppendDelimited(_doiBuilder.Build(item).Payload);

    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _doiBuilder.Dispose();
            }

            _disposedValue = true;

            base.Dispose(disposing);
        }
    }
}
