// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.WebService.Mapping;

public class AuditInfoProfile : Profile
{
    public AuditInfoProfile()
    {
        CreateMap<AuditInfo, Proto.V1.Services.Models.AuditInfoModel>()
            .ForMember(dest => dest.ModifiedById, opt => opt.NullSubstitute(string.Empty))
            .ForMember(dest => dest.ModifiedByName, opt => opt.NullSubstitute(string.Empty));
    }
}
