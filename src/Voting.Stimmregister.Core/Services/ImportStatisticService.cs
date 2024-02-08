// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="IImportStatisticService"/>
public class ImportStatisticService : IImportStatisticService
{
    private const string ProcessingErrorDefaultMessage = "default";

    private readonly IImportStatisticRepository _importStatisticRepository;
    private readonly IPermissionService _permissionService;

    public ImportStatisticService(
        IImportStatisticRepository importStatisticRepository,
        IPermissionService permissionService)
    {
        _importStatisticRepository = importStatisticRepository;
        _permissionService = permissionService;
    }

    public async Task<Page<ImportStatisticEntity>> GetHistory(ImportStatisticSearchParametersModel searchParameters)
    {
        var queryable = BuildQuery(searchParameters);
        queryable = FilterByStatus(queryable, searchParameters);

        var pagedResult = await queryable
            .OrderByDescending(i => i.AuditInfo.CreatedAt)
            .ToPageAsync(searchParameters.Page);

        return SanatizeResult(pagedResult);
    }

    public async Task<Page<ImportStatisticEntity>> List(ImportStatisticSearchParametersModel searchParameters)
    {
        if (!searchParameters.ImportType.HasValue)
        {
            throw new InvalidOperationException($"Cannot list {nameof(ImportStatisticEntity)} without {nameof(searchParameters.ImportType)}");
        }

        if (!searchParameters.ImportSourceSystem.HasValue)
        {
            throw new InvalidOperationException($"Cannot list {nameof(ImportStatisticEntity)} without {nameof(searchParameters.ImportSourceSystem)}");
        }

        var query = BuildQuery(searchParameters).Where(x => x.IsLatest);
        query = FilterByStatus(query, searchParameters);

        var pagedResult = await query.ToPageAsync(searchParameters.Page);

        return SanatizeResult(pagedResult);
    }

    private IQueryable<ImportStatisticEntity> BuildQuery(ImportStatisticSearchParametersModel searchParameters)
    {
        var queryable = _importStatisticRepository.Query();

        if (searchParameters.ImportType.HasValue)
        {
            queryable = queryable.Where(i => i.ImportType.Equals(searchParameters.ImportType));
        }

        if (searchParameters.MunicipalityId != null && searchParameters.MunicipalityId != 0)
        {
            queryable = queryable.Where(i => i.MunicipalityId.Equals(searchParameters.MunicipalityId));
        }

        if (searchParameters.ImportSourceSystem != null)
        {
            queryable = queryable.Where(i => i.SourceSystem == searchParameters.ImportSourceSystem);
        }

        if (!_permissionService.IsImportObserver())
        {
            queryable = queryable.Where(i => _permissionService.BfsAccessControlList.Contains(i.MunicipalityId.ToString()));
        }

        return queryable;
    }

    private IQueryable<ImportStatisticEntity> FilterByStatus(IQueryable<ImportStatisticEntity> query, ImportStatisticSearchParametersModel searchParameters)
    {
        return searchParameters.ImportStatus?.Count > 0
            ? query.Where(i => searchParameters.ImportStatus.Contains(i.ImportStatus))
            : query;
    }

    private Page<ImportStatisticEntity> SanatizeResult(Page<ImportStatisticEntity> pagedResult)
    {
        if (_permissionService.IsImportObserver())
        {
            return pagedResult;
        }

        pagedResult.Items.ForEach(item =>
        {
            if (!string.IsNullOrEmpty(item.ProcessingErrors))
            {
                item.ProcessingErrors = ProcessingErrorDefaultMessage;
            }
        });

        return pagedResult;
    }
}
