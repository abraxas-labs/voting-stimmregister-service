// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Voting.Lib.Common;
using Voting.Lib.VotingExports.Models;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Extensions;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Abstractions;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="IExportCsvService" />
public class ExportCsvService : IExportCsvService
{
    private const string ExportFileNameFormat = "{0}-{1}.csv";
    private const string ExportFileNamePrefixDefault = "stimmregister";

    private readonly IPersonService _personService;
    private readonly IMapper _mapper;
    private readonly ICsvService _csvService;
    private readonly IDataContext _dataContext;
    private readonly IVerifySigningService _verifySigningService;
    private readonly IFilterVersionRepository _filterVersionRepository;
    private readonly BfsIntegrityPersonsVerifier _bfsIntegrityPersonsVerifier;
    private readonly IClock _clock;

    public ExportCsvService(
        IPersonService personService,
        IMapper mapper,
        ICsvService csvService,
        IClock clock,
        IFilterVersionRepository filterVersionRepository,
        IDataContext dataContext,
        IVerifySigningService verifySigningService,
        BfsIntegrityPersonsVerifier bfsIntegrityPersonsVerifier)
    {
        _personService = personService;
        _mapper = mapper;
        _csvService = csvService;
        _clock = clock;
        _filterVersionRepository = filterVersionRepository;
        _dataContext = dataContext;
        _verifySigningService = verifySigningService;
        _bfsIntegrityPersonsVerifier = bfsIntegrityPersonsVerifier;
    }

    public async Task<FileModel> ExportCsv<T>(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria, ExportCsvOptions options)
    {
        var transaction = await _dataContext.BeginTransaction();
        try
        {
            var persons = _personService.StreamAll(criteria);
            return VerifyPersonSignaturesAndBuildExport<T>(persons, transaction, options);
        }
        catch (Exception)
        {
            await transaction.DisposeAsync();
            throw;
        }
    }

    public async Task<FileModel> ExportCsvByFilter<T>(Guid filterId, ExportCsvOptions options)
    {
        var transaction = await _dataContext.BeginTransaction();
        try
        {
            var persons = await _personService.StreamAllByFilter(filterId);
            return VerifyPersonSignaturesAndBuildExport<T>(persons, transaction, options);
        }
        catch (Exception)
        {
            await transaction.DisposeAsync();
            throw;
        }
    }

    public async Task<FileModel> ExportCsvByFilterVersion<T>(Guid filterVersionId, ExportCsvOptions options)
    {
        var transaction = await _dataContext.BeginTransaction();
        try
        {
            var filterVersion = await _filterVersionRepository.GetByKey(filterVersionId) ?? throw new EntityNotFoundException(filterVersionId);
            var verifier = _verifySigningService.CreateFilterVersionSignatureVerifier(filterVersion);
            var persons = _personService.StreamAllByFilterVersion(filterVersionId);
            persons = persons.Select(p =>
            {
                verifier.Append(p);
                return p;
            });

            return BuildCsvExport<T>(persons, transaction, options, verifier.EnsureValid);
        }
        catch (Exception)
        {
            await transaction.DisposeAsync();
            throw;
        }
    }

    private FileModel VerifyPersonSignaturesAndBuildExport<T>(
        IAsyncEnumerable<PersonEntity> persons,
        IDbContextTransaction transaction,
        ExportCsvOptions options)
    {
        var municipalityIds = new HashSet<int>();
        persons = persons.Peek(p => municipalityIds.Add(p.MunicipalityId));
        return BuildCsvExport<T>(persons, transaction, options, async ct => await _bfsIntegrityPersonsVerifier.Verify(municipalityIds, ct));
    }

    private FileModel BuildCsvExport<T>(
        IAsyncEnumerable<PersonEntity> persons,
        IDbContextTransaction transaction,
        ExportCsvOptions options,
        Action onComplete)
    {
        return BuildCsvExport<T>(persons, transaction, options, _ =>
        {
            onComplete();
            return Task.CompletedTask;
        });
    }

    private FileModel BuildCsvExport<T>(
        IAsyncEnumerable<PersonEntity> persons,
        IDbContextTransaction transaction,
        ExportCsvOptions options,
        Func<CancellationToken, Task> onComplete)
    {
        var fileNamePrefix = !string.IsNullOrEmpty(options.FileNamePrefix) ? options.FileNamePrefix : ExportFileNamePrefixDefault;
        var fileName = string.Format(ExportFileNameFormat, fileNamePrefix, _clock.UtcNow.ToString("yyyy-MM-dd"));
        var mappedPersons = persons.Select(p => _mapper.Map<T>(p));

        return new FileModel(
            fileName,
            ExportFileFormat.Csv,
            WriteData);

        async Task WriteData(PipeWriter pipeWriter, CancellationToken ct)
        {
            try
            {
                await _csvService.Write(pipeWriter, mappedPersons, ct);
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
