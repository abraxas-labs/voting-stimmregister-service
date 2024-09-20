// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using Voting.Stimmregister.WebService.Auth;

namespace Voting.Stimmregister.WebService.Services;

/// <summary>
/// Person gRPC service.
/// </summary>
public class PersonGrpcService : PersonService.PersonServiceBase
{
    private readonly IMapper _mapper;
    private readonly IPersonService _personService;

    public PersonGrpcService(IMapper mapper, IPersonService personService)
    {
        _mapper = mapper;
        _personService = personService;
    }

    [AuthorizeReaderOrManager]
    public override async Task<PersonServiceGetLastUsedParametersResponse> GetLastUsedParameters(PersonServiceGetLastUsedParametersRequest request, ServerCallContext context)
    {
        var parameters = await _personService.GetLastUsedParameters((PersonSearchType)request.SearchType);
        return _mapper.Map<PersonServiceGetLastUsedParametersResponse>(parameters);
    }

    [AuthorizeReaderOrManager]
    public override async Task<PersonServiceGetAllResponse> GetAll(
        PersonServiceGetAllRequest request,
        ServerCallContext context)
    {
        var peopleSearchResult = await _personService.GetAll(_mapper.Map<PersonSearchParametersModel>(request));
        return _mapper.Map<PersonServiceGetAllResponse>(peopleSearchResult);
    }

    [AuthorizeReaderOrManager]
    public override async Task<PersonServiceGetSingleResponse> GetSingle(
        PersonServiceGetSingleRequest request,
        ServerCallContext context)
    {
        var searchParameters = _mapper.Map<PersonSearchSingleParametersModel>(request);
        var person = await _personService.GetPersonModelIncludingDoIs(searchParameters.RegisterId);
        return _mapper.Map<PersonServiceGetSingleResponse>(person);
    }

    [AuthorizeReaderOrManager]
    public override async Task<PersonServiceGetAllResponse> GetByFilterId(
        PersonServiceGetByFilterIdRequest request,
        ServerCallContext context)
    {
        var searchParameters = _mapper.Map<PersonSearchFilterIdParametersModel>(request);
        var personSearchResult = await _personService.GetByFilterVersionId(searchParameters);
        return _mapper.Map<PersonServiceGetAllResponse>(personSearchResult);
    }

    [AuthorizeReaderOrManager]
    public override async Task<PersonServiceGetAllResponse> GetByFilterVersionId(
        PersonServiceGetByFilterVersionIdRequest request,
        ServerCallContext context)
    {
        var searchParameters = _mapper.Map<PersonSearchFilterIdParametersModel>(request);
        var personSearchResult = await _personService.GetByFilterVersionId(searchParameters);
        return _mapper.Map<PersonServiceGetAllResponse>(personSearchResult);
    }
}
