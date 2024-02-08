// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

public class AesCipherMetadata
{
    public AesCipherMetadata(byte[] iv, byte[] encryptedAesKey, byte[] encryptedMacKey)
    {
        AesIv = iv;
        EncryptedAesKey = encryptedAesKey;
        EncryptedMacKey = encryptedMacKey;
    }

    public AesCipherMetadata(byte[] iv, byte[] encryptedAesKey, byte[] encryptedMacKey, byte[] hmac)
    {
        AesIv = iv;
        EncryptedAesKey = encryptedAesKey;
        EncryptedMacKey = encryptedMacKey;
        Hmac = hmac;
    }

    public byte[] EncryptedAesKey { get; set; }

    public byte[] AesIv { get; set; }

    public byte[] EncryptedMacKey { get; set; }

    public byte[] Hmac { get; set; } = Array.Empty<byte>();
}
