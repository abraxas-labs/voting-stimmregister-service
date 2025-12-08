// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.Lib.Cryptography;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Adapter.Hsm.Configuration;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Signing;

namespace Voting.Stimmregister.Adapter.Hsm.Services;

/// <inheritdoc cref="IHsmCryptoAdapter"/>
public class HsmCryptoAdapter : IHsmCryptoAdapter
{
    private readonly ICryptoProvider _pkcs11DeviceAdapter;
    private readonly HsmConfig _hsmConfig;

    public HsmCryptoAdapter(ICryptoProvider pkcs11DeviceAdapter, HsmConfig hsmConfig)
    {
        _pkcs11DeviceAdapter = pkcs11DeviceAdapter;
        _hsmConfig = hsmConfig;
    }

    /// <inheritdoc />
    public SignatureConfigLabels GetSignatureConfigLabels()
        => new(_hsmConfig.VosrEcdsaPublicKey, _hsmConfig.VosrEcdsaPrivateKey, _hsmConfig.VosrAesKey);

    /// <inheritdoc />
    public Task<bool> VerifyEcdsaSha384Signature(byte[] data, byte[] signature, AsymmetricKeyConfig signatureConfig)
        => _pkcs11DeviceAdapter.VerifyEcdsaSha384Signature(data, signature, signatureConfig.PublicKeyCkaLabel);

    /// <inheritdoc />
    public Task<byte[]> CreateEcdsaSha384Signature(byte[] data, AsymmetricKeyConfig signatureConfig)
    {
        return _pkcs11DeviceAdapter.CreateEcdsaSha384Signature(data, signatureConfig.PrivateKeyCkaLabel);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<byte[]>> BulkCreateEcdsaSha384Signature(ICollection<byte[]> bulkData, AsymmetricKeyConfig signatureConfig)
    {
        if (bulkData.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bulkData), "Contains no elements to sign.");
        }

        return _pkcs11DeviceAdapter.BulkCreateEcdsaSha384Signature(bulkData, signatureConfig.PrivateKeyCkaLabel);
    }

    /// <inheritdoc />
    public Task<byte[]> EncryptAes(byte[] data, SymmetricKeyConfig symmetricKeyConfig)
    {
        if (data.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Contains nothing to encrypt.");
        }

        return _pkcs11DeviceAdapter.EncryptAesGcm(data, symmetricKeyConfig.CkaLabel);
    }

    /// <inheritdoc />
    public Task<byte[]> DecryptAes(byte[] data, SymmetricKeyConfig symmetricKeyConfig)
    {
        if (data.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Contains nothing to encrypt.");
        }

        return _pkcs11DeviceAdapter.DecryptAesGcm(data, symmetricKeyConfig.CkaLabel);
    }
}
