// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.WebService.Mapping.Converter;

namespace Voting.Stimmregister.WebService.Mapping;

/// <summary>
/// AutoMapper Profile for type conversions between:
/// <list type="bullet">
///     <item><see cref="DateTime"/> and <see cref="Timestamp"/></item>
///     <item><see cref="Guid"/> and <see cref="string"/></item>
/// </list>
/// </summary>
public class ConverterProfile : Profile
{
    public ConverterProfile()
    {
        CreateMap<Timestamp, DateTime>().ConvertUsing<ProtoTimestampConverter>();
        CreateMap<Timestamp, DateTime?>().ConvertUsing<ProtoTimestampConverter>();
        CreateMap<Timestamp, DateOnly>().ConvertUsing<ProtoTimestampConverter>();
        CreateMap<Timestamp, DateOnly?>().ConvertUsing<ProtoTimestampConverter>();
        CreateMap<DateTime, Timestamp>().ConvertUsing<ProtoTimestampConverter>();
        CreateMap<DateTime?, Timestamp?>().ConvertUsing<ProtoTimestampConverter>();
        CreateMap<DateOnly, Timestamp>().ConvertUsing<ProtoTimestampConverter>();
        CreateMap<DateOnly?, Timestamp?>().ConvertUsing<ProtoTimestampConverter>();

        CreateMap<DateTime, DateOnly>().ConvertUsing<DateTimeDateOnlyConverter>();
        CreateMap<DateTime?, DateOnly?>().ConvertUsing<DateTimeDateOnlyConverter>();
        CreateMap<DateOnly, DateTime>().ConvertUsing<DateTimeDateOnlyConverter>();
        CreateMap<DateOnly?, DateTime?>().ConvertUsing<DateTimeDateOnlyConverter>();

        CreateMap<Guid?, string>().ConvertUsing<GuidStringConverter>();
        CreateMap<Guid, string>().ConvertUsing<GuidStringConverter>();
        CreateMap<string, Guid?>().ConvertUsing<GuidStringConverter>();
        CreateMap<string, Guid>().ConvertUsing<GuidStringConverter>();

        CreateMap(typeof(Page<>), typeof(Page<>)).ConvertUsing(typeof(PageConverter<,>));
    }
}
