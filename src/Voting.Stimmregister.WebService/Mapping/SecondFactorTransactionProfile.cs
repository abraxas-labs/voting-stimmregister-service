// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using AutoMapper;
using Voting.Lib.Iam.SecondFactor.Models;
using Voting.Stimmregister.Domain.Models;
using LibSecondFactorTransactionInfo = Voting.Lib.Iam.SecondFactor.Models.SecondFactorTransactionInfo;
using SecondFactorTransactionInfo = Voting.Stimmregister.Domain.Models.SecondFactorTransactionInfo;
using SecondFactorTransactionModel = Voting.Stimmregister.Proto.V1.Services.Models.SecondFactorTransactionModel;
using SecondFactorTransactionNevisInfoModel = Voting.Stimmregister.Proto.V1.Services.Models.SecondFactorTransactionNevisInfo;

namespace Voting.Stimmregister.WebService.Mapping;

public class SecondFactorTransactionProfile : Profile
{
    public SecondFactorTransactionProfile()
    {
        CreateMap<LibSecondFactorTransactionInfo, SecondFactorTransactionInfo>()
            .ForCtorParam(nameof(SecondFactorTransactionInfo.Id), o => o.MapFrom(s => s.Transaction.Id))
            .ForCtorParam(
                nameof(SecondFactorTransactionInfo.Nevis),
                o => o.MapFrom(s => s.Nevis == null
                    ? null
                    : new SecondFactorTransactionNevisInfo(s.CorrelationCode, s.Nevis.QrCode)));

        CreateMap<SecondFactorTransactionInfo, SecondFactorTransactionModel>();
        CreateMap<SecondFactorTransactionNevisInfo, SecondFactorTransactionNevisInfoModel>();
        CreateMap<SecondFactorTransaction, SecondFactorTransactionEntity>()
            .ReverseMap();
    }
}
