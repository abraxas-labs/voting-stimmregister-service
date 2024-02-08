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
            LatestVersion => _serviceProvider.GetRequiredService<FilterVersionSignaturePayloadBuilderV1>(),
            _ => throw new InvalidOperationException($"Unsupported filter version signature version {filterVersion.SignatureVersion}"),
        };
    }

    internal IIntegritySignaturePayloadBuilder Get(BfsIntegrityEntity integrity)
    {
        return integrity.SignatureVersion switch
        {
            1 => _serviceProvider.GetRequiredService<IntegritySignaturePayloadBuilderV1>(),
            LatestVersion => _serviceProvider.GetRequiredService<IntegritySignaturePayloadBuilderV1>(),
            _ => throw new InvalidOperationException($"Unsupported integrity signature version {integrity.SignatureVersion}"),
        };
    }

    internal ISignaturePayloadBuilder<PersonEntity> Get(PersonEntity person)
    {
        return person.SignatureVersion switch
        {
            1 => _serviceProvider.GetRequiredService<PersonSignaturePayloadBuilderV1>(),
            LatestVersion => _serviceProvider.GetRequiredService<PersonSignaturePayloadBuilderV1>(),
            _ => throw new InvalidOperationException($"Unsupported person signature version {person.SignatureVersion}"),
        };
    }

    internal ISignaturePayloadBuilder<DomainOfInfluenceEntity> Get(DomainOfInfluenceEntity doi)
    {
        return doi.SignatureVersion switch
        {
            1 => _serviceProvider.GetRequiredService<DomainOfInfluenceSignaturePayloadBuilderV1>(),
            LatestVersion => _serviceProvider.GetRequiredService<DomainOfInfluenceSignaturePayloadBuilderV1>(),
            _ => throw new InvalidOperationException($"Unsupported doi signature version {doi.SignatureVersion}"),
        };
    }
}
