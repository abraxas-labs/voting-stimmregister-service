// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Mapping;

public class PersonProfile : Profile
{
    public PersonProfile()
    {
        CreateMap<PersonEntity, PersonEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<PersonDoiEntity, PersonDoiEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
