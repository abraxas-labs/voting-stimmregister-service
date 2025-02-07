// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Lib.Common.Net;
using Voting.Lib.Iam.Configuration;
using Voting.Stimmregister.Abstractions.Core.Configuration;
using Voting.Stimmregister.Adapter.Data.Configuration;
using Voting.Stimmregister.Adapter.Ech.Configuration;
using Voting.Stimmregister.Adapter.Hsm.Configuration;
using Voting.Stimmregister.Adapter.VotingIam.Configuration;
using Voting.Stimmregister.Core.Configuration;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.WebService.Configuration;

public class AppConfig
{
    /// <summary>
    /// Gets or sets the CORS config options used within the <see cref="Voting.Lib.Common.DependencyInjection.ApplicationBuilderExtensions"/>
    /// to configure the CORS middleware from <see cref="Microsoft.AspNetCore.Builder.CorsMiddlewareExtensions"/>.
    /// </summary>
    public CorsConfig Cors { get; set; } = new CorsConfig();

    /// <summary>
    /// Gets or sets the Ports configuration with the listening ports for the application.
    /// </summary>
    public PortConfig Ports { get; set; } = new PortConfig();

    /// <summary>
    /// Gets or sets the port configuration for the metric endpoint.
    /// </summary>
    public ushort MetricPort { get; set; } = 9090;

    /// <summary>
    /// Gets or sets the Database configuration.
    /// </summary>
    public DataConfig Database { get; set; } = new DataConfig();

    /// <summary>
    /// Gets or sets the Memory Cache configuration.
    /// </summary>
    public MemoryCacheConfig MemoryCache { get; set; } = new MemoryCacheConfig();

    /// <summary>
    /// Gets or sets the identity provider configuration.
    /// </summary>
    public VotingIamConfig SecureConnect { get; set; } = new VotingIamConfig();

    /// <summary>
    /// Gets or sets the identity provider api.
    /// </summary>
    public Uri? SecureConnectApi { get; set; }

    /// <summary>
    /// Gets or sets the certificate pinning configuration.
    /// </summary>
    public CertificatePinningConfig CertificatePinning { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether detailed errors are enabled. Should not be enabled in production environments,
    /// as this could expose information about the internals of this service.
    /// </summary>
    public bool EnableDetailedErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether grpc-web should be used or plain grpc.
    /// </summary>
    public bool EnableGrpcWeb { get; set; }

    /// <summary>
    /// Gets or sets a list of paths where language headers are getting ignored.
    /// </summary>
    public HashSet<string> LanguageHeaderIgnoredPaths { get; set; } = new()
    {
        "/healthz",
        "/metrics",
    };

    /// <summary>
    /// Gets or sets a list of paths where access control list evaluations are getting ignored.
    /// </summary>
    public HashSet<string> AccessControlListEvaluationIgnoredPaths { get; set; } = new()
    {
        "/healthz",
        "/metrics",
    };

    /// <summary>
    /// Gets or sets a time span for the prometheus adapter interval.
    /// </summary>
    public TimeSpan PrometheusAdapterInterval { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the health check names of all health checks which are considered as non mission-critical
    /// (if any of them is unhealthy the system may still operate but in a degraded state).
    /// These health checks are monitored separately.
    /// </summary>
    public HashSet<string> LowPriorityHealthCheckNames { get; set; } = new();

    /// <summary>
    /// Gets or sets the imports settings.
    /// </summary>
    public ImportsConfig Imports { get; set; } = new();

    /// <summary>
    /// Gets or sets the E-Voting settings.
    /// </summary>
    public EVotingConfig EVoting { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the Pkcs11 signing should be mocked. Then HSM config isn't used.
    /// </summary>
    public bool EnablePkcs11Mock { get; set; }

    /// <summary>
    /// Gets or sets the HSM settings.
    /// </summary>
    public HsmConfig Hsm { get; set; } = new();

    /// <summary>
    /// Gets or sets the ech config.
    /// </summary>
    public EchConfig Ech { get; set; } = new(typeof(AppConfig).Assembly);

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
    /// Gets or sets filter related configurations.
    /// </summary>
    public FilterConfig Filter { get; set; } = new();

    /// <summary>
    /// Gets or sets person related configurations.
    /// </summary>
    public PersonConfig Person { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether server timing is enabled.
    /// Should only be enabled for troubleshooting the performance.
    /// </summary>
    public bool EnableServerTiming { get; set; }

    /// <summary>
    /// Gets or sets cleanup related configurations.
    /// </summary>
    public CleanupConfig Cleanup { get; set; } = new();

    /// <summary>
    /// Gets or sets the auth store configuration.
    /// </summary>
    public AuthStoreConfig AuthStore { get; set; } = new();
}
