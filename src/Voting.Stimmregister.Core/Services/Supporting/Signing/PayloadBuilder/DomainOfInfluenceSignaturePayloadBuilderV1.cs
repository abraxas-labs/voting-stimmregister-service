// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.Extensions.ObjectPool;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

internal class DomainOfInfluenceSignaturePayloadBuilderV1 : IDisposable, ISignaturePayloadBuilder<DomainOfInfluenceEntity>
{
    private readonly SignaturePayloadBuilder _payloadBuilder;
    private bool _disposedValue;

    public DomainOfInfluenceSignaturePayloadBuilderV1(
        ObjectPool<HashBuilder> hashBuilderPool,
        IHsmCryptoAdapter hsmCryptoAdapter)
    {
        _payloadBuilder = new SignaturePayloadBuilder(nameof(DomainOfInfluenceEntity), Version, hsmCryptoAdapter.GetSignatureConfigLabels(), hashBuilderPool);
    }

    public byte Version => 1;

    public SignaturePayload Build(DomainOfInfluenceEntity domainOfInfluence)
    {
        _payloadBuilder.HashBuilder
            .AppendDelimited(domainOfInfluence.Id)
            .AppendDelimited(domainOfInfluence.DomainOfInfluenceId)
            .AppendDelimited(domainOfInfluence.MunicipalityId)
            .AppendDelimited(domainOfInfluence.Street)
            .AppendDelimited(domainOfInfluence.HouseNumber)
            .AppendDelimited(domainOfInfluence.HouseNumberAddition)
            .AppendDelimited(domainOfInfluence.SwissZipCode)
            .AppendDelimited(domainOfInfluence.IsPartOfPoliticalMunicipality)
            .AppendDelimited(domainOfInfluence.Town)
            .AppendDelimited(domainOfInfluence.PoliticalCircleId)
            .AppendDelimited(domainOfInfluence.PoliticalCircleName)
            .AppendDelimited(domainOfInfluence.CatholicChurchCircleId)
            .AppendDelimited(domainOfInfluence.CatholicChurchCircleName)
            .AppendDelimited(domainOfInfluence.EvangelicChurchCircleId)
            .AppendDelimited(domainOfInfluence.EvangelicChurchCircleName)
            .AppendDelimited(domainOfInfluence.SchoolCircleId)
            .AppendDelimited(domainOfInfluence.SchoolCircleName)
            .AppendDelimited(domainOfInfluence.TrafficCircleId)
            .AppendDelimited(domainOfInfluence.TrafficCircleName)
            .AppendDelimited(domainOfInfluence.ResidentialDistrictCircleId)
            .AppendDelimited(domainOfInfluence.ResidentialDistrictCircleName)
            .AppendDelimited(domainOfInfluence.PeopleCouncilCircleId)
            .AppendDelimited(domainOfInfluence.PeopleCouncilCircleName);

        return _payloadBuilder.GetPayloadAndReset();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _payloadBuilder.Dispose();
            }

            _disposedValue = true;
        }
    }
}
