// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AbxVoting_1_5;
using Ech0006_3_0;
using Ech0007_6_0;
using Ech0010_8_0;
using Ech0011_9_0;
using Ech0021_8_0;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Core.Import.Innosolv.Utils;
using AbxVoting10Deserializer = Voting.Lib.Ech.AbxVoting_1_0.Converter.AbxVotingDeserializer;
using AbxVoting15Deserializer = Voting.Lib.Ech.AbxVoting_1_5.Converter.AbxVotingDeserializer;

namespace Voting.Stimmregister.Core.Import.Innosolv.Services;

internal class InnosolvXmlReader : IImportRecordReader<PersonInfoType>
{
    private const int MaxBufferSize = 50 * 1024; // This should be enough to read at least one person
    private readonly BufferedReadStream _stream;
    private readonly AbxVoting10Deserializer _deserializer10;
    private readonly AbxVoting15Deserializer _deserializer15;

    public InnosolvXmlReader(Stream stream, AbxVoting10Deserializer deserializer10, AbxVoting15Deserializer deserializer15)
    {
        _stream = new BufferedReadStream(stream, MaxBufferSize);
        _deserializer10 = deserializer10;
        _deserializer15 = deserializer15;
    }

    public async IAsyncEnumerable<PersonInfoType> ReadRecords()
    {
        await using var asyncVoters = _deserializer15.ReadVoters(_stream, CancellationToken.None).GetAsyncEnumerator();

        var isAbxVoting10 = false;
        try
        {
            await asyncVoters.MoveNextAsync();
        }
        catch
        {
            isAbxVoting10 = true;
        }

        if (isAbxVoting10)
        {
            _stream.SeekToStartOfBuffer();
            await foreach (var voter in _deserializer10.ReadVoters(_stream, CancellationToken.None))
            {
                yield return Convert10PersonTo15(voter);
            }

            yield break;
        }

        _stream.StopBuffering();
        yield return asyncVoters.Current;

        while (await asyncVoters.MoveNextAsync())
        {
            yield return asyncVoters.Current;
        }
    }

    public ValueTask DisposeAsync()
    {
        _stream.Dispose();
        return ValueTask.CompletedTask;
    }

