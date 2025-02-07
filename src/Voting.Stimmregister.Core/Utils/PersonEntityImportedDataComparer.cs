// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Utils;

/// <summary>
/// A comparer for the <see cref="PersonEntity"/> which compares all business attributes relevant for the import.
/// Ignores data not changed by the imported business data (eg. the version, the creation date, the id, ...).
/// </summary>
internal sealed class PersonEntityImportedDataComparer : IEqualityComparer<PersonEntity>
{
    public static readonly PersonEntityImportedDataComparer Instance = new();

    private PersonEntityImportedDataComparer()
    {
    }

    public bool Equals(PersonEntity? x, PersonEntity? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.SourceSystemId == y.SourceSystemId
            && x.SourceSystemName == y.SourceSystemName
            && x.Vn == y.Vn
            && x.OfficialName == y.OfficialName
            && x.FirstName == y.FirstName
            && x.Sex == y.Sex
            && x.DateOfBirth.Equals(y.DateOfBirth)
            && x.DateOfBirthAdjusted == y.DateOfBirthAdjusted
            && x.OriginalName == y.OriginalName
            && x.AllianceName == y.AllianceName
            && x.AliasName == y.AliasName
            && x.OtherName == y.OtherName
            && x.CallName == y.CallName
            && x.Country == y.Country
            && x.CountryNameShort == y.CountryNameShort
            && x.ContactAddressExtensionLine1 == y.ContactAddressExtensionLine1
            && x.ContactAddressExtensionLine2 == y.ContactAddressExtensionLine2
            && x.ContactAddressStreet == y.ContactAddressStreet
            && x.ContactAddressHouseNumber == y.ContactAddressHouseNumber
            && x.ContactAddressDwellingNumber == y.ContactAddressDwellingNumber
            && x.ContactAddressPostOfficeBoxText == y.ContactAddressPostOfficeBoxText
            && x.ContactAddressPostOfficeBoxNumber == y.ContactAddressPostOfficeBoxNumber
            && x.ContactAddressTown == y.ContactAddressTown
            && x.ContactAddressZipCode == y.ContactAddressZipCode
            && x.ContactAddressLocality == y.ContactAddressLocality
            && x.ContactAddressLine1 == y.ContactAddressLine1
            && x.ContactAddressLine2 == y.ContactAddressLine2
            && x.ContactAddressLine3 == y.ContactAddressLine3
            && x.ContactAddressLine4 == y.ContactAddressLine4
            && x.ContactAddressLine5 == y.ContactAddressLine5
            && x.ContactAddressLine6 == y.ContactAddressLine6
            && x.ContactAddressLine7 == y.ContactAddressLine7
            && x.ContactCantonAbbreviation == y.ContactCantonAbbreviation
            && x.ContactAddressCountryIdIso2 == y.ContactAddressCountryIdIso2
            && x.LanguageOfCorrespondence == y.LanguageOfCorrespondence
            && x.Religion == y.Religion
            && x.ResidencePermit == y.ResidencePermit
            && Nullable.Equals(x.ResidencePermitValidFrom, y.ResidencePermitValidFrom)
            && Nullable.Equals(x.ResidencePermitValidTill, y.ResidencePermitValidTill)
            && Nullable.Equals(x.ResidenceEntryDate, y.ResidenceEntryDate)
            && x.ResidenceCantonAbbreviation == y.ResidenceCantonAbbreviation
            && x.MunicipalityName == y.MunicipalityName
            && x.MunicipalityId == y.MunicipalityId
            && x.DomainOfInfluenceId == y.DomainOfInfluenceId
            && Nullable.Equals(x.MoveInArrivalDate, y.MoveInArrivalDate)
            && x.MoveInMunicipalityName == y.MoveInMunicipalityName
            && x.MoveInCantonAbbreviation == y.MoveInCantonAbbreviation
            && x.MoveInComesFrom == y.MoveInComesFrom
            && x.MoveInCountryNameShort == y.MoveInCountryNameShort
            && x.MoveInUnknown == y.MoveInUnknown
            && x.ResidenceAddressExtensionLine1 == y.ResidenceAddressExtensionLine1
            && x.ResidenceAddressExtensionLine2 == y.ResidenceAddressExtensionLine2
            && x.ResidenceAddressStreet == y.ResidenceAddressStreet
            && x.ResidenceAddressHouseNumber == y.ResidenceAddressHouseNumber
            && x.ResidenceAddressDwellingNumber == y.ResidenceAddressDwellingNumber
            && x.ResidenceAddressPostOfficeBoxText == y.ResidenceAddressPostOfficeBoxText
            && x.ResidenceAddressTown == y.ResidenceAddressTown
            && x.ResidenceCountry == y.ResidenceCountry
            && x.ResidenceAddressZipCode == y.ResidenceAddressZipCode
            && x.TypeOfResidence == y.TypeOfResidence
            && x.RestrictedVotingAndElectionRightFederation == y.RestrictedVotingAndElectionRightFederation
            && x.EVoting == y.EVoting
            && x.IsSwissAbroad == y.IsSwissAbroad
            && x.SendVotingCardsToDomainOfInfluenceReturnAddress == y.SendVotingCardsToDomainOfInfluenceReturnAddress
            && x.IsValid == y.IsValid
            && x.ValidationErrors == y.ValidationErrors
            && Nullable.Equals(x.DeletedDate, y.DeletedDate)
            && x.IsDeleted == y.IsDeleted
            && x.CantonBfs == y.CantonBfs
            && x.IsHouseholder == y.IsHouseholder
            && x.ResidenceBuildingId == y.ResidenceBuildingId
            && x.ResidenceApartmentId == y.ResidenceApartmentId

            // the hash set SequenceEqual does ignore duplicates
            // and ignores the order
            && x.PersonDois.ToHashSet(PersonDoiEntityImportedDataComparer.Instance).SetEquals(y.PersonDois);
    }

