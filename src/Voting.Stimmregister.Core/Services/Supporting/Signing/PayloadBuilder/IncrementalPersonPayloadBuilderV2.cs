// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

public abstract class IncrementalPersonPayloadBuilderV2<T> : IDisposable, IIncrementalSignaturePayloadBuilder<T, PersonEntity>
{
    private readonly PersonSignaturePayloadBuilderV2 _personPayloadBuilder;
    private readonly SignaturePayloadBuilder _builder;
    private PersonEntity? _previousPerson;
    private bool _disposedValue;

    protected IncrementalPersonPayloadBuilderV2(
        string typeName,
        ObjectPool<HashBuilder> hashBuilderPool,
        IHsmCryptoAdapter hsmCryptoAdapter,
        PersonSignaturePayloadBuilderV2 personPayloadBuilder)
    {
        _personPayloadBuilder = personPayloadBuilder;
        _builder = new SignaturePayloadBuilder(typeName, Version, hsmCryptoAdapter.GetSignatureConfigLabels(), hashBuilderPool);
    }

    public byte Version => 2;

    protected HashBuilder HashBuilder => _builder.HashBuilder;

    public abstract void Init(T item);

    public void Append(PersonEntity item)
    {
        if (_previousPerson != null && !IsOrderValid(_previousPerson, item))
        {
            throw new InvalidOperationException("Persons must be sorted by id, a subsequent person needs to have a bigger id than the previous person");
        }

        HashBuilder.AppendDelimited(_personPayloadBuilder.Build(item).Payload);
        _previousPerson = item;
    }

    public SignaturePayload Build((T InitializationEntity, IEnumerable<PersonEntity> ContentEntities) payload)
    {
        Init(payload.InitializationEntity);

        foreach (var item in payload.ContentEntities)
        {
            Append(item);
        }

        return BuildAndReset();
    }

    public SignaturePayload BuildAndReset()
        => _builder.GetPayloadAndReset();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual bool IsOrderValid(PersonEntity previousEntity, PersonEntity currentEntity)
    {
        // current entity id must be bigger than the previous
        return currentEntity.Id.CompareTo(previousEntity.Id) == 1;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _builder.Dispose();
                _personPayloadBuilder.Dispose();
            }

            _disposedValue = true;
        }
    }
}
