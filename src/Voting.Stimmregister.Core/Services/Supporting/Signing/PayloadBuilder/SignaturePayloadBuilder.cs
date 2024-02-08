// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.Extensions.ObjectPool;
using Voting.Lib.Common;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Signing;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

internal class SignaturePayloadBuilder : IDisposable
{
    private readonly string _typeName;
    private readonly byte _signatureVersion;
    private readonly ObjectPool<HashBuilder> _hashBuilderPool;
    private readonly AsymmetricKeyConfig _signatureConfig;
    private bool _disposedValue;

    public SignaturePayloadBuilder(
        string typeName,
        byte signatureVersion,
        SignatureConfigLabels signatureConfigLabels,
        ObjectPool<HashBuilder> hashBuilderPool)
    {
        _typeName = typeName;
        _signatureVersion = signatureVersion;
        _hashBuilderPool = hashBuilderPool;

        _signatureConfig = new()
        {
            PrivateKeyCkaLabel = signatureConfigLabels.VosrEcdsaPrivateKey,
            PublicKeyCkaLabel = signatureConfigLabels.VosrEcdsaPublicKey,
        };

        HashBuilder = _hashBuilderPool.Get();
        WriteSignatureHeader();
    }

    public HashBuilder HashBuilder { get; }

    public SignaturePayload GetPayloadAndReset()
    {
        var payload = new SignaturePayload(_signatureConfig, _typeName, _signatureVersion, HashBuilder.GetHashAndReset());

        // write header for next payload
        WriteSignatureHeader();

        return payload;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _hashBuilderPool.Return(HashBuilder);
            }

            _disposedValue = true;
        }
    }

    private void WriteSignatureHeader()
    {
        HashBuilder
            .AppendDelimited(_signatureVersion)
            .AppendDelimited(_signatureConfig.SignatureKeyId);
    }
}
