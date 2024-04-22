// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using Voting.Stimmregister.WebService.Auth;

namespace Voting.Stimmregister.WebService.Services;

/// <summary>
/// Import statistic gRPC service.
/// </summary>
public class RegistrationStatisticGrpcService : RegistrationStatisticService.RegistrationStatisticServiceBase
{
    private readonly IRegistrationStatisticService _registrationStatisticService;
    private readonly IMapper _mapper;

    public RegistrationStatisticGrpcService(
        IRegistrationStatisticService registrationStatisticService,
        IMapper mapper)
    {
        _registrationStatisticService = registrationStatisticService;
        _mapper = mapper;
    }

    [AuthorizeEVotingStatisticsReader]
    public override async Task<ListRegistrationStatisticResponse> List(
        ListRegistrationStatisticRequest request,
        ServerCallContext context)
    {
        var result = await _registrationStatisticService.List();
        return _mapper.Map<ListRegistrationStatisticResponse>(result);
    }
}
