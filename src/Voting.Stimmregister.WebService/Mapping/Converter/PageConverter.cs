// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using AutoMapper;
using Voting.Lib.Database.Models;

namespace Voting.Stimmregister.WebService.Mapping.Converter;

public class PageConverter<TSource, TTarget> : ITypeConverter<Page<TSource>, Page<TTarget>>
{
    public Page<TTarget> Convert(Page<TSource> source, Page<TTarget> destination, ResolutionContext context)
        => new Page<TTarget>(context.Mapper.Map<IEnumerable<TTarget>>(source.Items), source.TotalItemsCount, source.CurrentPage, source.PageSize);
}
