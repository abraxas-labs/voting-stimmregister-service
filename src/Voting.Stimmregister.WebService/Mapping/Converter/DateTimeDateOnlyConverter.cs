// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using AutoMapper;

namespace Voting.Stimmregister.WebService.Mapping.Converter;

/// <summary>
/// AutoMapper type converter for conversions between <see cref="DateTime"/> and <see cref="DateOnly"/>.
/// </summary>
public class DateTimeDateOnlyConverter :
    ITypeConverter<DateTime, DateOnly>,
    ITypeConverter<DateTime?, DateOnly?>,
    ITypeConverter<DateOnly, DateTime>,
    ITypeConverter<DateOnly?, DateTime?>
{
    public DateTime Convert(DateOnly source, DateTime destination, ResolutionContext context)
        => source.ToDateTime(TimeOnly.MinValue);

    public DateTime? Convert(DateOnly? source, DateTime? destination, ResolutionContext context)
        => source?.ToDateTime(TimeOnly.MinValue);

    public DateOnly Convert(DateTime source, DateOnly destination, ResolutionContext context)
        => DateOnly.FromDateTime(source);

    public DateOnly? Convert(DateTime? source, DateOnly? destination, ResolutionContext context)
        => source == null ? null : DateOnly.FromDateTime((DateTime)source);
}
