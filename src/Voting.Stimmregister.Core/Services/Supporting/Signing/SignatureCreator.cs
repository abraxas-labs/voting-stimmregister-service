// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Core.Diagnostics;
using Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing;

public class SignatureCreator
{
    private readonly ActivityFactory<SignatureCreator> _activityFactory;
    private readonly IHsmCryptoAdapter _hsmCryptoAdapter;

    public SignatureCreator(ActivityFactory<SignatureCreator> activityFactory, IHsmCryptoAdapter hsmCryptoAdapter)
    {
        _activityFactory = activityFactory;
        _hsmCryptoAdapter = hsmCryptoAdapter;
    }

    internal void BulkSign(IReadOnlyList<SignaturePayload> payloads, IEnumerable<BaseEntityWithSignature> entities)
    {
        using var activity = _activityFactory.Start("bulk-sign");

        if (payloads.Count == 0)
        {
            return;
        }

        var primaryPayload = payloads[0];
        var rawPayloads = payloads.Select(x => x.Payload).ToList();
        var bulkCreateSignatures = _hsmCryptoAdapter.BulkCreateEcdsaSha384Signature(rawPayloads, primaryPayload.Config);

        var i = 0;
        foreach (var domainOfInfluence in entities)
        {
            domainOfInfluence.SignatureVersion = primaryPayload.Version;
            domainOfInfluence.SignatureKeyId = primaryPayload.Config.SignatureKeyId;
            domainOfInfluence.Signature = bulkCreateSignatures[i];
            i++;
        }
    }

    internal void Sign(SignaturePayload payload, BaseEntityWithSignature entity)
    {
        using var activity = _activityFactory.Start("sign");

        var integritySignature = _hsmCryptoAdapter.CreateEcdsaSha384Signature(payload.Payload, payload.Config);

        entity.SignatureVersion = payload.Version;
        entity.SignatureKeyId = payload.Config.SignatureKeyId;
        entity.Signature = integritySignature;
    }
}
