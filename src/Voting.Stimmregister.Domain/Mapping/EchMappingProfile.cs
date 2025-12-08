// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Domain.Mapping;

/// <summary>
/// AutoMapper Profile for eCH-model mappings between.
/// </summary>
public class EchMappingProfile : Profile
{
    public EchMappingProfile()
    {
        CreateMap<Ech0072_1_0.CountryType, CountryModel>()
            .ForMember(dest => dest.BfsId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SystemId, opt => opt.Ignore())
            .ForMember(dest => dest.Iso2, opt => opt.MapFrom(src => src.Iso2Id))
            .ForMember(dest => dest.ShortNameDe, opt => opt.MapFrom(src => src.ShortNameDe))
            .ForMember(dest => dest.ShortNameEn, opt => opt.MapFrom(src => src.ShortNameEn));

        CreateMap<LogantoCountryModel, CountryModel>()
            .ForMember(dest => dest.BfsId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SystemId, opt => opt.MapFrom(src => src.LogaId))
            .ForMember(dest => dest.Iso2, opt => opt.MapFrom(src => src.Iso2Id))
            .ForMember(dest => dest.ShortNameDe, opt => opt.MapFrom(src => src.ShortNameDe))
            .ForMember(dest => dest.ShortNameEn, opt => opt.Ignore());
    }
}
