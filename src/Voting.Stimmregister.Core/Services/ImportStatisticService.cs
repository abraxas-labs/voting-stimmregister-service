// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="IImportStatisticService"/>
public class ImportStatisticService : IImportStatisticService
{
    private const string ProcessingErrorDefaultMessage = "default";

    private readonly IImportStatisticRepository _importStatisticRepository;
    private readonly IPermissionService _permissionService;
    private readonly ImportsConfig _importsConfig;
    private readonly IClock _clock;

    public ImportStatisticService(
        IImportStatisticRepository importStatisticRepository,
        IPermissionService permissionService,
        ImportsConfig importsConfig,
        IClock clock)
    {
        _importStatisticRepository = importStatisticRepository;
        _permissionService = permissionService;
        _importsConfig = importsConfig;
        _clock = clock;
    }

    public async Task<IEnumerable<ImportStatisticEntity>> GetHistory(ImportStatisticSearchParametersModel searchParameters)
    {
        var queryable = BuildQuery(searchParameters);
        queryable = FilterByStatus(queryable, searchParameters);

        var result = await queryable
            .OrderByDescending(i => i.AuditInfo.CreatedAt)
            .ToListAsync();

        return SanatizeResult(result);
    }

    public async Task<IEnumerable<ImportStatisticEntity>> List(ImportStatisticSearchParametersModel searchParameters)
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

        var result = await query.ToListAsync();

        return SanatizeResult(result);
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

        // exclude disabled source systems
        if (_importsConfig.AllowedPersonImportSourceSystem.Any())
        {
            var bfsToExclude = _importsConfig.AllowedPersonImportSourceSystem
                .Where(a =>
                    a.ImportSourceSystem != searchParameters.ImportSourceSystem &&
                    a.StartingDate < _clock.UtcNow)
                .Select(a => a.MunicipalityId);

            if (bfsToExclude.Count() > 0)
            {
                queryable = queryable.Where(i => i.MunicipalityId == null || !bfsToExclude.Contains(i.MunicipalityId.Value));
            }
        }

        return queryable;
    }

    private IQueryable<ImportStatisticEntity> FilterByStatus(IQueryable<ImportStatisticEntity> query, ImportStatisticSearchParametersModel searchParameters)
    {
        return searchParameters.ImportStatus?.Count > 0
            ? query.Where(i => searchParameters.ImportStatus.Contains(i.ImportStatus))
            : query;
    }

    private List<ImportStatisticEntity> SanatizeResult(List<ImportStatisticEntity> result)
    {
        if (_permissionService.IsImportObserver())
        {
            return result;
        }

        result.ForEach(item =>
        {
            if (!string.IsNullOrEmpty(item.ProcessingErrors))
            {
                item.ProcessingErrors = ProcessingErrorDefaultMessage;
            }
        });

        return result;
    }
}
