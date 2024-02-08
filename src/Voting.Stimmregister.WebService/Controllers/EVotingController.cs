// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Constants.EVoting;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.WebService.Auth;
using Voting.Stimmregister.WebService.Exceptions;
using Voting.Stimmregister.WebService.Models.EVoting.Request;
using Voting.Stimmregister.WebService.Models.EVoting.Response;

namespace Voting.Stimmregister.WebService.Controllers;

[Route("v2/evoting")]
[ApiController]
[AuthorizeEVoting]
[ServiceFilter(typeof(EVotingExceptionFilterAttribute))]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
public class EVotingController : ControllerBase
{
    private readonly IEVotingService _eVotingService;
    private readonly IMapper _mapper;

    public EVotingController(
        IEVotingService eVotingService,
        IMapper mapper)
    {
        _eVotingService = eVotingService;
        _mapper = mapper;
    }

    [HttpPost("information")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProcessStatusResponseBase), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(nameof(GetRegistrationStatus))]
    public async Task<GetRegistrationInformationResponse> GetRegistrationStatus([FromBody] RegistrationStatusRequest request)
    {
        var parsedAhvn13 = ValidateAhvn13(request.Ahvn13);
        ValidateBfsCantonNumber(request.BfsCanton);

        var eVotingInformation = await _eVotingService.GetEVotingInformation(parsedAhvn13, request.BfsCanton);
        return _mapper.Map<GetRegistrationInformationResponse>(eVotingInformation);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProcessStatusResponseBase), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(nameof(RegisterForEVoting))]
    public async Task RegisterForEVoting([FromBody] CreateRegistrationRequest request)
    {
        var parsedAhvn13 = ValidateAhvn13(request.Ahvn13);
        ValidateBfsCantonNumber(request.BfsCanton);

        await _eVotingService.RegisterForEVoting(parsedAhvn13, request.BfsCanton);
    }

    [HttpPost("unregister")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProcessStatusResponseBase), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(nameof(UnregisterFromEVoting))]
    public async Task UnregisterFromEVoting([FromBody] CreateUnregistrationRequest request)
    {
        var parsedAhvn13 = ValidateAhvn13(request.Ahvn13);
        ValidateBfsCantonNumber(request.BfsCanton);

        await _eVotingService.UnregisterFromEVoting(parsedAhvn13, request.BfsCanton);
    }

    [HttpPost("report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProcessStatusResponseBase), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(nameof(GetEVotingReport))]
    public async Task<GetReportResponse> GetEVotingReport([FromBody] RegistrationReportRequest request)
    {
        var parsedAhvn13 = ValidateAhvn13(request.Ahvn13);

        var report = await _eVotingService.GetEVotingReport(parsedAhvn13);
        var response = _mapper.Map<GetReportResponse>(report);
        return response;
    }

    private Ahvn13 ValidateAhvn13(string ahvn13)
    {
        if (!Ahvn13.TryParse(ahvn13, out var parsed))
        {
            throw new EVotingValidationException(
                "Die AHVN13 hat ein ungültiges Format. Erwartetes Format '756.xxxx.xxxx.xc",
                ProcessStatusCode.InvalidAhvn13Format);
        }

        return parsed;
    }

    private void ValidateBfsCantonNumber(short bfs)
    {
        const short min = 1;
        const short max = 26;

        if (bfs < min || bfs > max)
        {
            throw new EVotingValidationException(
                $"Die BFS Kantonsnummer liegt ausserhalb des Gültigkeitsbereiches [{min}...{max}].'",
                ProcessStatusCode.InvalidBfsCantonFormat);
        }
    }
}