    public int GetHashCode(PersonEntity obj)
    {
        var hashCode = default(HashCode);
        hashCode.Add(obj.SourceSystemId);
        hashCode.Add((int)obj.SourceSystemName);
        hashCode.Add(obj.Vn);
        hashCode.Add(obj.OfficialName);
        hashCode.Add(obj.FirstName);
        hashCode.Add((int)obj.Sex);
        hashCode.Add(obj.DateOfBirth);
        hashCode.Add(obj.DateOfBirthAdjusted);
        hashCode.Add(obj.OriginalName);
        hashCode.Add(obj.AllianceName);
        hashCode.Add(obj.AliasName);
        hashCode.Add(obj.OtherName);
        hashCode.Add(obj.CallName);
        hashCode.Add(obj.Country);
        hashCode.Add(obj.CountryNameShort);
        hashCode.Add(obj.ContactAddressExtensionLine1);
        hashCode.Add(obj.ContactAddressExtensionLine2);
        hashCode.Add(obj.ContactAddressStreet);
        hashCode.Add(obj.ContactAddressHouseNumber);
        hashCode.Add(obj.ContactAddressDwellingNumber);
        hashCode.Add(obj.ContactAddressPostOfficeBoxText);
        hashCode.Add(obj.ContactAddressPostOfficeBoxNumber);
        hashCode.Add(obj.ContactAddressTown);
        hashCode.Add(obj.ContactAddressZipCode);
        hashCode.Add(obj.ContactAddressLocality);
        hashCode.Add(obj.ContactAddressLine1);
        hashCode.Add(obj.ContactAddressLine2);
        hashCode.Add(obj.ContactAddressLine3);
        hashCode.Add(obj.ContactAddressLine4);
        hashCode.Add(obj.ContactAddressLine5);
        hashCode.Add(obj.ContactAddressLine6);
        hashCode.Add(obj.ContactAddressLine7);
        hashCode.Add(obj.ContactCantonAbbreviation);
        hashCode.Add(obj.ContactAddressCountryIdIso2);
        hashCode.Add(obj.LanguageOfCorrespondence);
        hashCode.Add((int)obj.Religion);
        hashCode.Add(obj.ResidencePermit);
        hashCode.Add(obj.ResidencePermitValidFrom);
        hashCode.Add(obj.ResidencePermitValidTill);
        hashCode.Add(obj.ResidenceEntryDate);
        hashCode.Add(obj.ResidenceCantonAbbreviation);
        hashCode.Add(obj.MunicipalityName);
        hashCode.Add(obj.MunicipalityId);
        hashCode.Add(obj.DomainOfInfluenceId);
        hashCode.Add(obj.MoveInArrivalDate);
        hashCode.Add(obj.MoveInMunicipalityName);
        hashCode.Add(obj.MoveInCantonAbbreviation);
        hashCode.Add(obj.MoveInComesFrom);
        hashCode.Add(obj.MoveInCountryNameShort);
        hashCode.Add(obj.MoveInUnknown);
        hashCode.Add(obj.ResidenceAddressExtensionLine1);
        hashCode.Add(obj.ResidenceAddressExtensionLine2);
        hashCode.Add(obj.ResidenceAddressStreet);
        hashCode.Add(obj.ResidenceAddressHouseNumber);
        hashCode.Add(obj.ResidenceAddressDwellingNumber);
        hashCode.Add(obj.ResidenceAddressPostOfficeBoxText);
        hashCode.Add(obj.ResidenceAddressTown);
        hashCode.Add(obj.ResidenceCountry);
        hashCode.Add(obj.ResidenceAddressZipCode);
        hashCode.Add((int)obj.TypeOfResidence);
        hashCode.Add(obj.RestrictedVotingAndElectionRightFederation);
        hashCode.Add(obj.EVoting);
        hashCode.Add(obj.IsSwissAbroad);
        hashCode.Add(obj.SendVotingCardsToDomainOfInfluenceReturnAddress);
        hashCode.Add(obj.IsValid);
        hashCode.Add(obj.ValidationErrors);
        hashCode.Add(obj.DeletedDate);
        hashCode.Add(obj.IsDeleted);
        hashCode.Add(obj.PersonDois.Count);
        hashCode.Add(obj.CantonBfs);
        hashCode.Add(obj.IsHouseholder);
        hashCode.Add(obj.ResidenceBuildingId);
        hashCode.Add(obj.ResidenceApartmentId);
        return hashCode.ToHashCode();
    }
}
