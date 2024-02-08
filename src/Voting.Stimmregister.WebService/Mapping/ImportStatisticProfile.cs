// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.WebService.Mapping;

/// <summary>
/// AutoMapper Profile for import statistic model mappings between:
/// <list type="bullet">
///     <item><see cref="Proto.V1.Services.Models.ImportStatisticModel"/></item>
///     <item><see cref="ImportStatisticEntity"/></item>
/// </list>
/// </summary>
public class ImportStatisticProfile : Profile
{
    public ImportStatisticProfile()
    {
        // Domain to Proto
        CreateMap<ImportStatisticEntity, Proto.V1.Services.Models.ImportStatisticModel>()
            .ForMember(dest => dest.MunicipalityId, opt => opt.NullSubstitute(0))
            .ForMember(dest => dest.MunicipalityName, opt => opt.NullSubstitute(string.Empty))
            .ForMember(dest => dest.ProcessingErrors, opt => opt.NullSubstitute(string.Empty))
            .ForMember(dest => dest.ImportType, opt => opt.MapFrom(src => Enum.GetName(src.ImportType)))
            .ForMember(dest => dest.TotalElapsedMilliseconds, opt => opt.MapFrom(src => src.TotalElapsedMilliseconds == null ? "0" : src.TotalElapsedMilliseconds.ToString()))
            .ForMember(dest => dest.RecordValidationErrors, opt => opt.MapFrom(src =>
                src.RecordValidationErrors == null
                    ? new()
                    : JsonSerializer.Deserialize<List<RecordValidationErrorModel>>(src.RecordValidationErrors, new JsonSerializerOptions())));

        CreateMap<RecordValidationErrorModel, Proto.V1.Services.Models.RecordValidationErrorModel>();
        CreateMap<FieldValidationErrorModel, Proto.V1.Services.Models.FieldValidationErrorModel>();

        CreateMap<Page<ImportStatisticEntity>, Proto.V1.Services.Responses.ListImportStatisticsResponse>()
            .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(x => x.TotalItemsCount))
            .ForMember(dest => dest.ImportStatistics, opt => opt.MapFrom(x => x.Items));
        CreateMap<Page<ImportStatisticEntity>, Proto.V1.Services.Responses.GetImportStatisticHistoryResponse>()
            .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(x => x.TotalItemsCount))
            .ForMember(dest => dest.ImportStatistics, opt => opt.MapFrom(x => x.Items));

        CreateMap<ImportSourceSystem, Proto.V1.Services.Models.ImportSourceSystem>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());
        CreateMap<ImportStatus, Proto.V1.Services.Models.ImportStatus>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());
        CreateMap<ImportType, Proto.V1.Services.Models.ImportType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());

        CreateMap<Proto.V1.Services.Models.ImportSourceSystem, ImportSourceSystem>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());
        CreateMap<Proto.V1.Services.Models.ImportStatus, ImportStatus>()
            .ConvertUsingEnumMapping(opt => opt.MapByName(true))
            .ReverseMap();

        CreateMap<Proto.V1.Services.Models.ImportType, ImportType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());
    }
}
