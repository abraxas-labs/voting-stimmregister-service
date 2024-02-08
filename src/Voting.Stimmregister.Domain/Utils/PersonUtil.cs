// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Domain.Utils;

public static class PersonUtil
{
    public static bool HasResidenceAddress(PersonEntity person)
        => !string.IsNullOrWhiteSpace(person.ResidenceAddressExtensionLine1) ||
           !string.IsNullOrWhiteSpace(person.ResidenceAddressExtensionLine2) ||
           !string.IsNullOrWhiteSpace(person.ResidenceAddressStreet) ||
           !string.IsNullOrWhiteSpace(person.ResidenceAddressHouseNumber) ||
           !string.IsNullOrWhiteSpace(person.ResidenceAddressDwellingNumber) ||
           !string.IsNullOrWhiteSpace(person.ResidenceAddressTown) ||
           !string.IsNullOrWhiteSpace(person.ResidenceAddressPostOfficeBoxText) ||
           !string.IsNullOrWhiteSpace(person.ResidenceAddressZipCode);

    public static bool HasContactAddress(PersonEntity person)
        => !string.IsNullOrWhiteSpace(person.ContactAddressExtensionLine1) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressExtensionLine2) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressStreet) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressHouseNumber) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressDwellingNumber) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressTown) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressPostOfficeBoxText) ||
           person.ContactAddressPostOfficeBoxNumber != null ||
           !string.IsNullOrWhiteSpace(person.ContactAddressZipCode) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressLine1) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressLine2) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressLine3) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressLine4) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressLine5) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressLine6) ||
           !string.IsNullOrWhiteSpace(person.ContactAddressLine7);
}
