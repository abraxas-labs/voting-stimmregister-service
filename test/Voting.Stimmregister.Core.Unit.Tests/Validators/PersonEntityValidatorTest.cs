// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Voting.Stimmregister.Core.Validators;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Core.Unit.Tests.Validators;

public class PersonEntityValidatorTest
{
    private const int MaxStringLength = 150;
    private const int MaxIntNumber = 1000000;
    private const string ValidName = "Kurt";
    private const string ValidStreetName = "Ruhbergstrasse";
    private const string ValidTownName = "Goldach";
    private const string ValidCountryName = "Schweiz";
    private const string ValidCountryIso2 = "CH";
    private const string TooLongName = "KurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurtKurt";
    private const string TooLongSourceSystemId = "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";

    [Theory]
    [InlineData(ValidName, true, null)]
    [InlineData("", false, "NotEmptyValidator")]
    [InlineData(" ", false, "NotEmptyValidator")]
    [InlineData("  ", false, "NotEmptyValidator")]
    [InlineData(TooLongName, false, "MaximumLengthValidator")]
    public async Task TestFirstName(string firstName, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.FirstName = firstName, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(ValidName, true, null)]
    [InlineData("", false, "NotEmptyValidator")]
    [InlineData(" ", false, "NotEmptyValidator")]
    [InlineData("  ", false, "NotEmptyValidator")]
    [InlineData(TooLongName, false, "MaximumLengthValidator")]
    public async Task TestOfficialName(string officialName, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.OfficialName = officialName, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(false, true, null)]
    [InlineData(true, true, null)]
    public async Task TestRestrictedVotingAndElectionRightFederation(
        bool restrictedVotingAndElectionRightFederation,
        bool expectIsValid,
        string? expectedErrorCode)
    {
        await TestValidation(
            p => p.RestrictedVotingAndElectionRightFederation = restrictedVotingAndElectionRightFederation,
            expectIsValid,
            expectedErrorCode);
    }

    [Theory]
    [InlineData("5345", true, null)]
    [InlineData(TooLongSourceSystemId, false, "MaximumLengthValidator")]
    [InlineData(null, false, "NotEmptyValidator")]
    public async Task TestSourceSystemId(string? sourceSystemId, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.SourceSystemId = sourceSystemId, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(1, true, null)]
    [InlineData(634546456, false, "InclusiveBetweenValidator")]
    [InlineData(-54, false, "InclusiveBetweenValidator")]
    public async Task TestDomainOfInfluenceId(int? domainOfInfluenceId, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.DomainOfInfluenceId = domainOfInfluenceId, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData("123", true, null)]
    [InlineData(" 123", true, null)]
    [InlineData("123 ", true, null)]
    [InlineData(" 123 ", true, null)]
    [InlineData("", true, null)]
    [InlineData(" ", false, "PredicateValidator")]
    [InlineData("  ", false, "PredicateValidator")]
    public async Task TestWhitespaces(string input, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.OriginalName = input, expectIsValid, expectedErrorCode);
        await TestValidation(p => p.ContactAddressDwellingNumber = input, expectIsValid, expectedErrorCode);
        await TestValidation(p => p.ResidenceAddressStreet = input, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(true, true, false, true, null)]
    [InlineData(true, false, false, true, null)]
    [InlineData(false, true, false, true, null)]
    [InlineData(false, false, true, true, null)]
    [InlineData(false, false, false, false, "PredicateValidator")]
    public async Task TestAddressAvailability(bool hasContactAddress, bool hasResidenceAddress, bool hasAddressLine1to7, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(
            p =>
            {
                var contactData = hasContactAddress ? "mock-data" : null;
                var contactAddressLines = hasAddressLine1to7 ? "mock-data" : null;
                var residenceData = hasResidenceAddress ? "mock-data" : null;

                p.ResidenceAddressExtensionLine1 = residenceData;
                p.ResidenceAddressExtensionLine2 = residenceData;
                p.ResidenceAddressStreet = residenceData;
                p.ResidenceAddressHouseNumber = residenceData;
                p.ResidenceAddressDwellingNumber = residenceData;
                p.ResidenceAddressTown = residenceData;
                p.ResidenceAddressPostOfficeBoxText = residenceData;
                p.ResidenceAddressZipCode = hasResidenceAddress ? "9000" : null!;
                p.ResidenceCountry = hasResidenceAddress ? "CH" : null!;

                p.ContactAddressExtensionLine1 = contactData;
                p.ContactAddressExtensionLine2 = contactData;
                p.ContactAddressStreet = contactData;
                p.ContactAddressHouseNumber = contactData;
                p.ContactAddressDwellingNumber = contactData;
                p.ContactAddressTown = contactData;
                p.ContactAddressPostOfficeBoxText = contactData;
                p.ContactAddressPostOfficeBoxNumber = hasContactAddress ? 5000 : null;
                p.ContactAddressZipCode = hasContactAddress ? "9001" : null!;
                p.ContactAddressCountryIdIso2 = hasContactAddress ? "CH" : null!;

                p.ContactAddressLine1 = contactAddressLines;
                p.ContactAddressLine2 = contactAddressLines;
                p.ContactAddressLine3 = contactAddressLines;
                p.ContactAddressLine4 = contactAddressLines;
                p.ContactAddressLine5 = contactAddressLines;
                p.ContactAddressLine6 = contactAddressLines;
                p.ContactAddressLine7 = contactAddressLines;
            },
            expectIsValid,
            expectedErrorCode);
    }