    private PersonInfoType Convert10PersonTo15(AbxVoting_1_0.PersonInfoType person)
    {
        var secondaryResidence = person.HasSecondaryResidence == null
            ? null
            : new PersonInfoTypeHasSecondaryResidence
            {
                ArrivalDate = person.HasSecondaryResidence.ArrivalDate,
                DepartureDate = person.HasSecondaryResidence.DepartureDate,
                ComesFrom = MapDestinationType(person.HasSecondaryResidence.ComesFrom),
                DwellingAddress = MapDwellingAddress(person.HasSecondaryResidence.DwellingAddress),
                MainResidence = person.HasSecondaryResidence.MainResidence,
                ReportingMunicipality = person.HasSecondaryResidence.ReportingMunicipality,
            };
        var partnerIdOrganisation = person.ContactData?.PartnerIdOrganisation == null
            ? null
            : new PartnerIdOrganisationType
            {
                LocalPersonId = person.ContactData.PartnerIdOrganisation.LocalPersonId,
                OtherPersonId = person.ContactData.PartnerIdOrganisation.OtherPersonId,
            };

        return new PersonInfoType
        {
            PersonIdentification = person.PersonIdentification,
            DispatchLockVotingCard = person.DispatchLockVotingCard,
            ContactData = person.ContactData == null ? null : new ContactDataType
            {
                PersonIdentification = person.ContactData.PersonIdentification,
                ContactAddress = MapMailAddress(person.ContactData.ContactAddress),
                ContactValidFrom = person.ContactData.ContactValidFrom,
                ContactValidTill = person.ContactData.ContactValidTill,
                PartnerIdOrganisation = partnerIdOrganisation,
                PersonIdentificationPartner = person.ContactData.PersonIdentificationPartner,
            },
            District = person.District?.ConvertAll(x => new DistrictType
            {
                DistrictCategory = x.DistrictCategory,
                DistrictName = x.DistrictName,
                LocalDistrictId = x.LocalDistrictId,
                OtherDistrictId = x.OtherDistrictId,
            }),
            HasMainResidence = person.HasMainResidence == null ? null : new ResidenceType
            {
                ArrivalDate = person.HasMainResidence.ArrivalDate,
                DepartureDate = person.HasMainResidence.DepartureDate,
                ComesFrom = MapDestinationType(person.HasMainResidence.ComesFrom),
                DwellingAddress = MapDwellingAddress(person.HasMainResidence.DwellingAddress),
                ReportingMunicipality = person.HasMainResidence.ReportingMunicipality,
            },
            HasSecondaryResidence = secondaryResidence,
            NameData = person.NameData == null ? null : new NameDataType
            {
                AliasName = person.NameData.AliasName,
                AllianceName = person.NameData.AllianceName,
                CallName = person.NameData.CallName,
                FirstName = person.NameData.FirstName,
                OfficialName = person.NameData.OfficialName,
                OriginalName = person.NameData.OriginalName,
                OtherName = person.NameData.OtherName,
                NameOnForeignPassport = MapForeignerName(person.NameData.NameOnForeignPassport),
                DeclaredForeignName = MapForeignerName(person.NameData.DeclaredForeignName),
            },
            NationalityData = person.NationalityData == null
                ? null
                : new NationalityDataType
                {
                    NationalityStatus = (NationalityStatusType)person.NationalityData.NationalityStatus,
                    CountryInfo = person.NationalityData.CountryInfo?.ConvertAll(x => new CountryInfoType
                    {
                        Country = x.Country,
                        NationalityValidFrom = x.NationalityValidFrom,
                    }),
                },
            PersonAdditionalData = person.PersonAdditionalData == null
                ? null
                : new PersonAdditionalData
                {
                    LanguageOfCorrespondance = person.PersonAdditionalData.LanguageOfCorrespondance,
                    MrMrs = (MrMrsType?)person.PersonAdditionalData.MrMrs,
                    Title = person.PersonAdditionalData.Title,
                },
            PlaceOfOrigin = person.PlaceOfOrigin?.ConvertAll(x => new PlaceOfOriginType
            {
                Canton = (CantonAbbreviationType)x.Canton,
                HistoryMunicipalityId = x.HistoryMunicipalityId,
                OriginName = x.OriginName,
                PlaceOfOriginId = x.PlaceOfOriginId,
            }),
            PoliticalRightData = person.PoliticalRightData == null ? null : new PoliticalRightDataType
            {
                RestrictedVotingAndElectionRightFederation = person.PoliticalRightData.RestrictedVotingAndElectionRightFederation,
            },
            ReligionData = person.ReligionData == null ? null : new ReligionDataType
            {
                Religion = person.ReligionData.Religion,
                ReligionValidFrom = person.ReligionData.ReligionValidFrom,
            },
            ResidencePermit = person.ResidencePermit == null ? null : new ResidencePermitDataType
            {
                ResidencePermit = (ResidencePermitType?)person.ResidencePermit.ResidencePermit,
                EntryDate = person.ResidencePermit.EntryDate,
                ResidencePermitValidFrom = person.ResidencePermit.ResidencePermitValidFrom,
                ResidencePermitValidTill = person.ResidencePermit.ResidencePermitValidTill,
            },
        };
    }

