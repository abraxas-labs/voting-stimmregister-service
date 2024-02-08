// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.EVoting;
using Voting.Stimmregister.WebService.Models.EVoting.Response;

namespace Voting.Stimmregister.WebService.Mapping;

public class EVotingProfile : Profile
{
    public EVotingProfile()
    {
        CreateMap<EVotingAuditModel, EVotingAuditModel>();
        CreateMap<EVotingReportModel, GetReportResponse>()
            .ForMember(dst => dst.Audits, opts => opts.MapFrom(src => src.Audits));

        CreateMap<EVoterEntity, EVotingReportModel>()
            .ForMember(dst => dst.CreatedAt, opts => opts.MapFrom(src => src.AuditInfo.CreatedAt))
            .ForMember(dst => dst.ModifiedAt, opts => opts.MapFrom(src => src.AuditInfo.ModifiedAt))
            .ForMember(dst => dst.CreatedBy, opts => opts.MapFrom(src => src.AuditInfo.CreatedById))
            .ForMember(dst => dst.ModifiedBy, opts => opts.MapFrom(src => src.AuditInfo.ModifiedById));
        CreateMap<EVoterAuditEntity, EVotingAuditModel>()
            .ForMember(dst => dst.CreatedAt, opts => opts.MapFrom(src => src.AuditInfo.CreatedAt))
            .ForMember(dst => dst.CreatedBy, opts => opts.MapFrom(src => src.AuditInfo.CreatedById));

        CreateMap<EVotingInformationModel, GetRegistrationInformationResponse>()
            .ForMember(dst => dst.VotingStatus, opts => opts.MapFrom(src => src.Status));

        CreateMap<EVotingPersonDataModel, GetRegistrationInformationPerson>()
            .ForMember(dst => dst.Ahvn13, opts => opts.MapFrom(src => src.Ahvn13.ToNumber()))
            .ForMember(dst => dst.MunicipalityBfs, opts => opts.MapFrom(src => src.BfsMunicipality));
        CreateMap<EVotingAddressModel, GetRegistrationInformationAddress>();
    }
}
