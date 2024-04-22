// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentValidation;
using Voting.Stimmregister.Core.Utils;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Constants;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Utils;

namespace Voting.Stimmregister.Core.Validators;

/// <summary>
/// Validator for the <see cref="PersonEntity"/> for validation reports.
/// </summary>
public class PersonEntityValidator : AbstractValidator<PersonEntity>
{
    private readonly int _maxStringLength;
    private readonly int _maxIntNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonEntityValidator"/> class with fluent validation rule sets.
    /// </summary>
    /// <param name="personImportConfig">The person import config.</param>
    public PersonEntityValidator(PersonImportConfig personImportConfig)
    {
        _maxStringLength = personImportConfig.EntityValidationMaxStringLength != 0
            ? personImportConfig.EntityValidationMaxStringLength
            : 150;

        _maxIntNumber = personImportConfig.EntityValidationMaxIntNumber != 0
            ? personImportConfig.EntityValidationMaxIntNumber
            : 100000000;

        InitializeRuleSet();
    }

    /// <summary>
    /// Initializes the fluent validation rule set.
    /// </summary>
    private void InitializeRuleSet()
    {
        GeneralRules();
        RuleForVnType();
        RulesForBaseNameType();
        RuleForLanguageOfCorrespondenceType();
        RuleForCountryNameShort();
        RuleForMunicipalityIdType();
        RuleForMunicipalityNameType();
        RuleForTypeOfResidenceType();
        RuleForAddressCountry();
        RuleForHouseNumberType();
        RuleForDwellingNumberType();
        RulesForStreetType();
        RulesForTownType();
        RulesForAddressLineType();
        RuleForLocalityType();
        RulesForZipCodeType();
        RuleForCantonAbbreviationType();
        RulesForPostOfficeBoxTextTypes();
        RulesForPostOfficeBoxNumberTypes();
        RulesForDates();
        RulesForDois();
        RulesForWhitespaces();
    }

    private void RulesForDois()
    {
        // TODO: VOTING-2714 localization for validation messages
        RuleFor(x => x.PersonDois)
            .Must(personDois => personDois.Any(pd =>
                pd.DomainOfInfluenceType == DomainOfInfluenceType.Og &&
                !string.IsNullOrWhiteSpace(pd.Canton) && !string.IsNullOrEmpty(pd.Name)))
            .When(p => p.Country == Countries.Switzerland)
            .WithMessage("Für Schweizer Staatsbürger muss mindestens ein Heimatort und Heimatkanton gesetzt sein.");
        RuleForEach(p => p.PersonDois.Where(pd => pd.DomainOfInfluenceType == DomainOfInfluenceType.Og).Select(personDoi => personDoi.Name))
            .MaximumLength(50)
            .OverridePropertyName(nameof(PersonEntity.PersonDois));
    }

    private void GeneralRules()
    {
        // See eCH-0044:personidentificationType.localPersonId
        RuleFor(p => p.SourceSystemId).NotEmpty().MaximumLength(_maxStringLength);

        // Internal foreign key
        RuleFor(p => p.DomainOfInfluenceId).InclusiveBetween(0, _maxIntNumber - 1);

        // Address availability
        RuleFor(p => p)
            .Must(p => PersonUtil.HasResidenceAddress(p) || PersonUtil.HasContactAddress(p))
            .WithMessage("Es muss mindestens eine Wohn- oder Kontaktadresse gesetzt sein.");
    }

    /// <summary>
    /// See eCH-0011:typeOfResidenceType.
    /// </summary>
    private void RuleForTypeOfResidenceType()
    {
        RuleFor(p => p.TypeOfResidence).Must(value => value != ResidenceType.Undefined);
    }

    /// <summary>
    /// See eCH-0045:swissAbroad.residenceCountry -> eCh-0008:countryNameShort.
    /// </summary>
    private void RuleForAddressCountry()
    {
        const int countryIso2Length = 2;

        RuleFor(p => p.ResidenceCountry)
            .Length(countryIso2Length)
            .When(p => !p.IsSwissAbroad);

        RuleFor(p => p.ResidenceCountry)
            .NotEmpty()
            .Length(countryIso2Length)
            .When(p => p.IsSwissAbroad);

        RuleFor(p => p.ContactAddressCountryIdIso2)
            .Length(countryIso2Length)
            .When(p => !p.IsSwissAbroad);
    }

