// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using Voting.Stimmregister.WebService.Auth;

namespace Voting.Stimmregister.WebService.Services;

public class EcollectingGrpcService : EcollectingService.EcollectingServiceBase
{
    private readonly IECollectingService _eCollectingService;
    private readonly IMapper _mapper;

    public EcollectingGrpcService(
        IECollectingService eCollectingService,
        IMapper mapper)
    {
        _eCollectingService = eCollectingService;
        _mapper = mapper;
    }

    [AuthorizeECollectingCitizenReader]
    public override async Task<EcollectingServiceGetPersonIdByAhvn13Response> EcollectingServiceGetPersonIdByAhvn13(EcollectingServiceGetPersonIdByAhvn13Request request, ServerCallContext context)
    {
        var searchModel = _mapper.Map<ECollectingPersonSearchByVnParametersModel>(request);
        var personSearchResult = await _eCollectingService.GetPersonWithVotingRightByVn(searchModel);
        return _mapper.Map<EcollectingServiceGetPersonIdByAhvn13Response>(personSearchResult);
    }

    [AuthorizeReaderOrManager]
    public override async Task<EcollectingServiceGetPeopleResponse> EcollectingServiceGetPeopleByIds(EcollectingServiceGetPeopleByIdsRequest request, ServerCallContext context)
    {
        var peopleSearchResult = await _eCollectingService.GetPeopleByIds(_mapper.Map<ECollectingPeopleSearchByIdsParametersModel>(request));
        return _mapper.Map<EcollectingServiceGetPeopleResponse>(peopleSearchResult);
    }

    [AuthorizeReaderOrManager]
    public override async Task<EcollectingServiceGetPeopleResponse> EcollectingServiceGetPeopleByName(EcollectingServiceGetPeopleByNameRequest request, ServerCallContext context)
    {
        var peopleSearchResult = await _eCollectingService.GetPeopleByName(_mapper.Map<ECollectingPeopleSearchByNameParametersModel>(request));
        return _mapper.Map<EcollectingServiceGetPeopleResponse>(peopleSearchResult);
    }
}
