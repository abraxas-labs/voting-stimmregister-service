// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using Voting.Stimmregister.WebService.Mapping.Converter;

namespace Voting.Stimmregister.WebService.Mapping;

/// <summary>
/// AutoMapper Profile for person model mappings between:
/// <list type="bullet">
///     <item><see cref="Proto.V1.Services.Models.PersonModel"/></item>
///     <item><see cref="PersonModel"/></item>
///     <item><see cref="PersonEntity"/></item>
/// </list>
/// </summary>
public class PersonProfile : Profile
{
    public PersonProfile()
    {
        CreateMap<PersonEntity, PersonEntityModel>()
            .ForMember(dest => dest.IsVotingAllowed, opt => opt.Ignore())
            .ForMember(dest => dest.IsBirthDateValidForVotingRights, opt => opt.Ignore())
            .ForMember(dest => dest.IsNationalityValidForVotingRights, opt => opt.Ignore())
            .ForMember(dest => dest.Actuality, opt => opt.Ignore())
            .ForMember(dest => dest.ActualityDate, opt => opt.Ignore());

        CreateMap<IEnumerable<string>, Proto.V1.Services.Models.ValidationResultModel>()
            .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src));

        // Domain to Proto
        CreateMap<PersonEntityModel, Proto.V1.Services.Responses.PersonServiceGetSingleResponse>()
            .ForMember(dst => dst.Latest, opt => opt.MapFrom(src => src))
            .ForMember(dst => dst.LatestDomainOfInfluences, opt => opt.MapFrom(src => src.PersonDois));

        CreateMap<string, Proto.V1.Services.Models.Canton>().ConvertUsing((value, _) =>
        {
            if (Enum.TryParse(typeof(Proto.V1.Services.Models.Canton), value, true, out var canton))
            {
                return (Proto.V1.Services.Models.Canton)canton!;
            }
            else
            {
                return Proto.V1.Services.Models.Canton.Unspecified;
            }
        });

        CreateMap<PersonEntityModel, Proto.V1.Services.Models.PersonModel>()
            .ForMember(
                dest => dest.ValidationErrors!,
                opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.ValidationErrors)
                        ? null!
                        : JsonSerializer.Deserialize<Dictionary<string, string[]>?>(src.ValidationErrors, null as JsonSerializerOptions)!))
            .ForMember(dest => dest.HasValidationErrors, opt => opt.MapFrom(src => !src.IsValid))

            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null)); // Skiping mapping of nullable members
        CreateMap<PersonSearchResultPage<PersonEntityModel>, Proto.V1.Services.Responses.PersonServiceGetAllResponse>()
            .ForMember(dst => dst.TotalCount, opt => opt.MapFrom(src => src.TotalItemsCount))
            .ForMember(dst => dst.People, opt => opt.MapFrom(src => src.Items));
        CreateMap<LastSearchParameterEntity, PersonServiceGetLastUsedParametersResponse>()
            .ForMember(dst => dst.Criteria, opts => opts.MapFrom(src => src.FilterCriteria));
        CreateMap(typeof(PersonSearchResultPage<>), typeof(PersonSearchResultPage<>))
            .ConvertUsing(typeof(PersonSearchResultPageConverter<,>));

        // Proto to Domain
        CreateMap<Proto.V1.Services.Models.PagingModel, Pageable>()
            .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.PageIndex + 1))
            .ReverseMap()
            .ForMember(dst => dst.PageIndex, opt => opt.MapFrom(src => src.Page - 1));
        CreateMap<PersonServiceGetSingleRequest, PersonSearchSingleParametersModel>();
        CreateMap<PersonServiceGetAllRequest, PersonSearchParametersModel>();
        CreateMap<PersonServiceGetByFilterIdRequest, PersonSearchFilterIdParametersModel>();
        CreateMap<PersonServiceGetByFilterVersionIdRequest, PersonSearchFilterIdParametersModel>()
            .ForMember(dest => dest.FilterId, opt => opt.Ignore());
        CreateMap<FilterCriteriaModel, PersonSearchFilterCriteriaModel>()
            .ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.ReferenceId))
            .ForMember(dest => dest.FilterValue, opt => opt.MapFrom(src => src.FilterValue))
            .ForMember(dest => dest.FilterOperator, opt => opt.MapFrom(src => src.FilterOperator))
            .ForMember(dest => dest.FilterType, opt => opt.MapFrom(src => src.FilterDataType))
            .ForAllOtherMembers(opt => opt.Ignore());
        CreateMap<PersonSearchFilterCriteriaModel, FilterCriteriaEntity>()
            .ForMember(dst => dst.FilterOperator, opts => opts.MapFrom(src => src.FilterOperator))
            .ForMember(dst => dst.ReferenceId, opts => opts.MapFrom(src => src.ReferenceId))
            .ForMember(dst => dst.FilterType, opts => opts.MapFrom(src => src.FilterType))
            .ForMember(dst => dst.FilterValue, opts => opts.MapFrom(src => src.FilterValue))
            .ForAllOtherMembers(opt => opt.Ignore());
    }
}
