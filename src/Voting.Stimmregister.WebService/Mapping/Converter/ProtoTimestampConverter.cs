// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Voting.Stimmregister.WebService.Mapping.Converter;

/// <summary>
/// AutoMapper type converter for conversions between <see cref="DateTime"/> and <see cref="Timestamp"/>.
/// </summary>
public class ProtoTimestampConverter :
    ITypeConverter<Timestamp, DateTime>,
    ITypeConverter<Timestamp, DateTime?>,
    ITypeConverter<Timestamp, DateOnly>,
    ITypeConverter<Timestamp, DateOnly?>,
    ITypeConverter<DateTime, Timestamp>,
    ITypeConverter<DateTime?, Timestamp?>,
    ITypeConverter<DateOnly, Timestamp>,
    ITypeConverter<DateOnly?, Timestamp?>
{
    public DateTime Convert(Timestamp source, DateTime destination, ResolutionContext context)
        => source.ToDateTime();

    public DateTime? Convert(Timestamp? source, DateTime? destination, ResolutionContext context)
        => source?.ToDateTime();

    public DateOnly Convert(Timestamp source, DateOnly destination, ResolutionContext context)
        => DateOnly.FromDateTime(source.ToDateTime());

    public DateOnly? Convert(Timestamp? source, DateOnly? destination, ResolutionContext context)
        => source == null ? null : DateOnly.FromDateTime(source.ToDateTime());

    public Timestamp Convert(DateTime source, Timestamp destination, ResolutionContext context)
        => source.ToTimestamp();

    public Timestamp? Convert(DateTime? source, Timestamp? destination, ResolutionContext context)
        => source?.ToTimestamp();

    public Timestamp Convert(DateOnly source, Timestamp destination, ResolutionContext context)
        => DateTime.SpecifyKind(source.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc).ToTimestamp();

    public Timestamp? Convert(DateOnly? source, Timestamp? destination, ResolutionContext context)
        => source?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToTimestamp();
}
