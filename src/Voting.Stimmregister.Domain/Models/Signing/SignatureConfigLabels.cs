// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models.Signing;

/// <summary>
/// The Signature labels available to use. The effective naming is injected by the HSM config.
/// </summary>
public class SignatureConfigLabels
{
    public SignatureConfigLabels(string vosrEcdsaPublicKey, string vosrEcdsaPrivateKey, string aes)
    {
        VosrEcdsaPublicKey = vosrEcdsaPublicKey;
        VosrEcdsaPrivateKey = vosrEcdsaPrivateKey;
        Aes = aes;
    }

    /// <summary>
    /// Gets Voting Stimmregister ECDSA Public Key label.
    /// </summary>
    public string VosrEcdsaPublicKey { get; }

    /// <summary>
    /// Gets Voting Stimmregister ECDSA Private Key label.
    /// </summary>
    public string VosrEcdsaPrivateKey { get; }

    /// <summary>
    /// Gets AEs Key label.
    /// </summary>
    public string Aes { get; }
}
