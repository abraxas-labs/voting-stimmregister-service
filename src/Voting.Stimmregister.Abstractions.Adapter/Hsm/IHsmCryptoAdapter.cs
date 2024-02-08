// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Signing;

namespace Voting.Stimmregister.Abstractions.Adapter.Hsm;

/// <summary>HSM services for PKCS11 crypto operations.</summary>
public interface IHsmCryptoAdapter
{
    /// <summary>
    /// Gets the system defined labels for the keys that can be used.
    /// </summary>
    /// <returns>The system defined labels for the keys that can be used.</returns>
    SignatureConfigLabels GetSignatureConfigLabels();

    /// <summary>
    /// Verifies a sha384 ecdsa signature is valid.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="signature">The signature.</param>
    /// <param name="signatureConfig">The config of the signature.</param>
    /// <returns>True if the signature is valid, false otherwise.</returns>
    bool VerifyEcdsaSha384Signature(byte[] data, byte[] signature, AsymmetricKeyConfig signatureConfig);

    /// <summary>
    /// Create a signature hash for the provided data.
    /// </summary>
    /// <param name="data">The data to get a signature for.</param>
    /// <param name="signatureConfig">The config to create the signature.</param>
    /// <returns>The signature hash.</returns>
    byte[] CreateEcdsaSha384Signature(byte[] data, AsymmetricKeyConfig signatureConfig);

    /// <summary>
    /// Create a signature hashes for the provided bulk data.
    /// </summary>
    /// <param name="bulkData">The bulk data to get a signature for.</param>
    /// <param name="signatureConfig">The config to create the signature.</param>
    /// <returns>The signature hashes.</returns>
    IReadOnlyList<byte[]> BulkCreateEcdsaSha384Signature(ICollection<byte[]> bulkData, AsymmetricKeyConfig signatureConfig);

    /// <summary>
    /// Encrypts the provided data with AES.
    /// </summary>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="symmetricKeyConfig">The config to create the encryption.</param>
    /// <returns>The encrypted data.</returns>
    byte[] EncryptAes(byte[] data, SymmetricKeyConfig symmetricKeyConfig);

    /// <summary>
    /// Decrypts the encrypted data with AES.
    /// </summary>
    /// <param name="data">The data to decrypt.</param>
    /// <param name="symmetricKeyConfig">The config to create the decryption.</param>
    /// <returns>The decrypted data.</returns>
    byte[] DecryptAes(byte[] data, SymmetricKeyConfig symmetricKeyConfig);
}
