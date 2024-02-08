// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.WebService.Mapping.Converter;

public class PersonSearchResultPageConverter<TSource, TTarget> : PageConverter<TSource, TTarget>, ITypeConverter<PersonSearchResultPage<TSource>, PersonSearchResultPage<TTarget>>
    where TSource : PersonEntity
    where TTarget : PersonEntity
{
    public PersonSearchResultPage<TTarget> Convert(PersonSearchResultPage<TSource> source, PersonSearchResultPage<TTarget> destination, ResolutionContext context)
        => new PersonSearchResultPage<TTarget>(context.Mapper.Map<Page<TTarget>>(source), source.InvalidPersonsCount);
}
