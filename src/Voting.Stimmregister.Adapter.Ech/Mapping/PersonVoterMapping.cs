// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Ech0007_6_0;
using Ech0010_6_0;
using Ech0011_8_1;
using Ech0044_4_1;
using Ech0045_4_0;
using Ech0155_4_0;
using Microsoft.Extensions.Logging;
using Voting.Lib.Ech.Ech0045_4_0.Models;
using Voting.Stimmregister.Adapter.Ech.Configuration;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Utils;
using CantonAbbreviation = Ech0007_5_0.CantonAbbreviationType;

namespace Voting.Stimmregister.Adapter.Ech.Mapping;

public class PersonVoterMapping : IPersonVoterMapping
{
    private const string PersonIdCategoryLocalSourceId = "LOC.EW";
    private const string PersonIdCategoryLocalId = "LOC";
    private const string PersonIdCategoryLocalRevisionId = "LOC.REV";
    private const string SwissCountryIdIso2 = "CH";
    private const string ResidenceCountryUnknown = "Unbekannt";

    private readonly ICountryHelperService _countryHelperService;
    private readonly ILogger<PersonVoterMapping> _logger;
    private readonly EchConfig _config;

    public PersonVoterMapping(ICountryHelperService countryHelperService, ILogger<PersonVoterMapping> logger, EchConfig config)
    {
        _countryHelperService = countryHelperService;
        _logger = logger;
        _config = config;
    }

    public VotingPersonType ToEchVoter(PersonEntity person)
    {
        var votingPersonType = new VotingPersonType
        {
            Person = BuildPerson(person),
            ElectoralAddress = PersonUtil.HasResidenceAddress(person) ? BuildElectoralAddress(person) : BuildDeliveryAddress(person),
            DeliveryAddress = PersonUtil.HasContactAddress(person) ? BuildDeliveryAddress(person) : BuildElectoralAddress(person),
            IsEvoter = person is { EVoting: true, IsSwissAbroad: false },
        };

        foreach (var doiInfo in BuildDomainOfInfluenceInfoType(person))
        {
            votingPersonType.DomainOfInfluenceInfo.Add(doiInfo);
        }

        return votingPersonType;
    }

    private static List<VotingPersonTypeDomainOfInfluenceInfo> BuildDomainOfInfluenceInfoType(PersonEntity person)
        => person.PersonDois
            .OrderBy(pd => pd.DomainOfInfluenceType)
            .ThenBy(pd => pd.Name)
            .ThenBy(pd => pd.Canton)
            .Select(pd =>
                new VotingPersonTypeDomainOfInfluenceInfo
                {
                    DomainOfInfluence = new DomainOfInfluenceType
                    {
                        DomainOfInfluenceTypeProperty = pd.DomainOfInfluenceType.ToEchDomainOfInfluence(),
                        LocalDomainOfInfluenceIdentification = DoiIdentifierMapping.ToEchIdentifier(pd),
                        DomainOfInfluenceName = pd.Name,
                    },
                })
            .ToList();

    private static NamedPersonIdType? BuildLocalPersonId(PersonEntity person)
        => !string.IsNullOrWhiteSpace(person.SourceSystemId) ?
            new NamedPersonIdType { PersonIdCategory = PersonIdCategoryLocalSourceId, PersonId = person.SourceSystemId }
            : throw new ArgumentException($"The person with id {person.Id} is missing a local person id.", nameof(person));

    private static List<NamedPersonIdType> BuildOtherPersonIds(PersonEntity person)
        => person.SourceSystemId != null ?
            new()
            {
                new NamedPersonIdType { PersonIdCategory = PersonIdCategoryLocalId, PersonId = person.RegisterId.ToString() },
                new NamedPersonIdType { PersonIdCategory = PersonIdCategoryLocalRevisionId, PersonId = person.Id.ToString() },
            }
            : new();

    private static List<PlaceOfOriginType> BuildPlaceOfOrigins(PersonEntity person)
        => person.PersonDois
            .Where(pd => pd.DomainOfInfluenceType == Domain.Enums.DomainOfInfluenceType.Og)
            .OrderBy(pd => pd.DomainOfInfluenceType)
            .ThenBy(pd => pd.Name)
            .ThenBy(pd => pd.Canton)
            .Select(x => BuildPlaceOfOrigin(x.Name, x.Canton))
            .WhereNotNull()
            .ToList();

