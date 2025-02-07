// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Mapping;

/// <summary>
/// AutoMapper Profile for person model mappings between:
/// <list type="bullet">
///     <item><see cref="PersonEntity"/></item>
///     <item><see cref="PersonStistatCsvExportModel"/></item>
/// </list>
/// </summary>
public class PersonStistatCsvExportProfile : Profile
{
    public const string DefaultYes = "Ja";
    public const string DefaultNo = "Nein";

    public PersonStistatCsvExportProfile()
    {
        CreateMap<PersonEntity, PersonStistatCsvExportModel>()
            .ForMember(dest => dest.EVoting, opt => opt.MapFrom(src => src.EVoting ? DefaultYes : DefaultNo));
    }
}
