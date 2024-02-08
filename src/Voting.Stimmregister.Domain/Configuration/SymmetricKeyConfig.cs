// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// The AES configuration used for encyption and decryption HSM operations.
/// </summary>
public class SymmetricKeyConfig
{
    /// <summary>
    /// Gets or sets the CKA_LABEL which is required to get the stored symmetric Key.
    /// </summary>
    public string CkaLabel { get; set; } = string.Empty;
}
