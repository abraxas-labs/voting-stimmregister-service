// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Adapter.Hsm.Configuration;

/// <summary>
/// Configuration options for HSM adapter..
/// </summary>
public class HsmConfig
{
    /// <summary>
    /// Gets or sets the path of the unmanaged PKCS#11 library.
    /// </summary>
    public string LibraryPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Login Pin so that a User can login into the device.
    /// </summary>
    public string LoginPin { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the slot id of the device.
    /// </summary>
    public ulong SlotId { get; set; }

    /// <summary>
    /// Gets or sets the CKA Label VOSR_ECDSA_PUBLIC_KEY.
    /// </summary>
    public string VosrEcdsaPublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CKA Label VOSR_ECDSA_PRIVATE_KEY.
    /// </summary>
    public string VosrEcdsaPrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CKA Label VOSR_AES_KEY_PRE.
    /// </summary>
    public string VosrAesKey { get; set; } = string.Empty;
}
