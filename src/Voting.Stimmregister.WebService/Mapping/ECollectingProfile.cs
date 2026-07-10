// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using AutoMapper;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using PageInfo = Voting.Stimmregister.Proto.V1.Services.Models.PageInfo;

namespace Voting.Stimmregister.WebService.Mapping;

public class ECollectingProfile : Profile
{
    public ECollectingProfile()
    {
        // proto to domain
        CreateMap<EcollectingServiceGetPersonIdByAhvn13Request, ECollectingPersonSearchByVnParametersModel>()
            .ForMember(
                dst => dst.CantonBfs,
                opts => opts.Condition(x => x.CantonOrMunicipalityCase == EcollectingServiceGetPersonIdByAhvn13Request.CantonOrMunicipalityOneofCase.CantonBfs))
            .ForMember(
                dst => dst.MunicipalityId,
                opts => opts.Condition(x => x.CantonOrMunicipalityCase == EcollectingServiceGetPersonIdByAhvn13Request.CantonOrMunicipalityOneofCase.MunicipalityId));
        CreateMap<EcollectingServiceGetPeopleByIdsRequest, ECollectingPeopleSearchByIdsParametersModel>();

        // do not map empty strings and keep the null value in the target (proto doesn't know null)
        CreateMap<EcollectingServiceGetPeopleByNameRequest, ECollectingPeopleSearchByNameParametersModel>()
            .ForMember(dst => dst.Pageable, opts => opts.MapFrom(x => x.Paging))
            .ForAllMembers(opt =>
                opt.Condition((_, _, srcMember) => srcMember is not string str || !string.IsNullOrEmpty(str)));

        // domain to proto
        CreateMap<ECollectingPersonEntityModel, EcollectingServiceGetPersonIdByAhvn13Response>();
        CreateMap<ECollectingPersonEntityModel, EcollectingServicePersonModel>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
        CreateMap<Page<ECollectingPersonEntityModel>, EcollectingServiceGetPeopleResponse>()
            .ForMember(dest => dest.People, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.PageInfo, opt => opt.MapFrom(src => src));
        CreateMap<List<ECollectingPersonEntityModel>, EcollectingServiceGetPeopleResponse>()
            .ForMember(dest => dest.People, opt => opt.MapFrom(src => src))
            .ForMember(
                dest => dest.PageInfo,
                opts => opts.MapFrom(src => new PageInfo { PageSize = src.Count, PageIndex = 0, TotalCount = src.Count }));
    }
}