    /// <summary>
    /// See eCH-0044:vnType.
    /// </summary>
    private void RuleForVnType()
    {
        RuleFor(p => p.Vn).InclusiveBetween(7560000000001, 7569999999999);
    }

    /// <summary>
    /// See eCH-0010:streetType.
    /// </summary>
    private void RulesForStreetType()
    {
        RuleFor(p => p.ResidenceAddressStreet).MaximumLength(150);
        RuleFor(p => p.ContactAddressStreet).MaximumLength(150);
    }

    /// <summary>
    /// See 0010:townType.
    /// </summary>
    private void RulesForTownType()
    {
        RuleFor(p => p.ContactAddressTown).MaximumLength(40);
        RuleFor(p => p.ResidenceAddressTown).MaximumLength(40);
    }

    /// <summary>
    /// See eCH-0010:locality.
    /// </summary>
    private void RuleForLocalityType()
    {
        RuleFor(p => p.ContactAddressLocality).MaximumLength(40);
    }

    /// <summary>
    /// See eCH-0008:countryNameShortType.
    /// </summary>
    private void RuleForCountryNameShort()
    {
        RuleFor(p => p.Country).MaximumLength(2);
        RuleFor(p => p.MoveInCountryNameShort).MaximumLength(50);
    }

    /// <summary>
    /// See eCH-0010:houseNumberType on version ech-0045 v4.0 -> ech-0010 v6.0.
    /// </summary>
    private void RuleForHouseNumberType()
    {
        RuleFor(p => p.ResidenceAddressHouseNumber).MaximumLength(12);
        RuleFor(p => p.ContactAddressHouseNumber).MaximumLength(12);
    }

    /// <summary>
    /// See eCH-0010:dwellingNumberType on version ech-0045 v4.0 -> ech-0010 v6.0.
    /// </summary>
    private void RuleForDwellingNumberType()
    {
        RuleFor(p => p.ContactAddressDwellingNumber).MaximumLength(10);
        RuleFor(p => p.ResidenceAddressDwellingNumber).MaximumLength(10);
    }

    /// <summary>
    /// See 'eCH-0010:swissZipCodeType' for Swiss citizens.
    /// For Swiss abroad citizens just the min- and max-length is validated.
    /// </summary>
    private void RulesForZipCodeType()
    {
        const int minZipCode = 1000;
        const int maxZipCode = 9999;
        const string swissCountryIdIso2 = "CH";

        RuleFor(p => p.ContactAddressZipCode)
            .Length(4)
            .Must(value => int.TryParse(value, out var intValue) && intValue is >= minZipCode and <= maxZipCode)
            .When(p => p.ContactAddressCountryIdIso2 == swissCountryIdIso2 && !string.IsNullOrEmpty(p.ContactAddressZipCode));

        RuleFor(p => p.ContactAddressZipCode)
            .MaximumLength(15)
            .When(p => p.ContactAddressCountryIdIso2 != swissCountryIdIso2 && !string.IsNullOrEmpty(p.ContactAddressZipCode));

        RuleFor(p => p.ResidenceAddressZipCode)
            .Length(4)
            .Must(value => int.TryParse(value, out var intValue) && intValue is >= minZipCode and <= maxZipCode)
            .When(p => !string.IsNullOrEmpty(p.ResidenceAddressZipCode));
    }

    /// <summary>
    /// See eCH-0045:languageOfCorrespondence.
    /// </summary>
    private void RuleForLanguageOfCorrespondenceType()
    {
        RuleFor(p => p.LanguageOfCorrespondence).Length(2).When(p => p.LanguageOfCorrespondence != null);
    }

