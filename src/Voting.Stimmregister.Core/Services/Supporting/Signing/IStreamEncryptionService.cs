// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

/// <summary>
/// CryptoStream Encryption Service.
/// </summary>
public interface IStreamEncryptionService
{
    /// <summary>
    /// AES encrypt stream.
    /// </summary>
    /// <param name="target">The writeable stream to encrypt.</param>
    /// <returns>The stream wrapped with encryption.</returns>
    Task<(Stream EncryptedStream, AesCipherMetadata AesCipherMetadata)> CreateAesMacEncryptCryptoStream(Stream target);
}
