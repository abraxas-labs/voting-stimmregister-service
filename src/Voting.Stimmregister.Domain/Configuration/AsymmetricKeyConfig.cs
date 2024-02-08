// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// The asymmetric configuration used for HSM signature operations.
/// </summary>
public class AsymmetricKeyConfig
{
    /// <summary>
    /// Gets or sets the CKA_LABEL which is required to get the stored Public Key of the device.
    /// </summary>
    public string PublicKeyCkaLabel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CKA_LABEL which is required to get the stored Private Key of the device.
    /// </summary>
    public string PrivateKeyCkaLabel { get; set; } = string.Empty;

    /// <summary>
    /// Gets the SignatureKeyId which is stored in the database records.
    /// </summary>
    /// <returns>An the value to be used as SignatureKeyId.</returns>
    public string SignatureKeyId => PublicKeyCkaLabel;
}
