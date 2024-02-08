// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Voting.Lib.Cryptography.Asymmetric;
using Voting.Lib.Cryptography.Configuration;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Adapter.Hsm.Configuration;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Signing;

namespace Voting.Stimmregister.Adapter.Hsm.Services;

/// <inheritdoc cref="IHsmCryptoAdapter"/>
public class HsmCryptoAdapter : IHsmCryptoAdapter
{
    private readonly IPkcs11DeviceAdapter _pkcs11DeviceAdapter;
    private readonly HsmConfig _hsmConfig;

    public HsmCryptoAdapter(IPkcs11DeviceAdapter pkcs11DeviceAdapter, HsmConfig hsmConfig)
    {
        _pkcs11DeviceAdapter = pkcs11DeviceAdapter;
        _hsmConfig = hsmConfig;
    }

    /// <inheritdoc />
    public SignatureConfigLabels GetSignatureConfigLabels()
        => new(_hsmConfig.VosrEcdsaPublicKey, _hsmConfig.VosrEcdsaPrivateKey, _hsmConfig.VosrAesKey);

    /// <inheritdoc />
    public bool VerifyEcdsaSha384Signature(byte[] data, byte[] signature, AsymmetricKeyConfig signatureConfig)
        => _pkcs11DeviceAdapter.VerifyEcdsaSha384Signature(data, signature);

    /// <inheritdoc />
    public byte[] CreateEcdsaSha384Signature(byte[] data, AsymmetricKeyConfig signatureConfig)
    {
        var bulkData = new List<byte[]> { data };
        return BulkCreateEcdsaSha384Signature(bulkData, signatureConfig).Single();
    }

    /// <inheritdoc />
    public IReadOnlyList<byte[]> BulkCreateEcdsaSha384Signature(ICollection<byte[]> bulkData, AsymmetricKeyConfig signatureConfig)
    {
        if (bulkData.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bulkData), "Contains no elements to sign.");
        }

        return _pkcs11DeviceAdapter.BulkCreateEcdsaSha384Signature(bulkData, CreatePkcs11Config(signatureConfig));
    }

    /// <inheritdoc />
    public byte[] EncryptAes(byte[] data, SymmetricKeyConfig symmetricKeyConfig)
    {
        if (data.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Contains nothing to encrypt.");
        }

        return _pkcs11DeviceAdapter.EncryptAes(data, CreatePkcs11Config(symmetricKeyConfig));
    }

    /// <inheritdoc />
    public byte[] DecryptAes(byte[] data, SymmetricKeyConfig symmetricKeyConfig)
    {
        if (data.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Contains nothing to encrypt.");
        }

        return _pkcs11DeviceAdapter.DecryptAes(data, CreatePkcs11Config(symmetricKeyConfig));
    }

    private Pkcs11Config CreatePkcs11Config(AsymmetricKeyConfig config)
    {
        return new Pkcs11Config
        {
            LibraryPath = _hsmConfig.LibraryPath,
            LoginPin = _hsmConfig.LoginPin,
            SlotId = _hsmConfig.SlotId,
            PrivateKeyCkaLabel = config.PrivateKeyCkaLabel,
            PublicKeyCkaLabel = config.PublicKeyCkaLabel,
        };
    }

    private Pkcs11Config CreatePkcs11Config(SymmetricKeyConfig config)
    {
        return new Pkcs11Config
        {
            LibraryPath = _hsmConfig.LibraryPath,
            LoginPin = _hsmConfig.LoginPin,
            SlotId = _hsmConfig.SlotId,
            SecretKeyCkaLabel = config.CkaLabel,
        };
    }
}
