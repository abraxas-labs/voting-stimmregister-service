// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Mapping;

/// <summary>
/// AutoMapper Profile for person model mappings between:
/// <list type="bullet">
///     <item><see cref="PersonEntity"/></item>
///     <item><see cref="PersonCsvExportModel"/></item>
/// </list>
/// </summary>
public class PersonCsvExportProfile : Profile
{
    public const string DefaultYes = "Ja";
    public const string DefaultNo = "Nein";

    public PersonCsvExportProfile()
    {
        CreateMap<PersonEntity, PersonCsvExportModel>()
            .ForMember(dest => dest.DateOfBirthAdjusted, opt => opt.MapFrom(src => src.DateOfBirthAdjusted.Equals(true) ? DefaultYes : DefaultNo))
            .ForMember(dest => dest.SendVotingCardsToDomainOfInfluenceReturnAddress, opt => opt.MapFrom(src => src.SendVotingCardsToDomainOfInfluenceReturnAddress ? DefaultYes : DefaultNo))
            .ForMember(dest => dest.MoveInUnknown, opt => opt.MapFrom(src => src.MoveInUnknown.Equals(true) ? DefaultYes : DefaultNo))
            .ForMember(dest => dest.Religion, opt => opt.MapFrom(src => ConvertReligion(src.Religion)))
            .ForMember(dest => dest.ReligionCode, opt => opt.MapFrom(src => GetEnumValueReligion(src.Religion)))
            .ForMember(dest => dest.ResidencePermit, opt => opt.MapFrom(src => ConvertResidencePermit(src.ResidencePermit)))
            .ForMember(dest => dest.ResidencePermitCode, opt => opt.MapFrom(src => src.ResidencePermit))
            .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => ConvertSex(src.Sex)))
            .ForMember(dest => dest.SexCode, opt => opt.MapFrom(src => GetEnumValueSex(src.Sex)))
            .ForMember(dest => dest.EVoting, opt => opt.MapFrom(src => src.EVoting ? DefaultYes : DefaultNo))
            .ForMember(dest => dest.SwissCitizenship, opt => opt.MapFrom(src => ConvertSwissCitizenship(src.Country)))
            .ForMember(dest => dest.TypeOfResidence, opt => opt.MapFrom(src => ConvertResidence(src.TypeOfResidence)))
            .ForMember(dest => dest.TypeOfResidenceCode, opt => opt.MapFrom(src => GetEnumValueResidence(src.TypeOfResidence)))
            .ForMember(dest => dest.Vn, opt => opt.MapFrom(src => ConvertVn(src.Vn)))
            .ForMember(dest => dest.IsHouseholder, opt => opt.MapFrom(src => src.IsHouseholder ? DefaultYes : DefaultNo))
            .AfterMap((src, dest) =>
            {
                if (src.PersonDois != null)
                {
                    MapCircleDois(dest, src.PersonDois);
                    MapCircleCtBzDoi(dest, src.PersonDois, DomainOfInfluenceType.Ct);
                    MapCircleCtBzDoi(dest, src.PersonDois, DomainOfInfluenceType.Bz);
                    MapOriginDois(dest, src.PersonDois);
                }
            });
    }

    private static void MapCircleDois(PersonCsvExportModel dest, ICollection<PersonDoiEntity> srcPersonDois)
    {
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.Ch, id => dest.ConfederationCircleId = id, name => dest.ConfederationCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.Mu, id => dest.MunicipalityCircleId = id, name => dest.MunicipalityCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.Sk, id => dest.PoliticalCircleId = id, name => dest.PoliticalCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.KiKat, id => dest.CatholicCircleId = id, name => dest.CatholicCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.KiEva, id => dest.EvangelicCircleId = id, name => dest.EvangelicCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.Sc, id => dest.SchoolCircleId = id, name => dest.SchoolCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.AnVek, id => dest.TrafficCircleId = id, name => dest.TrafficCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.AnWok, id => dest.ResidentialDistrictCircleId = id, name => dest.ResidentialDistrictCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.AnVok, id => dest.PeopleCircleId = id, name => dest.PeopleCircleName = name);
        MapCircleDoi(srcPersonDois, DomainOfInfluenceType.Ko, id => dest.CorporationsCircleId = id, name => dest.CorporationsCircleName = name);
    }

    private static void MapCircleCtBzDoi(PersonCsvExportModel dest, ICollection<PersonDoiEntity> srcPersonDois, DomainOfInfluenceType type)
    {
        const int maxOriginCount = 2;
        var originPersonDois = srcPersonDois
            .Where(srcPersonDois => srcPersonDois.DomainOfInfluenceType == type)
            .OrderBy(srcPersonDois => srcPersonDois.Name)
            .Concat(new PersonDoiEntity[maxOriginCount])
            .ToArray();

        if (type == DomainOfInfluenceType.Ct)
        {
            (dest.CantonCircleId1, dest.CantonCircleName1) = GetCantonDistrictDoiOriginNames(originPersonDois[0]);
            (dest.CantonCircleId2, dest.CantonCircleName2) = GetCantonDistrictDoiOriginNames(originPersonDois[1]);
        }
        else
        {
            (dest.DistrictCircleId1, dest.DistrictCircleName1) = GetCantonDistrictDoiOriginNames(originPersonDois[0]);
            (dest.DistrictCircleId2, dest.DistrictCircleName2) = GetCantonDistrictDoiOriginNames(originPersonDois[1]);
        }
    }

    private static void MapOriginDois(PersonCsvExportModel dest, ICollection<PersonDoiEntity> srcPersonDois)
    {
        const int maxOriginCount = 7;
        var originPersonDois = srcPersonDois
            .Where(srcPersonDois => srcPersonDois.DomainOfInfluenceType == DomainOfInfluenceType.Og)
            .OrderBy(srcPersonDois => srcPersonDois.Name)
            .Concat(new PersonDoiEntity[maxOriginCount])
            .ToArray();

        (dest.OriginName1, dest.OriginCanton1) = GetPersonDoiOriginNames(originPersonDois[0]);
        (dest.OriginName2, dest.OriginCanton2) = GetPersonDoiOriginNames(originPersonDois[1]);
        (dest.OriginName3, dest.OriginCanton3) = GetPersonDoiOriginNames(originPersonDois[2]);
        (dest.OriginName4, dest.OriginCanton4) = GetPersonDoiOriginNames(originPersonDois[3]);
        (dest.OriginName5, dest.OriginCanton5) = GetPersonDoiOriginNames(originPersonDois[4]);
        (dest.OriginName6, dest.OriginCanton6) = GetPersonDoiOriginNames(originPersonDois[5]);
        (dest.OriginName7, dest.OriginCanton7) = GetPersonDoiOriginNames(originPersonDois[6]);
    }

    private static (string Identifier, string Name) GetCantonDistrictDoiOriginNames(PersonDoiEntity? personDoi)
    {
        var identifier = personDoi?.Identifier ?? string.Empty;
        var name = personDoi?.Name ?? string.Empty;
        return (identifier, name);
    }

    private static (string Name, string Canton) GetPersonDoiOriginNames(PersonDoiEntity? personDoi)
    {
        var name = personDoi?.Name ?? string.Empty;
        var canton = personDoi?.Canton ?? string.Empty;
        return (name, canton);
    }

    private static void MapCircleDoi(IEnumerable<PersonDoiEntity> personDoi, DomainOfInfluenceType type, Action<string?> identifierSetter, Action<string?> nameSetter)
    {
        var circleDoi = personDoi.FirstOrDefault(pd => pd.DomainOfInfluenceType == type);
        identifierSetter(circleDoi?.Identifier ?? string.Empty);
        nameSetter(circleDoi?.Name ?? string.Empty);
    }

    private string ConvertReligion(ReligionType religion)
    {
        return religion switch
        {
            ReligionType.Catholic => "römisch-katholische Kirche",
            ReligionType.ChristCatholic => "christkatholische / altkatholische Kirche",
            ReligionType.Evangelic => "evangelisch-reformierte (protestantische) Kirche",
            ReligionType.Unknown => "Unbekannt",
            _ => "Unbekannt",
        };
    }

    private string ConvertResidence(ResidenceType residenceTypes)
    {
        return residenceTypes switch
        {
            ResidenceType.HWS => "Hauptwohnsitz",
            ResidenceType.NWS => "Nebenwohnsitz",
            ResidenceType.Undefined => "Weder Haupt- noch Nebenwohnsitz",
            _ => "Unbekannt",
        };
    }

    private string ConvertResidencePermit(string? residencePermit)
    {
        return residencePermit switch
        {
            "01" => "Saisonarbeiterin / Saisonarbeiter",
            "02" => "Aufenthalterin / Aufenthalter",
            "03" => "Niedergelassene / Niedergelassener",
            "04" => "Erwerbstätige Ehepartnerin / erwerbstätiger Ehepartner und Kinder von Angehörigen ausländischer Vertretungen oder staatlichen internationalen Organisationen",
            "05" => "Vorläufig Aufgenommene / vorläufig Aufgenommener",
            "06" => "Grenzgängerin / Grenzgänger",
            "07" => "Kurzaufenthalterin / Kurzaufenthalter",
            "08" => "Asylsuchende / Asylsuchender",
            "09" => "Schutzbedürftige / Schutzbedürftiger",
            "10" => "Meldepflichtige / Meldepflichtiger bei ZEMIS (Zentrales Migrationssystem)",
            "11" => "Diplomatin / Diplomat und internationale Funktionärin / internationaler Funktionär mit diplomatischer Immunität",
            "12" => "Internationale Funktionärin / internationaler Funktionär ohne diplomatische Immunität",
            "13" => "Nicht zugeteilt",
            _ => "Unbekannt / " + residencePermit,
        };
    }

    private string ConvertSex(SexType sex)
    {
        return sex switch
        {
            SexType.Male => "männlich",
            SexType.Female => "weiblich",
            SexType.Undefined => "unbestimmt",
            _ => "unbestimmt",
        };
    }

    private string ConvertSwissCitizenship(string? country)
    {
        if (country == null)
        {
            return "Nein";
        }

        if (country.Equals("CH"))
        {
            return "Ja";
        }
        else
        {
            return "Nein";
        }
    }

    private string ConvertVn(long? nullableVn)
    {
        if (nullableVn is not long vn)
        {
            return string.Empty;
        }

        var ahvn13str = vn.ToString();
        return ahvn13str.Substring(0, 3) + "." + ahvn13str.Substring(3, 4) + "." + ahvn13str.Substring(7, 4) + "." + ahvn13str.Substring(11, 2);
    }

    private int GetEnumValueReligion(ReligionType religion)
    {
        return religion switch
        {
            ReligionType.Catholic => (int)ReligionType.Catholic,
            ReligionType.ChristCatholic => (int)ReligionType.ChristCatholic,
            ReligionType.Evangelic => (int)ReligionType.Evangelic,
            _ => (int)ReligionType.Unknown,
        };
    }

    private int GetEnumValueResidence(ResidenceType residenceTypes)
    {
        return residenceTypes switch
        {
            ResidenceType.HWS => (int)ResidenceType.HWS,
            ResidenceType.NWS => (int)ResidenceType.NWS,
            ResidenceType.Undefined => (int)ResidenceType.Undefined,
            _ => (int)ResidenceType.Undefined,
        };
    }

    private int GetEnumValueSex(SexType sex)
    {
        return sex switch
        {
            SexType.Male => (int)SexType.Male,
            SexType.Female => (int)SexType.Female,
            SexType.Undefined => (int)SexType.Undefined,
            _ => (int)SexType.Undefined,
        };
    }
}