    private static PlaceOfOriginType? BuildPlaceOfOrigin(string? origin, string? originCanton)
    {
        return origin == null || originCanton == null ||
               !Enum.TryParse<CantonAbbreviation>(originCanton, true, out var canton)
            ? null
            : new PlaceOfOriginType { Canton = canton, OriginName = origin, };
    }

    private static PersonMailAddressType BuildElectoralAddress(PersonEntity person)
    {
        var personAddress = new PersonMailAddressType
        {
            Person = new()
            {
                MrMrs = person.Sex.ToEchMrMrs(),
                FirstName = CallNameOrFirstName(person).Truncate(30),
                LastName = LastName(person).Truncate(30),
            },
            AddressInformation = BuildElectoralAddressInformation(person),
        };

        return personAddress;
    }

    private static string PlaceholderIfEmptyOrNull(string? s)
        => string.IsNullOrEmpty(s) ? "?" : s;

    private static string CallNameOrFirstName(PersonEntity person)
        => string.IsNullOrEmpty(person.CallName)
            ? person.FirstName
            : person.CallName;

    private static string LastName(PersonEntity person)
        => string.IsNullOrEmpty(person.AllianceName)
            ? person.OfficialName
            : $"{person.OfficialName}-{person.AllianceName}";

    private static AddressInformationType BuildDeliveryAddressInformation(PersonEntity person)
    {
        var address = new AddressInformationType()
        {
            AddressLine1 = person.ContactAddressLine1,
            AddressLine2 = person.ContactAddressLine2,
            Street = person.ContactAddressStreet,
            HouseNumber = person.ContactAddressHouseNumber,
            DwellingNumber = person.ContactAddressDwellingNumber,
            PostOfficeBoxText = person.ContactAddressPostOfficeBoxText,
            PostOfficeBoxNumber = (uint?)person.ContactAddressPostOfficeBoxNumber,
            Locality = person.ContactAddressLocality,
            Town = person.ContactAddressTown,
            Country = new Ech0010_6_0.CountryType
            {
                CountryIdIso2 = person.Country,
                CountryNameShort = person.CountryNameShort,
            },
        };

        if (int.TryParse(person.ContactAddressZipCode, out var zip) && !person.IsSwissAbroad)
        {
            address.SwissZipCode = (uint?)zip;
        }
        else
        {
            address.ForeignZipCode = person.ContactAddressZipCode;
        }

        return address;
    }

    private static AddressInformationType BuildElectoralAddressInformation(PersonEntity person)
    {
        var address = new AddressInformationType()
        {
            AddressLine1 = person.ResidenceAddressExtensionLine1,
            AddressLine2 = person.ResidenceAddressExtensionLine2,
            Street = person.ResidenceAddressStreet,
            HouseNumber = person.ResidenceAddressHouseNumber,
            DwellingNumber = person.ResidenceAddressDwellingNumber,
            Town = PlaceholderIfEmptyOrNull(person.ResidenceAddressTown ?? person.ContactAddressTown),
            Country = new Ech0010_6_0.CountryType
            {
                CountryIdIso2 = person.Country,
                CountryNameShort = person.CountryNameShort,
            },
            PostOfficeBoxText = person.ResidenceAddressPostOfficeBoxText,
        };

        if (int.TryParse(person.ResidenceAddressZipCode, out var zip) && !person.IsSwissAbroad)
        {
            address.SwissZipCode = (uint?)zip;
        }
        else
        {
            address.ForeignZipCode = person.ResidenceAddressZipCode;
        }

        return address;
    }

    private PersonMailAddressType? BuildDeliveryAddress(PersonEntity person)
    {
        if (string.IsNullOrWhiteSpace(person.ContactAddressTown))
        {
            _logger.LogDebug("Skip building delivery address because person with id {PersonId} has no delivery address set.", person.Id);
            return null;
        }

        var personAddress = new PersonMailAddressType
        {
            Person = new()
            {
                MrMrs = person.Sex.ToEchMrMrs(),
                FirstName = CallNameOrFirstName(person).Truncate(30),
                LastName = LastName(person).Truncate(30),
            },
            AddressInformation = BuildDeliveryAddressInformation(person),
        };

        return personAddress;
    }

