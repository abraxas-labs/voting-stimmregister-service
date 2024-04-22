// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;

namespace Voting.Stimmregister.WebService.Mapping;

/// <summary>
/// AutoMapper Profile for filter model mappings between:
/// <list type="bullet">
///     <item><see cref="Proto.V1.Services.Models.FilterCriteriaModel"/></item>
///     <item><see cref="Domain.Models.FilterCriteriaEntity"/></item>
/// </list>
/// </summary>
public class FilterProfile : Profile
{
    public FilterProfile()
    {
        CreateFilterMaps();
        CreateFilterVersionMaps();
        CreateFilterCriteriaMaps();
        CreateFilterEnumMaps();
    }

    private void CreateFilterEnumMaps()
    {
        CreateMap<FilterDataType, Domain.Enums.FilterDataType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());
        CreateMap<FilterOperator, Domain.Enums.FilterOperatorType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());
        CreateMap<FilterReference, Domain.Enums.FilterReference>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());
    }

    private void CreateFilterMaps()
    {
        CreateMap<FilterEntity, FilterEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.MunicipalityId, opt => opt.MapFrom(src => src.MunicipalityId))
            .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
            .ForMember(dest => dest.LatestVersion, opt => opt.MapFrom(src => src.LatestVersion))
            .ForMember(dest => dest.FilterVersions, opt => opt.MapFrom(src => src.FilterVersions))
            .ForMember(dest => dest.FilterCriterias, opt => opt.MapFrom(src => src.FilterCriterias));

        CreateMap<FilterEntity, FilterDefinitionModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.LatestVersion, opt => opt.MapFrom(src => src.LatestVersion))
            .ForMember(dest => dest.Criteria, opt => opt.MapFrom(src => src.FilterCriterias))
            .ForMember(dest => dest.Versions, opt => opt.MapFrom(src => src.FilterVersions))
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.TenantName ?? string.Empty))
            .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId ?? string.Empty))
            .ReverseMap();

        CreateMap<FilterServiceSaveFilterRequest, FilterEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FilterId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.FilterCriterias, opt => opt.MapFrom(src => src.Criteria))
            .ForMember(dest => dest.MunicipalityId, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.TenantName, opt => opt.Ignore())
            .ForMember(dest => dest.LatestVersion, opt => opt.Ignore())
            .ForMember(dest => dest.FilterVersions, opt => opt.Ignore())
            .ForMember(dest => dest.AuditInfo, opt => opt.Ignore());

        CreateMap<IEnumerable<FilterEntity>, FilterServiceGetAllResponse>()
            .ForMember(dst => dst.Filters, opts => opts.MapFrom(src => src));

        CreateMap<PersonCountsModel, FilterServicePreviewMetadataResponse>();
    }

    private void CreateFilterVersionMaps()
    {
        CreateMap<FilterVersionEntity, FilterVersionModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AuditInfo, opt => opt.MapFrom(src => src.AuditInfo))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.CountOfInvalidPersons, opt => opt.MapFrom(src => src.CountOfInvalidPersons))
            .ForMember(dest => dest.Criteria, opt => opt.MapFrom(src => src.FilterCriterias));

        CreateMap<FilterVersionEntity, FilterServiceGetSingleFilterVersionResponse>()
            .ForMember(dest => dest.Filter, opts => opts.MapFrom(src => src.Filter))
            .ForMember(dest => dest.FilterVersion, opts => opts.MapFrom(src => src));

        CreateMap<FilterServiceCreateFilterVersionRequest, FilterVersionEntity>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.FilterId, opt => opt.MapFrom(src => src.FilterId))
            .ForMember(dest => dest.AuditInfo, opt => opt.Ignore())
            .ForMember(dest => dest.Count, opt => opt.Ignore())
            .ForMember(dest => dest.CountOfInvalidPersons, opt => opt.Ignore())
            .ForMember(dest => dest.FilterVersionPersons, opt => opt.Ignore())
            .ForMember(dest => dest.FilterCriterias, opt => opt.Ignore())
            .ForMember(dest => dest.Filter, opt => opt.Ignore())
            .ForMember(dest => dest.SignatureVersion, opt => opt.Ignore())
            .ForMember(dest => dest.SignatureKeyId, opt => opt.Ignore())
            .ForMember(dest => dest.Signature, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }

    private void CreateFilterCriteriaMaps()
    {
        CreateMap<FilterCriteriaModel, FilterCriteriaEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.ReferenceId))
            .ForMember(dest => dest.FilterType, opt => opt.MapFrom(src => src.FilterDataType))
            .ForMember(dest => dest.FilterOperator, opt => opt.MapFrom(src => src.FilterOperator))
            .ForMember(dest => dest.FilterValue, opt => opt.MapFrom(src => src.FilterValue))
            .ForMember(dest => dest.SortIndex, opt => opt.Ignore())
            .ForMember(dest => dest.FilterId, opt => opt.Ignore())
            .ForMember(dest => dest.FilterVersionId, opt => opt.Ignore())
            .ForMember(dest => dest.LastSearchParameterId, opt => opt.Ignore())
            .ForMember(dest => dest.Filter, opt => opt.Ignore())
            .ForMember(dest => dest.FilterVersion, opt => opt.Ignore())
            .ForMember(dest => dest.LastSearchParameter, opt => opt.Ignore())
            .ForMember(dest => dest.AuditInfo, opt => opt.Ignore())
            .ReverseMap();
    }
}
