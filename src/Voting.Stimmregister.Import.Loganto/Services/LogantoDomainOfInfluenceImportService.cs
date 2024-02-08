// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Import.Mapping;
using Voting.Stimmregister.Abstractions.Import.Models;
using Voting.Stimmregister.Abstractions.Import.Services;
using Voting.Stimmregister.Abstractions.Import.Services.Loganto;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Import.Loganto.Utils;

namespace Voting.Stimmregister.Import.Loganto.Services;

/// <summary>
/// Loganto Domain of influence import service.
/// </summary>
public class LogantoDomainOfInfluenceImportService : BaseImportService<DomainOfInfluenceImportStateModel, LogantoDomainOfInfluenceCsvRecord, DomainOfInfluenceEntity>,
    IDomainOfInfluenceImportService<LogantoDomainOfInfluenceCsvRecord>
{
    private readonly ICreateSignatureService _createSignatureService;
    private readonly IDomainOfInfluenceRepository _domainOfInfluenceRepository;

    public LogantoDomainOfInfluenceImportService(
        IClock clock,
        IImportStatisticRepository importStatisticRepository,
        IIntegrityRepository integrityRepository,
        ILogger<LogantoDomainOfInfluenceImportService> logger,
        IValidator<LogantoDomainOfInfluenceCsvRecord> recordValidator,
        IValidator<DomainOfInfluenceEntity> entityValidator,
        IPermissionService permissionService,
        IAccessControlListDoiService aclDoiService,
        IDataContext dataContext,
        IDomainOfInfluenceRecordEntityMapper<LogantoDomainOfInfluenceCsvRecord> mapper,
        ImportsConfig importConfig,
        ICreateSignatureService createSignatureService,
        IDomainOfInfluenceRepository domainOfInfluenceRepository,
        IMunicipalityIdCantonCache municipalityIdCantonCache,
        ICantonBfsCache cantonBfsCache)
        : base(
            ImportType.DomainOfInfluence,
            clock,
            domainOfInfluenceRepository,
            importStatisticRepository,
            integrityRepository,
            logger,
            recordValidator,
            entityValidator,
            permissionService,
            aclDoiService,
            dataContext,
            mapper,
            importConfig,
            municipalityIdCantonCache,
            cantonBfsCache)
    {
        _createSignatureService = createSignatureService;
        _domainOfInfluenceRepository = domainOfInfluenceRepository;
    }

    protected override void SignEntities(DomainOfInfluenceImportStateModel state)
    {
        if (state.EntitiesToCreate.Count > 0)
        {
            _createSignatureService.BulkSignDomainOfInfluences(state.EntitiesToCreate);
        }

        if (state.EntitiesToUpdate.Count > 0)
        {
            // no need to verify the existing signature of updated entities,
            // since all attributes are overwritten anyway.
            _createSignatureService.BulkSignDomainOfInfluences(state.EntitiesToUpdate);
        }
    }

    protected override void SignIntegrity(DomainOfInfluenceImportStateModel state, BfsIntegrityEntity integrity)
    {
        var entities = state.EntitiesUnchanged
            .Concat(state.EntitiesToCreate)
            .Concat(state.EntitiesToUpdate)
            .ToList();

        _createSignatureService.SignIntegrity(integrity, entities);
    }

    protected override async Task InitializeState(DomainOfInfluenceImportStateModel state)
    {
        if (state.MunicipalityId.HasValue)
        {
            state.InitializeExistingDomainOfInfluences(await _domainOfInfluenceRepository.GetDomainOfInfluencesByBfsNumber(state.MunicipalityId.Value));
        }
    }

    protected override IImportRecordReader<LogantoDomainOfInfluenceCsvRecord> CreateReader(Stream content)
        => CsvImportHelper.CreateRecordReader<LogantoDomainOfInfluenceCsvRecord>(content, ImportConfig.ImportFileDefaultEncoding, ImportConfig.ImportFileCsvDelimiter);

    protected override int GetMunicipalityId(LogantoDomainOfInfluenceCsvRecord record)
        => record.MunicipalityId;

    protected override string BuildHumanReadableIdentifier(LogantoDomainOfInfluenceCsvRecord record)
        => record.BuildRecordIdentifier();

    protected override DomainOfInfluenceEntity? FindExistingEntity(DomainOfInfluenceImportStateModel state, LogantoDomainOfInfluenceCsvRecord record)
        => state.FindDomainOfInfluence(record.DomainOfInfluenceId);

    protected override void MarkUnprocessedAsDeleted(DomainOfInfluenceImportStateModel state)
        => state.DeleteNotImported();

    protected override bool IsImportedDataEqual(DomainOfInfluenceEntity updatedEntity, DomainOfInfluenceEntity existingEntity)
        => DomainOfInfluenceImportedDataComparer.Instance.Equals(updatedEntity, existingEntity);
}
