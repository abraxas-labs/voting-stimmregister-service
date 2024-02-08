// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services.Models;

namespace Voting.Stimmregister.WebService.Mapping;

public class DomainOfInfluenceProfile : Profile
{
    public DomainOfInfluenceProfile()
    {
        // domain to proto
        CreateMap<PersonDoiEntity, DomainOfInfluenceModel>();
    }
}
