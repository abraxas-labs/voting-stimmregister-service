// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using Microsoft.Extensions.ObjectPool;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Signing.PayloadBuilder;

public class PersonSignaturePayloadBuilderV1 : IDisposable, ISignaturePayloadBuilder<PersonEntity>
{
    private readonly SignaturePayloadBuilder _payloadBuilder;
    private bool _disposedValue;

    public PersonSignaturePayloadBuilderV1(
        ObjectPool<HashBuilder> hashBuilderPool,
        IHsmCryptoAdapter hsmCryptoAdapter)
    {
        _payloadBuilder = new SignaturePayloadBuilder(nameof(PersonEntity), 1, hsmCryptoAdapter.GetSignatureConfigLabels(), hashBuilderPool);
    }

    public SignaturePayload Build(PersonEntity person)
    {
        _payloadBuilder.HashBuilder
            .AppendDelimited(person.Id)
            .AppendDelimited(person.RegisterId)
            .AppendDelimited(person.Vn)
            .AppendDelimited(person.DomainOfInfluenceId)
            .AppendDelimited(person.SourceSystemId)
            .AppendDelimited(person.SourceSystemName)
            .AppendDelimited(person.OfficialName)
            .AppendDelimited(person.FirstName)
            .AppendDelimited(person.Sex)
            .AppendDelimited(person.DateOfBirth)
            .AppendDelimited(person.DateOfBirthAdjusted)
            .AppendDelimited(person.OriginalName)
            .AppendDelimited(person.AllianceName)
            .AppendDelimited(person.AliasName)
            .AppendDelimited(person.OtherName)
            .AppendDelimited(person.CallName)
            .AppendDelimited(person.LanguageOfCorrespondence)
            .AppendDelimited(person.Religion)
            .AppendDelimited(person.IsSwissAbroad)
            .AppendDelimited(person.SendVotingCardsToDomainOfInfluenceReturnAddress)
            .AppendDelimited(person.RestrictedVotingAndElectionRightFederation)
            .AppendDelimited(person.Country)
            .AppendDelimited(person.CountryNameShort)
            .AppendDelimited(person.EVoting)
            .AppendDelimited(person.MunicipalityName)
            .AppendDelimited(person.MunicipalityId)
            .AppendDelimited(person.TypeOfResidence)
            .AppendDelimited(person.ResidencePermit)
            .AppendDelimited(person.ResidencePermitValidFrom)
            .AppendDelimited(person.ResidencePermitValidTill)
            .AppendDelimited(person.ResidenceEntryDate)
            .AppendDelimited(person.ContactAddressExtensionLine1)
            .AppendDelimited(person.ContactAddressExtensionLine2)
            .AppendDelimited(person.ContactAddressStreet)
            .AppendDelimited(person.ContactAddressHouseNumber)
            .AppendDelimited(person.ContactAddressDwellingNumber)
            .AppendDelimited(person.ContactAddressPostOfficeBoxText)
            .AppendDelimited(person.ContactAddressPostOfficeBoxNumber)
            .AppendDelimited(person.ContactAddressLine1)
            .AppendDelimited(person.ContactAddressLine2)
            .AppendDelimited(person.ContactAddressLine3)
            .AppendDelimited(person.ContactAddressLine4)
            .AppendDelimited(person.ContactAddressLine5)
            .AppendDelimited(person.ContactAddressLine6)
            .AppendDelimited(person.ContactAddressLine7)
            .AppendDelimited(person.ContactAddressTown)
            .AppendDelimited(person.ContactAddressLocality)
            .AppendDelimited(person.ContactAddressZipCode)
            .AppendDelimited(person.ContactCantonAbbreviation)
            .AppendDelimited(person.ResidenceAddressExtensionLine1)
            .AppendDelimited(person.ResidenceAddressExtensionLine2)
            .AppendDelimited(person.ResidenceAddressStreet)
            .AppendDelimited(person.ResidenceAddressHouseNumber)
            .AppendDelimited(person.ResidenceAddressDwellingNumber)
            .AppendDelimited(person.ResidenceAddressPostOfficeBoxText)
            .AppendDelimited(person.ResidenceAddressTown)
            .AppendDelimited(person.ResidenceCountry)
            .AppendDelimited(person.ResidenceAddressZipCode)
            .AppendDelimited(person.ResidenceCantonAbbreviation)
            .AppendDelimited(person.MoveInArrivalDate)
            .AppendDelimited(person.VersionCount)
            .AppendDelimited(person.IsValid)
            .AppendDelimited(person.CreatedDate)
            .AppendDelimited(person.DeletedDate)
            .AppendDelimited(person.CantonBfs);

        AppendPersonDois(_payloadBuilder.HashBuilder, person);
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

    private void AppendPersonDois(HashBuilder hashBuilder, PersonEntity person)
    {
        var personDois = person.PersonDois.OrderBy(personDoi => personDoi.Id);
        foreach (var personDoi in personDois)
        {
            hashBuilder
                .AppendDelimited(personDoi.Id)
                .AppendDelimited(personDoi.Name)
                .AppendDelimited(personDoi.Identifier)
                .AppendDelimited(personDoi.Canton)
                .AppendDelimited(personDoi.DomainOfInfluenceType)
                .AppendDelimited(personDoi.PersonId);
        }
    }
}
