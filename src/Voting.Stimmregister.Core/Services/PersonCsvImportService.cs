// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Import.Mapping;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.Services;

/// <summary>
/// Import service base implementation.
/// </summary>
/// <typeparam name="TRecord">The csv record model type.</typeparam>
public class PersonCsvImportService<TRecord> : PersonImportService<TRecord>
    where TRecord : IPersonCsvRecord
{
    public PersonCsvImportService(
        IClock clock,
        IImportStatisticRepository importStatisticRepository,
        IIntegrityRepository integrityRepository,
        ILogger<PersonCsvImportService<TRecord>> logger,
        IValidator<TRecord> recordValidator,
        IValidator<PersonEntity> entityValidator,
        IPermissionService permissionService,
        IAccessControlListDoiService aclDoiService,
        IDataContext dataContext,
        IPersonRecordEntityMapper<TRecord> mapper,
        ImportsConfig importConfig,
        IPersonRepository personRepository,
        IDomainOfInfluenceRepository domainOfInfluenceRepository,
        IEVotersCache eVotersCache,
        ICreateSignatureService createSignatureService,
        IMapper personMapper,
        IMunicipalityIdCantonCache municipalityIdCantonCache,
        ICantonBfsCache cantonBfsCache,
        IBfsStatisticService bfsStatisticService)
        : base(
            clock,
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
            personRepository,
            domainOfInfluenceRepository,
            eVotersCache,
            createSignatureService,
            personMapper,
            municipalityIdCantonCache,
            cantonBfsCache,
            bfsStatisticService)
    {
    }

    protected override IImportRecordReader<TRecord> CreateReader(Stream content)
        => CsvImportHelper.CreateRecordReader<TRecord>(content, ImportConfig.ImportFileDefaultEncoding, ImportConfig.ImportFileCsvDelimiter);

    protected override int GetMunicipalityId(TRecord record)
        => record.MunicipalityId;

    protected override string BuildHumanReadableIdentifier(TRecord record)
        => record.BuildRecordIdentifier();

    protected override PersonEntity? FindExistingEntity(PersonImportStateModel state, TRecord record)
        => state.FindExistingPerson(record.Vn, record.SourceSystemId);
}
