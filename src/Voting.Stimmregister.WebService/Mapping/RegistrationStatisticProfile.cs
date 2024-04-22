// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.RegistrationStatistic;

namespace Voting.Stimmregister.WebService.Mapping;

/// <summary>
/// AutoMapper Profile for import statistic model mappings between:
/// <list type="bullet">
///     <item><see cref="Proto.V1.Services.Models.ImportStatisticModel"/></item>
///     <item><see cref="ImportStatisticEntity"/></item>
/// </list>
/// </summary>
public class RegistrationStatisticProfile : Profile
{
    public RegistrationStatisticProfile()
    {
        CreateMap<RegistrationStatisticResponseModel, Proto.V1.Services.Responses.ListRegistrationStatisticResponse>()
            .ForMember(dest => dest.MunicipalityRegistrationStatistics, opts => opts.MapFrom(src => src.MunicipalityStatistics))
            .ForMember(dest => dest.TotalRegistrationStatistic, opts => opts.MapFrom(src => src.TotalStatistic))
            .ForMember(dest => dest.IsTopLevelAuthority, opts => opts.MapFrom(src => src.IsTopLevelAuthority));

        CreateMap<BfsStatisticEntity, Proto.V1.Services.Models.MunicipalityRegistrationStatisticModel>()
            .ForMember(dest => dest.MunicipalityId, opts => opts.MapFrom(src => src.Bfs))
            .ForMember(dest => dest.MunicipalityName, opts => opts.MapFrom(src => src.BfsName))
            .ForMember(dest => dest.RegistrationStatistic, opts => opts.MapFrom(src => src));

        CreateMap<BfsStatisticEntity, Proto.V1.Services.Models.RegistrationStatisticModel>()
            .ForMember(dest => dest.EvoterShare, opts => opts.MapFrom(src => CalculateShare(src)));
    }

    private float CalculateShare(BfsStatisticEntity entity)
    {
        if (entity.VoterTotalCount == 0)
        {
            return 0;
        }

        return (float)entity.EVoterTotalCount / entity.VoterTotalCount * 100;
    }
}
