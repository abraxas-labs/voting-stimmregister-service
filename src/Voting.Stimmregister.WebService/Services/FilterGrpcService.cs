// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Voting.Lib.Common;
using Voting.Lib.Grpc;
using Voting.Lib.Iam.Exceptions;
using Voting.Lib.Iam.Store;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using Voting.Stimmregister.WebService.Auth;
using DomainOfInfluenceType = Voting.Stimmregister.Domain.Enums.DomainOfInfluenceType;

namespace Voting.Stimmregister.WebService.Services;

/// <summary>
/// Filter gRPC service.
/// </summary>
public class FilterGrpcService : FilterService.FilterServiceBase
{
    private readonly IAuth _auth;
    private readonly IMapper _mapper;
    private readonly IFilterService _filterService;
    private readonly IAccessControlListDoiService _accessControlListDoiService;

    public FilterGrpcService(
        IAuth auth,
        IMapper mapper,
        IFilterService filterService,
        IAccessControlListDoiService accessControlListDoiService)
    {
        _mapper = mapper;
        _filterService = filterService;
        _accessControlListDoiService = accessControlListDoiService;
        _auth = auth;
    }

    [AuthorizeReaderOrManager]
    public override async Task<FilterServiceGetAllResponse> GetAll(FilterServiceGetAllRequest request, ServerCallContext context)
    {
        var searchParameters = new FilterSearchParametersModel();
        var filterSearchResult = await _filterService.GetAll(searchParameters);
        return _mapper.Map<FilterServiceGetAllResponse>(filterSearchResult);
    }

    [AuthorizeReaderOrManager]
    public override async Task<FilterServiceGetSingleResponse> GetSingle(FilterServiceGetSingleRequest request, ServerCallContext context)
    {
        var id = GuidParser.Parse(request.FilterId);
        var response = new FilterServiceGetSingleResponse();
        var filter = await _filterService.GetSingleById(id) ?? throw new EntityNotFoundException("filter not found");
        response.Filter = _mapper.Map<FilterEntity, FilterDefinitionModel>(filter);
        return response;
    }

    [AuthorizeReaderOrManager]
    public override async Task<FilterServicePreviewMetadataResponse> GetMetadata(FilterServicePreviewMetadataRequest request, ServerCallContext context)
    {
        var id = GuidParser.Parse(request.FilterId);
        var deadline = DateOnly.FromDateTime(request.Deadline.ToDateTime());
        var metadata = await _filterService.GetMetadata(id, deadline);
        return _mapper.Map<FilterServicePreviewMetadataResponse>(metadata);
    }

    [AuthorizeReaderOrManager]
    public override async Task<FilterServiceGetSingleFilterVersionResponse> GetSingleVersion(FilterServiceGetSingleVersionRequest request, ServerCallContext context)
    {
        var versionId = GuidParser.Parse(request.FilterVersionId);
        var filterVersion = await _filterService.GetSingleVersionInclFilterByVersionId(versionId);
        return _mapper.Map<FilterServiceGetSingleFilterVersionResponse>(filterVersion);
    }

    [AuthorizeManager]
    public override async Task<FilterServiceSaveFilterResponse> Save(FilterServiceSaveFilterRequest request, ServerCallContext context)
    {
        var municipalityId = await GetMunicipalityId() ?? throw new ForbiddenException("no municipality id found");
        var filterModel = _mapper.Map<FilterServiceSaveFilterRequest, FilterEntity>(request);
        filterModel.FilterCriterias = _mapper
            .Map<IEnumerable<FilterCriteriaModel>, IEnumerable<FilterCriteriaEntity>>(request.Criteria).ToHashSet();

        var id = await _filterService.Save(filterModel, municipalityId);
        return new FilterServiceSaveFilterResponse
        {
            Id = id.ToString(),
        };
    }

    [AuthorizeManager]
    public override async Task<Empty> Delete(FilterServiceDeleteFilterRequest request, ServerCallContext context)
    {
        var municipalityId = await GetMunicipalityId() ?? throw new ForbiddenException("no municipality id found");
        var id = Guid.Parse(request.FilterId);
        await _filterService.DeleteSingleById(id, municipalityId);
        return ProtobufEmpty.Instance;
    }

    [AuthorizeManager]
    public override async Task<FilterServiceDuplicateFilterResponse> Duplicate(FilterServiceDuplicateFilterRequest request, ServerCallContext context)
    {
        var municipalityId = await GetMunicipalityId() ?? throw new ForbiddenException("no municipality id found");
        var id = Guid.Parse(request.FilterId);
        var createdFilterId = await _filterService.DuplicateSingleById(id, municipalityId);
        return new FilterServiceDuplicateFilterResponse
        {
            Id = createdFilterId.ToString(),
        };
    }

    [AuthorizeManager]
    public override async Task<FilterServiceCreateVersionResponse> CreateVersion(FilterServiceCreateFilterVersionRequest request, ServerCallContext context)
    {
        var filterVersionModel = _mapper.Map<FilterServiceCreateFilterVersionRequest, FilterVersionEntity>(request);
        var id = await _filterService.CreateVersion(filterVersionModel, context.CancellationToken);
        return new FilterServiceCreateVersionResponse
        {
            Id = id.ToString(),
        };
    }

    [AuthorizeManager]
    public override async Task<Empty> RenameVersion(FilterServiceRenameFilterVersionRequest request, ServerCallContext context)
    {
        var id = Guid.Parse(request.FilterVersionId);
        await _filterService.RenameVersion(id, request.Name, context.CancellationToken);
        return ProtobufEmpty.Instance;
    }

    [AuthorizeManager]
    public override async Task<Empty> DeleteVersion(FilterServiceDeleteFilterVersionRequest request, ServerCallContext context)
    {
        var id = Guid.Parse(request.FilterVersionId);
        await _filterService.DeleteVersionById(id);
        return ProtobufEmpty.Instance;
    }

    private async Task<int?> GetMunicipalityId()
    {
        var allBfs = await _accessControlListDoiService.GetBfsNumberAccessControlListByTenantId(_auth.Tenant.Id, DomainOfInfluenceType.Mu);
        var bfs = allBfs.FirstOrDefault();
        return bfs != null ? int.Parse(bfs) : null;
    }
}
