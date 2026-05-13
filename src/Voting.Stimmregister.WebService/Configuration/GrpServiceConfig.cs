// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.WebService.Configuration;

/// <summary>
/// Grpc configuration options used for the gRPC services provisioning. />.
/// </summary>
public class GrpServiceConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether grpc-web should be used or plain grpc.
    /// </summary>
    public bool EnableGrpcWeb { get; set; }

    /// <summary>
    /// Gets or sets the max grpc message size in MB, defaults to 25MB.
    /// The import service is configured separately.
    /// </summary>
    public int MaxGrpcMessageSizeMb { get; set; } = 25;

    /// <summary>
    /// Gets the max grpc message size in Bytes.
    /// The import service is configured separately.
    /// </summary>
    public int MaxGrpcMessageSizeBytes => MaxGrpcMessageSizeMb * 1024 * 1024;

    /// <summary>
    /// Gets or sets a value indicating whether grpc reflection endpoint should be enabled or not.
    /// </summary>
    public bool EnableGrpcReflection { get; set; }
}