    private VotingPersonTypePerson BuildPerson(PersonEntity person)
    {
        var language = person.LanguageOfCorrespondence?.ToEchLanguage() ?? LanguageType.De;

        var personIdentification = new PersonIdentificationType
        {
            Vn = (ulong?)person.Vn,
            LocalPersonId = BuildLocalPersonId(person),
            OfficialName = LastName(person),
            FirstName = CallNameOrFirstName(person),
            OriginalName = person.OriginalName,
            Sex = person.Sex.ToEchSexType(),
            DateOfBirth = person.DateOfBirth.ToEchDatePartiallyKnown(),
        };

        foreach (var otherPersonId in BuildOtherPersonIds(person))
        {
            personIdentification.OtherPersonId.Add(otherPersonId);
        }

        var swissMunicipality = new SwissMunicipalityType
        {
            MunicipalityId = person.MunicipalityId,
            MunicipalityName = person.MunicipalityName ?? throw new ArgumentException($"The person with id {person.Id} is missing the municipality name.", nameof(person)),
        };

        // Process foreigner person (Ausländische Person mit aktivem Stimm- und Wahlrecht)
        if (person.Country?.Equals(SwissCountryIdIso2, StringComparison.OrdinalIgnoreCase) == false)
        {
            return new VotingPersonTypePerson
            {
                Foreigner = new ForeignerType
                {
                    ForeignerPerson = new ForeignerPersonType
                    {
                        PersonIdentification = personIdentification,
                        LanguageOfCorrespondance = language,
                    },
                    Municipality = swissMunicipality,
                },
            };
        }

        // Process swiss person (Person mit Schweizer Pass)
        var swissPerson = new SwissPersonType
        {
            PersonIdentification = personIdentification,
            LanguageOfCorrespondance = language,
        };

        foreach (var placeOfOrigin in BuildPlaceOfOrigins(person))
        {
            swissPerson.PlaceOfOrigin.Add(placeOfOrigin);
        }

        if (!person.IsSwissAbroad)
        {
            if (person.SendVotingCardsToDomainOfInfluenceReturnAddress)
            {
                swissPerson.Extension = new SwissPersonExtension
                {
                    SendVotingCardsToDomainOfInfluenceReturnAddress = true,
                };
            }

            return new VotingPersonTypePerson { Swiss = new SwissDomesticType { SwissDomesticPerson = swissPerson, Municipality = swissMunicipality } };
        }

        // Process swiss abroad (Stimmberechtige Auslandschweizerinnen und Auslandschweizer)
        swissPerson.Extension = new SwissPersonExtension
        {
            Address = new SwissAbroadPersonExtensionAddress
            {
                Line1 = person.ContactAddressLine1 ?? string.Empty,
                Line2 = person.ContactAddressLine2 ?? string.Empty,
                Line3 = person.ContactAddressLine3 ?? string.Empty,
                Line4 = person.ContactAddressLine4 ?? string.Empty,
                Line5 = person.ContactAddressLine5 ?? string.Empty,
                Line6 = person.ContactAddressLine6 ?? string.Empty,
                Line7 = person.ContactAddressLine7 ?? string.Empty,
            },
            PostageCode = person.ContactAddressZipCode ?? string.Empty,
            VotingPlace = BuildVotingPlace(person.MunicipalityId),
        };

        var residenceCountryInfo = _countryHelperService.GetCountryInfo(person.ResidenceCountry!);

        return new VotingPersonTypePerson
        {
            SwissAbroad = new SwissAbroadType
            {
                SwissAbroadPerson = swissPerson,
                DateOfRegistration =
                    person.MoveInArrivalDate?.ToDateTime(TimeOnly.MinValue) ?? person.CreatedDate,
                ResidenceCountry = new Ech0008_3_0.CountryType
                {
                    CountryId = (ushort?)residenceCountryInfo?.Id,
                    CountryIdIso2 = residenceCountryInfo?.Iso2Id,
                    CountryNameShort = residenceCountryInfo?.ShortNameEn ??
                                       person.ResidenceCountry ?? ResidenceCountryUnknown,
                },
                Municipality = swissMunicipality,
            },
        };
    }

    private SwissAbroadPersonExtensionVotingPlace? BuildVotingPlace(int municipalityId)
        => _config.VotingPlacesByBfs.GetValueOrDefault(municipalityId.ToString());
}
