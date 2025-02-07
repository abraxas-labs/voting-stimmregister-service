// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

public class SignaturePayloadBuilderFactory
{
    private const byte LatestVersion = default;
    private readonly IServiceProvider _serviceProvider;

    public SignaturePayloadBuilderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal IIncrementalSignaturePayloadBuilder<FilterVersionEntity, PersonEntity> Get(FilterVersionEntity filterVersion)
    {
        return filterVersion.SignatureVersion switch
        {
            1 => _serviceProvider.GetRequiredService<FilterVersionSignaturePayloadBuilderV1>(),
            2 => _serviceProvider.GetRequiredService<FilterVersionSignaturePayloadBuilderV2>(),
            LatestVersion => _serviceProvider.GetRequiredService<FilterVersionSignaturePayloadBuilderV2>(),
            _ => throw new InvalidOperationException($"Unsupported filter version signature version {filterVersion.SignatureVersion}"),
        };
    }

    internal IIntegritySignaturePayloadBuilder Get(BfsIntegrityEntity integrity)
    {
        return integrity.SignatureVersion switch
        {
            1 => _serviceProvider.GetRequiredService<IntegritySignaturePayloadBuilderV1>(),
            2 => _serviceProvider.GetRequiredService<IntegritySignaturePayloadBuilderV2>(),
            LatestVersion => _serviceProvider.GetRequiredService<IntegritySignaturePayloadBuilderV2>(),
            _ => throw new InvalidOperationException($"Unsupported integrity signature version {integrity.SignatureVersion}"),
        };
    }
}
