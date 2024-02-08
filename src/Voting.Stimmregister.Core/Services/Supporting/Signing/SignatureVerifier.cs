// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Core.Diagnostics;
using Voting.Stimmregister.Core.Services.Supporting.Signing.Exceptions;
using Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

public class SignatureVerifier
{
    private readonly ActivityFactory<SignatureVerifier> _activityFactory;
    private readonly ILogger<SignatureVerifier> _logger;
    private readonly IHsmCryptoAdapter _hsmCryptoAdapter;

    public SignatureVerifier(
        ActivityFactory<SignatureVerifier> activityFactory,
        ILogger<SignatureVerifier> logger,
        IHsmCryptoAdapter hsmCryptoAdapter)
    {
        _activityFactory = activityFactory;
        _logger = logger;
        _hsmCryptoAdapter = hsmCryptoAdapter;
    }

    internal void EnsureValidSignature(BaseEntityWithSignature entity, SignaturePayload payload)
    {
        if (IsValidSignature(entity.Signature, payload))
        {
            return;
        }

        _logger.LogError(SecurityLogging.SecurityEventId, "Signature validation of {Type} with id {Id} failed", payload.TypeName, entity.Id);
        throw new SignatureInvalidException(payload.TypeName, entity.Id);
    }

    private bool IsValidSignature(byte[] signature, SignaturePayload payload)
    {
        using var activity = _activityFactory.Start("signature-is-valid");
        return _hsmCryptoAdapter.VerifyEcdsaSha384Signature(payload.Payload, signature, payload.Config);
    }
}