    /// <summary>
    /// See eCH-0011:baseNameType.
    /// </summary>
    private void RulesForBaseNameType()
    {
        RuleFor(p => p.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(p => p.OfficialName).NotEmpty().MaximumLength(100);
        RuleFor(p => p.AliasName).MaximumLength(100);
        RuleFor(p => p.OriginalName).MaximumLength(100);
        RuleFor(p => p.OtherName).MaximumLength(100);
        RuleFor(p => p.CallName).MaximumLength(100);
        RuleFor(p => p.AllianceName).MaximumLength(100);
    }

    /// <summary>
    /// See eCH-0011:birthDataType.
    /// </summary>
    private void RuleForDateOfBirthType()
    {
        RuleFor(p => p.DateOfBirth)
            .NotNull()
            .DateOnlyMustNotHaveMinValue()
            .Must(value => value <= DateOnly.FromDateTime(DateTime.Today));
    }

    /// <summary>
    /// See eCH-0007:swissMunicipalityType/municipalityName.
    /// </summary>
    private void RuleForMunicipalityNameType()
    {
        RuleFor(p => p.MunicipalityName)
            .NotEmpty()
            .MaximumLength(40);
    }

    /// <summary>
    /// See 0007:swissMunicipalityType/municipalityId.
    /// </summary>
    private void RuleForMunicipalityIdType()
    {
        RuleFor(p => p.MunicipalityId)
            .NotEmpty()
            .InclusiveBetween(1, 9999);
    }

    /// <summary>
    /// See eCH-0010:addressLineType.
    /// </summary>
    private void RulesForAddressLineType()
    {
        const int maxLengthAddressLine = 60;

        RuleFor(p => p.ResidenceAddressExtensionLine1).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ResidenceAddressExtensionLine2).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressExtensionLine1).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressExtensionLine2).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressLine1).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressLine2).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressLine3).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressLine4).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressLine5).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressLine6).MaximumLength(maxLengthAddressLine);
        RuleFor(p => p.ContactAddressLine7).MaximumLength(maxLengthAddressLine);
    }

    /// <summary>
    /// See eCH0010:postOfficeBoxTextType.
    /// </summary>
    private void RulesForPostOfficeBoxTextTypes()
    {
        const byte maxPostOfficeBoxText = 15;

        RuleFor(p => p.ContactAddressPostOfficeBoxText).MaximumLength(maxPostOfficeBoxText);
        RuleFor(p => p.ResidenceAddressPostOfficeBoxText).MaximumLength(maxPostOfficeBoxText);
    }

    /// <summary>
    /// See eCH0010:postOfficeBoxNumberType.
    /// </summary>
    private void RulesForPostOfficeBoxNumberTypes()
    {
        const int maxPostOfficeBoxNumber = 99999999;

        RuleFor(p => p.ContactAddressPostOfficeBoxNumber)
            .InclusiveBetween(0, maxPostOfficeBoxNumber)
            .When(p => p.ContactAddressPostOfficeBoxNumber.HasValue);
    }

    /// <summary>
    /// See eCH-0007:cantonAbbreviation.
    /// </summary>
    private void RuleForCantonAbbreviationType()
    {
        RuleFor(p => p.MoveInCantonAbbreviation)
            .Length(2)
            .Must(value => Enum.TryParse<Canton>(value, out _))
            .When(p => !string.IsNullOrEmpty(p.MoveInCantonAbbreviation));
    }

    private void RulesForDates()
    {
        RuleForDateOfBirthType();
        RuleFor(p => p.MoveInArrivalDate).DateOnlyMustNotHaveMinValue();
        RuleFor(p => p.ResidencePermitValidTill).DateOnlyMustNotHaveMinValue();
        RuleFor(p => p.ResidenceEntryDate).DateOnlyMustNotHaveMinValue();
        RuleFor(p => p.ResidencePermitValidFrom).DateOnlyMustNotHaveMinValue();
    }

    private void RulesForWhitespaces()
    {
        var paramExp = Expression.Parameter(typeof(PersonEntity));

        foreach (var propInfo in typeof(PersonEntity).GetProperties())
        {
            if (propInfo.PropertyType == typeof(string) && ReflectionUtil.IsNullable(propInfo))
            {
                var memExp = Expression.Property(paramExp, propInfo);
                var unaryExp = Expression.Convert(memExp, propInfo.PropertyType);
                var expression = Expression.Lambda<Func<PersonEntity, string>>(unaryExp, paramExp);

                RuleFor(expression).MustNotContainWhitespacesOnly();
            }
        }
    }
}
