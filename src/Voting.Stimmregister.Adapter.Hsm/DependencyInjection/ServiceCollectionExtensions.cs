// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Cryptography.Configuration;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Adapter.Hsm.Configuration;
using Voting.Stimmregister.Adapter.Hsm.Services;

namespace Voting.Stimmregister.Adapter.Hsm.DependencyInjection;

/// <summary>
/// Service collection extensions to register Adapter.Loganto services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the adapter hsm services to DI container.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="hsmConfig">The configuration to connect to the HSM.</param>
    /// <param name="enablePkcs11Mock">Whether Pkcs11 adapter should be mocked.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAdapterHsmServices(this IServiceCollection services, HsmConfig hsmConfig, bool enablePkcs11Mock)
    {
        if (!enablePkcs11Mock)
        {
            // NOTE: Stimmregister does not use a global config. Instead the config will be given on each call be able to choose which key should be used.
            var dummyConfig = new Pkcs11Config
            {
                LibraryPath = hsmConfig.LibraryPath,
                LoginPin = hsmConfig.LoginPin,
                SlotId = hsmConfig.SlotId,
                PublicKeyCkaLabel = hsmConfig.VosrEcdsaPublicKey,
            };
            services.AddVotingLibPkcs11(dummyConfig);
        }
        else
        {
            services.AddVotingLibPkcs11Mock();
        }

        return services
            .AddVotingLibCryptography()
            .AddSingleton(hsmConfig)
            .AddTransient<IHsmCryptoAdapter, HsmCryptoAdapter>();
    }
}
