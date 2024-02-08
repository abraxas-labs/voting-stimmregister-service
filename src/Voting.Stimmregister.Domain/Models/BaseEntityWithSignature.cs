// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Database.Models;

namespace Voting.Stimmregister.Domain.Models;

public abstract class BaseEntityWithSignature : BaseEntity
{
    /// <summary>
    /// Gets or sets the version of the signature.
    /// </summary>
    public byte SignatureVersion { get; set; }

    /// <summary>
    /// Gets or sets the key id of the signature.
    /// </summary>
    public string SignatureKeyId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hash of the signature.
    /// </summary>
    public byte[] Signature { get; set; } = Array.Empty<byte>();
}
