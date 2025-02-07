// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using Voting.Stimmregister.WebService.Auth;

namespace Voting.Stimmregister.WebService.Services;

/// <summary>
/// Import statistic gRPC service.
/// </summary>
public class ImportStatisticGrpcService : ImportStatisticService.ImportStatisticServiceBase
{
    private static readonly IReadOnlyDictionary<ImportStatusSimple, IReadOnlyCollection<Domain.Enums.ImportStatus>> _importStatusMapping = new Dictionary<ImportStatusSimple, IReadOnlyCollection<Domain.Enums.ImportStatus>>
    {
        [ImportStatusSimple.Success] = new[]
        {
            Domain.Enums.ImportStatus.FinishedSuccessfully,
        },
        [ImportStatusSimple.Failed] = new[]
        {
            Domain.Enums.ImportStatus.Failed,
            Domain.Enums.ImportStatus.Stale,
            Domain.Enums.ImportStatus.Aborted,
            Domain.Enums.ImportStatus.FinishedWithErrors,
        },
    };

    private readonly IImportStatisticService _importStatisticService;
    private readonly IMapper _mapper;

    public ImportStatisticGrpcService(
        IImportStatisticService importStatisticService,
        IMapper mapper)
    {
        _importStatisticService = importStatisticService;
        _mapper = mapper;
    }

    [AuthorizeReaderOrManager]
    public override async Task<ListImportStatisticsResponse> List(
        ListImportStatisticsRequest request,
        ServerCallContext context)
    {
        var searchParameters = new ImportStatisticSearchParametersModel
        {
            ImportType = _mapper.Map<Domain.Enums.ImportType?>(request.ImportType),
            ImportStatus = _importStatusMapping.GetValueOrDefault(request.ImportStatusSimple),
            ImportSourceSystem = _mapper.Map<Domain.Enums.ImportSourceSystem?>(request.ImportSourceSystem),
            IsManualUpload = MapImportSource(request.ImportSource),
        };

        var result = await _importStatisticService.List(searchParameters);
        return _mapper.Map<ListImportStatisticsResponse>(result);
    }

    [AuthorizeReaderOrManager]
    public override async Task<GetImportStatisticHistoryResponse> GetHistory(
        GetImportStatisticHistoryRequest request,
        ServerCallContext context)
    {
        var searchParameters = new ImportStatisticSearchParametersModel
        {
            MunicipalityId = request.MunicipalityId,
            ImportType = _mapper.Map<Domain.Enums.ImportType?>(request.ImportType),
            ImportSourceSystem = _mapper.Map<Domain.Enums.ImportSourceSystem?>(request.ImportSourceSystem),
        };

        var result = await _importStatisticService.GetHistory(searchParameters);
        return _mapper.Map<GetImportStatisticHistoryResponse>(result);
    }

    private bool? MapImportSource(ImportSource importSource)
    {
        return importSource switch
        {
            ImportSource.Manual => true,
            ImportSource.Automated => false,
            _ => null,
        };
    }
}