    private MailAddressType? MapMailAddress(Ech0010_5_1.MailAddressType? address)
    {
        if (address == null)
        {
            return null;
        }

        return new MailAddressType
        {
            AddressInformation = MapAddress(address.AddressInformation),
            Organisation = address.Organisation == null ? null : new OrganisationMailAddressInfoType
            {
                FirstName = address.Organisation.FirstName,
                MrMrs = (MrMrsType?)address.Organisation.MrMrs,
                Title = address.Organisation.Title,
                LastName = address.Organisation.LastName,
                OrganisationName = address.Organisation.OrganisationName,
                OrganisationNameAddOn1 = address.Organisation.OrganisationNameAddOn1,
                OrganisationNameAddOn2 = address.Organisation.OrganisationNameAddOn2,
            },
            Person = address.Person == null ? null : new PersonMailAddressInfoType
            {
                FirstName = address.Person.FirstName,
                MrMrs = (MrMrsType?)address.Person.MrMrs,
                Title = address.Person.Title,
                LastName = address.Person.LastName,
            },
        };
    }

    private DestinationType? MapDestinationType(Ech0011_8_1.DestinationType? destination)
    {
        if (destination == null)
        {
            return null;
        }

        return new DestinationType
        {
            ForeignCountry = destination.ForeignCountry == null ? null : new GeneralPlaceTypeForeignCountry
            {
                Country = destination.ForeignCountry.Country,
                Town = destination.ForeignCountry.Town,
            },
            MailAddress = destination.MailAddress == null ? null : MapAddress(destination.MailAddress),
            SwissTown = destination.SwissTown == null ? null : new SwissMunicipalityType
            {
                HistoryMunicipalityId = destination.SwissTown.HistoryMunicipalityId,
                CantonAbbreviation = (CantonAbbreviationType?)destination.SwissTown.CantonAbbreviation,
                MunicipalityId = destination.SwissTown.MunicipalityId,
                MunicipalityName = destination.SwissTown.MunicipalityName,
            },
            Unknown = (UnknownType?)destination.Unknown,
        };
    }

    private AddressInformationType? MapAddress(Ech0010_5_1.AddressInformationType? address)
    {
        if (address == null)
        {
            return null;
        }

        return new AddressInformationType
        {
            Country = MapCountry(address.Country),
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            DwellingNumber = address.DwellingNumber,
            ForeignZipCode = address.ForeignZipCode,
            HouseNumber = address.HouseNumber,
            Locality = address.Locality,
            PostOfficeBoxNumber = address.PostOfficeBoxNumber,
            PostOfficeBoxText = address.PostOfficeBoxText,
            Street = address.Street,
            SwissZipCode = address.SwissZipCode,
            SwissZipCodeAddOn = address.SwissZipCodeAddOn,
            SwissZipCodeId = address.SwissZipCodeId,
            Town = address.Town,
        };
    }

    private DwellingAddressType? MapDwellingAddress(Ech0011_8_1.DwellingAddressType? address)
    {
        if (address == null)
        {
            return null;
        }

        var swissAddress = address.Address == null
            ? null
            : new SwissAddressInformationType
            {
                Country = MapCountry(address.Address.Country),
                AddressLine1 = address.Address.AddressLine1,
                AddressLine2 = address.Address.AddressLine2,
                DwellingNumber = address.Address.DwellingNumber,
                HouseNumber = address.Address.HouseNumber,
                Locality = address.Address.Locality,
                Street = address.Address.Street,
                SwissZipCode = address.Address.SwissZipCode,
                SwissZipCodeAddOn = address.Address.SwissZipCodeAddOn,
                SwissZipCodeId = address.Address.SwissZipCodeId,
                Town = address.Address.Town,
            };

        return new DwellingAddressType
        {
            Address = swissAddress,
            Egid = address.Egid,
            Ewid = address.Ewid,
            HouseholdId = address.HouseholdId,
            MovingDate = address.MovingDate,
            TypeOfHousehold = (TypeOfHouseholdType)address.TypeOfHousehold,
        };
    }

    private ForeignerNameType? MapForeignerName(Ech0011_8_1.ForeignerNameType? name)
    {
        return name == null
            ? null
            : new ForeignerNameType
            {
                FirstName = name.FirstName,
                Name = name.Name,
            };
    }

    private CountryType? MapCountry(string? country)
    {
        return string.IsNullOrEmpty(country)
            ? null
            : new CountryType { CountryIdIso2 = country };
    }
}
