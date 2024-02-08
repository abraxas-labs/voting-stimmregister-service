// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

/// <summary>
/// CryptoStream Decryption Service.
/// </summary>
public interface IStreamDecryptionService
{
    /// <summary>
    /// AES decrypt stream with the specified and encrypted key.
    /// </summary>
    /// <param name="inputStream">The stream to decrypt.</param>
    /// <param name="aesCipherMetadata">The encrypted cipher to decrypt the stream.</param>
    /// <returns>The stream wrapped with decryption.</returns>
    Stream CreateAesMacDecryptCryptoStream(Stream inputStream, AesCipherMetadata aesCipherMetadata);

    /// <summary>
    /// Verify whether the computed hash of the input stream matches with the hash stored inside the AES cipher metadata.
    /// </summary>
    /// <param name="inputStream">The input stream.</param>
    /// <param name="aesCipherMetadata">The serialized AES cipher metdata.</param>
    /// <returns>Whether the hash matches.</returns>
    Task<bool> VerifyHMAC(Stream inputStream, AesCipherMetadata aesCipherMetadata);
}
