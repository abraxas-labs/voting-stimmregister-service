// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.WebService.Mapping.Converter;

public class PersonSearchResultPageConverter<TSource, TTarget> : PageConverter<TSource, TTarget>, ITypeConverter<PersonSearchResultPageModel<TSource>, PersonSearchResultPageModel<TTarget>>
    where TSource : PersonEntity
    where TTarget : PersonEntity
{
    public PersonSearchResultPageModel<TTarget> Convert(PersonSearchResultPageModel<TSource> source, PersonSearchResultPageModel<TTarget> destination, ResolutionContext context)
        => new PersonSearchResultPageModel<TTarget>(context.Mapper.Map<Page<TTarget>>(source), source.InvalidPersonsCount);
}
