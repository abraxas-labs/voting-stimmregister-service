// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Lib.VotingExports.Models;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Ech;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Core.Services;

public class ExportEchService : IExportEchService
{
    private const string ExportFileNameFormat = "stimmregister-{0}.xml";

    private readonly IPersonService _personService;
    private readonly IEchService _echService;
    private readonly IDataContext _dataContext;
    private readonly IVerifySigningService _verifySigningService;
    private readonly IFilterVersionRepository _filterVersionRepository;
    private readonly IClock _clock;
    private readonly ILogger<ExportEchService> _logger;
    private readonly BfsIntegrityPersonsVerifier _bfsIntegrityPersonsVerifier;

    public ExportEchService(
        IPersonService personService,
        IEchService echService,
        IClock clock,
        ILogger<ExportEchService> logger,
        IFilterVersionRepository filterVersionRepository,
        IDataContext dataContext,
        IVerifySigningService verifySigningService,
        BfsIntegrityPersonsVerifier bfsIntegrityPersonsVerifier)
    {
        _personService = personService;
        _echService = echService;
        _clock = clock;
        _logger = logger;
        _filterVersionRepository = filterVersionRepository;
        _dataContext = dataContext;
        _verifySigningService = verifySigningService;
        _bfsIntegrityPersonsVerifier = bfsIntegrityPersonsVerifier;
    }

    public async Task<FileModel> ExportEch0045(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria)
    {
        var transaction = await _dataContext.BeginTransaction();
        try
        {
            var persons = await _personService.StreamAllWithCounts(criteria);
            return VerifyPersonSignaturesAndBuildExport(persons, transaction);
        }
        catch (Exception)
        {
            await transaction.DisposeAsync();
            throw;
        }
    }

    public async Task<FileModel> ExportEch0045ByFilter(Guid filterId)
    {
        var transaction = await _dataContext.BeginTransaction();
        try
        {
            var persons = await _personService.StreamAllWithCountsByFilter(filterId);
            return VerifyPersonSignaturesAndBuildExport(persons, transaction);
        }
        catch (Exception)
        {
            await transaction.DisposeAsync();
            throw;
        }
    }

    public async Task<FileModel> ExportEch0045ByFilterVersion(Guid filterVersionId)
    {
        var transaction = await _dataContext.BeginTransaction();
        try
        {
            var filterVersion = await _filterVersionRepository.GetByKey(filterVersionId) ?? throw new EntityNotFoundException(filterVersionId);
            var verifier = _verifySigningService.CreateFilterVersionSignatureVerifier(filterVersion);
            var persons = await _personService.StreamAllWithCountsByFilterVersion(filterVersionId);
            persons.Peek(verifier.Append);
            return BuildEch0045Export(persons, transaction, () => verifier.EnsureValid());
        }
        catch (Exception)
        {
            await transaction.DisposeAsync();
            throw;
        }
    }

    private FileModel VerifyPersonSignaturesAndBuildExport(
        PersonSearchStreamedResultModel<PersonEntity> persons,
        IDbContextTransaction transaction)
    {
        var municipalityIds = new HashSet<int>();
        persons.Peek(p => municipalityIds.Add(p.MunicipalityId));
        return BuildEch0045Export(persons, transaction, async ct => await _bfsIntegrityPersonsVerifier.Verify(municipalityIds, ct));
    }

    private FileModel BuildEch0045Export(
        PersonSearchStreamedResultModel<PersonEntity> persons,
        IDbContextTransaction transaction,
        Action onComplete)
    {
        return BuildEch0045Export(persons, transaction, _ =>
        {
            onComplete();
            return Task.CompletedTask;
        });
    }

    private FileModel BuildEch0045Export(
        PersonSearchStreamedResultModel<PersonEntity> persons,
        IDbContextTransaction transaction,
        Func<CancellationToken, Task> onComplete)
    {
        var fileName = string.Format(ExportFileNameFormat, _clock.UtcNow.ToString("yyyy-MM-dd"));

        if (persons.InvalidCount > 0)
        {
            _logger.LogWarning(
                "Exporting eCH-0045 with {Count} persons, {InvalidCount} persons with invalid data are not included",
                persons.Count,
                persons.InvalidCount);
        }

        return new FileModel(
            fileName,
            ExportFileFormat.Xml,
            WriteData);

        async Task WriteData(PipeWriter pipeWriter, CancellationToken ct)
        {
            try
            {
                await _echService.WriteEch0045(pipeWriter, persons.ValidCount, persons.Data.Where(p => p.IsValid), ct);
                await onComplete(ct);
                await transaction.CommitAsync(ct);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