    [Theory]
    [InlineData(ResidenceType.HWS, true, null)]
    [InlineData(ResidenceType.Undefined, false, "PredicateValidator")]
    public async Task TestTypeOfResidence(ResidenceType typeOfResidence, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.TypeOfResidence = typeOfResidence, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(7560000000044L, true, null)]
    [InlineData(1560000000001L, false, "InclusiveBetweenValidator")]
    [InlineData(17569999999999L, false, "InclusiveBetweenValidator")]
    [InlineData(-1L, false, "InclusiveBetweenValidator")]
    [InlineData(null, true, null)]
    public async Task TestVn(long? vn, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.Vn = vn, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(SexType.Female, true, null)]
    [InlineData(SexType.Undefined, true, null)]
    public async Task TestSex(SexType sex, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.Sex = sex, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(ReligionType.Unknown, true, null)]
    [InlineData(ReligionType.Catholic, true, null)]
    public async Task TestReligion(ReligionType religion, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.Religion = religion, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData("", true, null)]
    [InlineData(ValidStreetName, true, null)]
    [InlineData(TooLongName, false, "MaximumLengthValidator")]
    public async Task TestStreet(string? street, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.ContactAddressStreet = street, expectIsValid, expectedErrorCode);
        await TestValidation(p => p.ResidenceAddressStreet = street, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData("", true, null)]
    [InlineData(ValidTownName, true, null)]
    [InlineData(TooLongName, false, "MaximumLengthValidator")]
    public async Task TestTown(string? town, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.ContactAddressTown = town, expectIsValid, expectedErrorCode);
        await TestValidation(p => p.ResidenceAddressTown = town, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(null, true, null)]
    [InlineData(ValidCountryIso2, true, null)]
    [InlineData(TooLongName, false, "MaximumLengthValidator")]
    public async Task TestCountry(string? country, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.Country = country, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(null, true, null)]
    [InlineData(ValidCountryName, true, null)]
    [InlineData(TooLongName, false, "MaximumLengthValidator")]
    public async Task TestMoveInCountryNameShort(string? country, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.MoveInCountryNameShort = country, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(null, true, null)]
    [InlineData("ABC", false, "ExactLengthValidator")]
    [InlineData("ABCD", false, "PredicateValidator")]
    [InlineData("5000", true, null)]
    [InlineData("10000", false, "ExactLengthValidator")]
    [InlineData("999", false, "ExactLengthValidator")]
    public async Task TestZipCode(string? zipCode, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.ResidenceAddressZipCode = zipCode, expectIsValid, expectedErrorCode);
        await TestValidation(p => p.ContactAddressZipCode = zipCode, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(null, true, null)]
    [InlineData("DE", true, null)]
    [InlineData("It", true, null)]
    [InlineData("fr", true, null)]
    [InlineData("rm", true, null)]
    [InlineData("en", true, null)]
    [InlineData("d", false, "ExactLengthValidator")]
    [InlineData("deu", false, "ExactLengthValidator")]
    public async Task TestLanguageOfCorrespondence(string? languageOfCorrespondence, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.LanguageOfCorrespondence = languageOfCorrespondence, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(null, false, "PredicateValidator")]
    [InlineData("2500-01-02", false, "PredicateValidator")]
    [InlineData("2010-01-02", true, null)]
    public async Task TestDateOfBirth(string? dateOfBirthString, bool expectIsValid, string? expectedErrorCode)
    {
        var dateOfBirth = dateOfBirthString == null ? DateOnly.MinValue : DateOnly.Parse(dateOfBirthString);
        await TestValidation(p => p.DateOfBirth = dateOfBirth, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(null, true, null)]
    [InlineData("AR", true, null)]
    [InlineData("ABC", false, "ExactLengthValidator")]
    [InlineData("XX", false, "PredicateValidator")]
    public async Task TestCanton(string? canton, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.MoveInCantonAbbreviation = canton, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(null, false, "NotEmptyValidator")]
    [InlineData("Brileshampton Mount Becclesmau Rurydedenc", false, "MaximumLengthValidator")]
    [InlineData("Brileshampton", true, "X")]
    public async Task TestRuleForMunicipalityName(string? value, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.MunicipalityName = value, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData("CH", false, true, "X")]
    [InlineData("CH", true, true, "X")]
    [InlineData(null, false, true, null)]
    [InlineData("", false, false, "ExactLengthValidator")]
    [InlineData("CHH", false, false, "ExactLengthValidator")]
    [InlineData(null, true, false, "NotEmptyValidator")]
    [InlineData("", true, false, "NotEmptyValidator")]
    [InlineData("A", true, false, "ExactLengthValidator")]
    [InlineData("CHE", true, false, "ExactLengthValidator")]
    public async Task TestRuleForResidenceCountry(string? value, bool isSwissAbraod, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(
            p =>
            {
                p.ResidenceCountry = value;
                p.IsSwissAbroad = isSwissAbraod;
            },
            expectIsValid,
            expectedErrorCode);
    }

    [Theory]
    [InlineData(null, true, null)]
    [InlineData(TooLongName, false, "MaximumLengthValidator")]
    [InlineData("ABC", true, "X")]
    public async Task TestMaxLengths(string? value, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.AliasName = value, expectIsValid, expectedErrorCode);
        await TestValidation(p => p.ContactAddressLine3 = value, expectIsValid, expectedErrorCode);
        await TestValidation(p => p.AllianceName = value, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData("Postfach Test 12345", false, "MaximumLengthValidator")]
    [InlineData("Postfach 2000", true, null)]
    [InlineData(null, true, null)]
    public async Task TestPostOfficeBoxText(string? value, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.ContactAddressPostOfficeBoxText = value, expectIsValid, expectedErrorCode);
        await TestValidation(p => p.ResidenceAddressPostOfficeBoxText = value, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(100000000, false, "InclusiveBetweenValidator")]
    [InlineData(5000, true, null)]
    [InlineData(null, true, null)]
    public async Task TestPostOfficeBoxNumber(int? value, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.ContactAddressPostOfficeBoxNumber = value, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(0, false, "GreaterThanOrEqualValidator")]
    [InlineData(1, true, null)]
    [InlineData(999999999, true, null)]
    [InlineData(1000000000, false, "LessThanOrEqualValidator")]
    [InlineData(null, true, null)]
    public async Task TestRuleForResidenceBuildingId(int? value, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.ResidenceBuildingId = value, expectIsValid, expectedErrorCode);
    }

    [Theory]
    [InlineData(0, false, "GreaterThanOrEqualValidator")]
    [InlineData(1, true, null)]
    [InlineData(999, true, null)]
    [InlineData(1000, false, "LessThanOrEqualValidator")]
    [InlineData(null, true, null)]
    public async Task TestRuleForResidenceApartmentId(int? value, bool expectIsValid, string? expectedErrorCode)
    {
        await TestValidation(p => p.ResidenceApartmentId = value, expectIsValid, expectedErrorCode);
    }

    private static async Task TestValidation(
        Action<PersonEntity> changePersonAction,
        bool expectIsValid,
        string? expectedErrorCode = null)
    {
        var validator = new PersonEntityValidator(new PersonImportConfig
        {
            EntityValidationMaxIntNumber = MaxIntNumber,
            EntityValidationMaxStringLength = MaxStringLength,
        });

        var person = PersonMockedData.PersonForValidations;
        changePersonAction(person);
        var result = await validator.TestValidateAsync(person);
        Assert.Equal(expectIsValid, result.IsValid);
        if (!expectIsValid)
        {
            var error = result.Errors[0];
            Assert.Equal(expectedErrorCode, error.ErrorCode);
        }
    }
}
