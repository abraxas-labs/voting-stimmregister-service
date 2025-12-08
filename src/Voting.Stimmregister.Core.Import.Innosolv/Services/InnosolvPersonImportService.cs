// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using AbxVoting_1_5;
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
using Voting.Stimmregister.Core.Exceptions;
using Voting.Stimmregister.Core.Services;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Models;

using AbxVoting10Deserializer = Voting.Lib.Ech.AbxVoting_1_0.Converter.AbxVotingDeserializer;
using AbxVoting15Deserializer = Voting.Lib.Ech.AbxVoting_1_5.Converter.AbxVotingDeserializer;

namespace Voting.Stimmregister.Core.Import.Innosolv.Services;

internal class InnosolvPersonImportService : PersonImportService<PersonInfoType>
{
    private readonly ILogger<InnosolvPersonImportService> _logger;
    private readonly AbxVoting10Deserializer _deserializer10;
    private readonly AbxVoting15Deserializer _deserializer15;

    public InnosolvPersonImportService(
        IClock clock,
        IImportStatisticRepository importStatisticRepository,
        IIntegrityRepository integrityRepository,
        ILogger<InnosolvPersonImportService> logger,
        IValidator<PersonEntity> entityValidator,
        IPermissionService permissionService,
        IAccessControlListDoiService aclDoiService,
        IDataContext dataContext,
        IPersonRecordEntityMapper<PersonInfoType> mapper,
        ImportsConfig importConfig,
        IPersonRepository personRepository,
        IDomainOfInfluenceRepository domainOfInfluenceRepository,
        IEVotersCache eVotersCache,
        ICreateSignatureService createSignatureService,
        IMapper personMapper,
        IMunicipalityIdCantonCache municipalityIdCantonCache,
        ICantonBfsCache cantonBfsCache,
        IBfsStatisticService bfsStatisticService,
        AbxVoting10Deserializer deserializer10,
        AbxVoting15Deserializer deserializer15)
        : base(clock, importStatisticRepository, integrityRepository, logger, null, entityValidator, permissionService, aclDoiService, dataContext, mapper, importConfig, personRepository, domainOfInfluenceRepository, eVotersCache, createSignatureService, personMapper, municipalityIdCantonCache, cantonBfsCache, bfsStatisticService)
    {
        _logger = logger;
        _deserializer10 = deserializer10;
        _deserializer15 = deserializer15;
    }

    protected override IImportRecordReader<PersonInfoType> CreateReader(Stream content)
        => new InnosolvXmlReader(content, _deserializer10, _deserializer15);

    protected override PersonEntity? FindExistingEntity(PersonImportStateModel state, PersonInfoType record)
        => state.FindExistingPerson((long?)record.PersonIdentification.Vn, record.PersonIdentification.LocalPersonId.PersonId);

    protected override int GetMunicipalityId(PersonInfoType record)
    {
        var municipalityId = (record.HasMainResidence ?? record.HasSecondaryResidence)?.ReportingMunicipality.MunicipalityId;
        if (municipalityId.HasValue)
        {
            return municipalityId.Value;
        }

        _logger.LogError("Could not extract municipality id from record");
        throw new ImportException("Could not extract municipality id from record");
    }

    protected override string BuildHumanReadableIdentifier(PersonInfoType record)
        => $"{record.PersonIdentification.Vn} {record.PersonIdentification.LocalPersonId.PersonId} {(record.HasMainResidence ?? record.HasSecondaryResidence)?.ReportingMunicipality.MunicipalityId}";
}
