// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Core.Services.Supporting.Signing.Exceptions;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

public class AesStreamEncryption : IStreamDecryptionService, IStreamEncryptionService
{
    private const int IvSize = 16;
    private const int AesKeySize = 32;
    private const int CryptoStreamBufferSize = 4096;
    private const int MacKeySize = 16;
    private readonly IHsmCryptoAdapter _hsmCryptoAdapter;
    private readonly SymmetricKeyConfig _symmetricKeyConfig;

    public AesStreamEncryption(IHsmCryptoAdapter hsmCryptoAdapter)
    {
        _hsmCryptoAdapter = hsmCryptoAdapter;
        _symmetricKeyConfig = new SymmetricKeyConfig
        {
            CkaLabel = hsmCryptoAdapter.GetSignatureConfigLabels().Aes,
        };
    }

    /// <inheritdoc />
    public (Stream EncryptedStream, AesCipherMetadata AesCipherMetadata) CreateAesMacEncryptCryptoStream(Stream target)
    {
        // Initialize crypto parameters
        var iv = new byte[IvSize];
        RandomNumberGenerator.Fill(iv);

        var aesKey = new byte[AesKeySize];
        RandomNumberGenerator.Fill(aesKey);

        var macKey = new byte[MacKeySize];
        RandomNumberGenerator.Fill(macKey);

        var encryptedAesKey = _hsmCryptoAdapter.EncryptAes(aesKey, _symmetricKeyConfig);
        var encryptedMacKey = _hsmCryptoAdapter.EncryptAes(macKey, _symmetricKeyConfig);
        var cipher = new AesCipherMetadata(iv, encryptedAesKey, encryptedMacKey);
        var macStream = new HmacSha256Stream(target, macKey, cipher);

        // Create AES cipher stream
        using var aes = CreateAes(iv, aesKey);
        var cryptoStream = new CryptoStream(macStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

        return (cryptoStream, cipher);
    }

    /// <inheritdoc />
    public Stream CreateAesMacDecryptCryptoStream(Stream inputStream, AesCipherMetadata aesCipherMetadata)
    {
        var aesKey = _hsmCryptoAdapter.DecryptAes(aesCipherMetadata.EncryptedAesKey, _symmetricKeyConfig);

        using var aes = CreateAes(aesCipherMetadata.AesIv, aesKey);

        var aesDecryptor = aes.CreateDecryptor();
        return new CryptoStream(inputStream, aesDecryptor, CryptoStreamMode.Read, true);
    }

    /// <inheritdoc />
    public async Task<bool> VerifyHMAC(Stream inputStream, AesCipherMetadata aesCipherMetadata)
    {
        var macKey = _hsmCryptoAdapter.DecryptAes(aesCipherMetadata.EncryptedMacKey, _symmetricKeyConfig);
        using var hmac = new HMACSHA256(macKey);
        await using var hmacCryptoStream = new CryptoStream(inputStream, hmac, CryptoStreamMode.Read, true);

        using var owner = MemoryPool<byte>.Shared.Rent(CryptoStreamBufferSize);
        while (await hmacCryptoStream.ReadAsync(owner.Memory) > 0)
        {
            // Only for computing the hash.
        }

        if (hmac.Hash is null)
        {
            throw new DecryptionException("Hash was not computed and is null.");
        }

        var computedHmac = hmac.Hash;
        inputStream.Seek(0, SeekOrigin.Begin);

        return aesCipherMetadata.Hmac.SequenceEqual(computedHmac);
    }

    private static Aes CreateAes(byte[] iv, byte[] key)
    {
        var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.IV = iv;
        aes.Key = key;
        return aes;
    }
}
