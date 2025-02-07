// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Common;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Models;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Core.Import.Cobra.Unit.Tests.Services;

public class CobraPersonImportServiceUnitTest : BaseWriteableDbTest
{
    private const string PersonFileInvalidBfsBlacklisted = "cobra_invalid_bfs_blacklisted.csv";
    private const string PersonFileInvalidBfsNotUnique = "cobra_invalid_bfs_not_unique.csv";
    private const string PersonFileInvalidFileEmpty = "cobra_invalid_file_empty.csv";
    private const string PersonFileInvalidHeader = "cobra_invalid_header.csv";
    private const string PersonFileWhithDuplicatedEntries = "cobra_invalid_duplicate.csv";
    private const string PersonFileWhithTwoDuplicatedEntries = "cobra_invalid_two_duplicates.csv";
    private const string PersonFileInvalidHeaderOnly = "cobra_invalid_header_only.csv";
    private const string PersonFileInvalidMaxRecordErros = "cobra_invalid_max_record_validation_exceeded.csv";
    private const string PersonFileInvalidWithoutOriginAndCanton = "cobra_invalid_person_without_origin_and_canton.csv";
    private const string PersonFileInvalidSwissabroadWithoutOriginAndCanton = "cobra_invalid_swiss_abroad_without_origin_and_canton.csv";
    private const string PersonFileValidOnePerson = "cobra_valid_1.csv";
    private const string PersonFileValidTwoPersons = "cobra_valid_2.csv";
    private const string PersonFileValidOnePersonWithOriginNameAndOnCanton = "cobra_valid_origin_id_name.csv";
    private const string PersonFileValidTwoPersonsUpdated = "cobra_valid_2_updated.csv";
    private const string PersonFileValidTwoPersonsDeleted = "cobra_valid_2_deleted_last.csv";
    private const string PeopleFileValidWithEntityValidationErrors = "cobra_valid_with_entity_validation_errors.csv";
    private const string PeopleFileValidWithRecordValidationErrors = "cobra_valid_with_record_validation_errors.csv";

    private static readonly string _testFilesPath = Path.Combine("Services", "_files");

    private readonly IClock _clock;
    private readonly IPersonRepository _personRepository;
    private readonly IImportStatisticRepository _importStatisticRepository;

    public CobraPersonImportServiceUnitTest(TestApplicationFactory factory)
        : base(factory)
    {
        _personRepository = GetService<IPersonRepository>();
        _importStatisticRepository = GetService<IImportStatisticRepository>();
        _clock = GetService<IClock>();
    }

    public override async Task InitializeAsync()
    {
        GetService<IPermissionService>().SetAbraxasAuthIfNotAuthenticated();
        await base.InitializeAsync();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task WhenBfsBlacklisted_ShouldAbortWithProcessingError()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileInvalidBfsBlacklisted);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.MatchSnapshot(p => p.Id!, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task WhenBfsNotUnique_ShouldAbortWithProcessingError()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileInvalidBfsNotUnique);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.MatchSnapshot(p => p.Id!, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task WhenFileEmpty_ShouldAbortWithProcessingError()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileInvalidFileEmpty);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.MatchSnapshot(p => p.Id!, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task WhenInvalidHeader_ShouldAbortWithProcessingError()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileInvalidHeader);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);

        // ensure CRLF line endings are comparable on windows and linux
        importStatistic!.ProcessingErrors = importStatistic.ProcessingErrors!.Replace("\r\n", "\n");
        importStatistic.ProcessingErrors = importStatistic.ProcessingErrors.Replace("CharCount: 645", "CharCount: 644");
        importStatistic.MatchSnapshot(p => p.Id!, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task WhenHeaderOnly_ShouldAbortWithProcessingError()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileInvalidHeaderOnly);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.MatchSnapshot(p => p.Id!, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task WhenMaxRecordValidationsReached_ShouldAbortWithProcessingError()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileInvalidMaxRecordErros);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.EntitiesWithValidationErrors.Should().HaveCount(1);
        importStatistic.MatchSnapshot(
            p => p.Id!,
            p => p.TotalElapsedMilliseconds!,
            p => p.EntitiesWithValidationErrors);
    }

    [Fact]
    public async Task WhenFileValid_ShouldCreatePerson()
    {
        await ImportPeopleFromFile(PersonFileValidOnePerson);

        var personEntity = await _personRepository.Query()
            .IgnoreQueryFilters()
            .SingleAsync();

        personEntity.MatchSnapshot(
            p => p.Id,
            p => p.RegisterId,
            p => p.ImportStatisticId!);

        await ValidateBfsIntegritySignature(personEntity.MunicipalityId);
    }

    [Fact]
    public async Task ShouldAddDoiToPersonWhenValidPersonIsImported()
    {
        const string sourceSystemId = "917011111";

        await ImportPeopleFromFile(PersonFileValidOnePersonWithOriginNameAndOnCanton);

        var person = await _personRepository.Query()
            .Include(p => p.PersonDois)
            .SingleAsync(p => p.SourceSystemId == sourceSystemId);

        person.PersonDois.ToList().ForEach(p => p.Person = null);
        person
            .PersonDois
            .OrderBy(d => d.DomainOfInfluenceType)
            .ThenBy(d => d.Name)
            .ThenBy(d => d.Identifier)
            .MatchSnapshot(pd => pd.PersonId, pd => pd.Id);
        await ValidateBfsIntegritySignature(person.MunicipalityId);
    }

    [Fact]
    public async Task WhenExistingPersonUpdated_ShouldCreateNewVersion()
    {
        // 1. import valid persons
        await ImportPeopleFromFile(PersonFileValidTwoPersons);

        // 2. import same valid persons with changed properties
        await ImportPeopleFromFile(PersonFileValidTwoPersonsUpdated);

        // get person from db
        var personEntities = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.SourceSystemId).ThenBy(p => p.VersionCount)
            .ToListAsync();

        // match snapshot ignoring generated ids
        personEntities.MatchSnapshot(
            p => p.Id,
            p => p.RegisterId,
            p => p.ImportStatisticId!);

        // assert there are two versions created and latest flag is set on most recent.
        personEntities.Count.Should().Be(4);
        personEntities[0].IsLatest.Should().BeFalse();
        personEntities[1].IsLatest.Should().BeTrue();
        personEntities[2].IsLatest.Should().BeFalse();
        personEntities[3].IsLatest.Should().BeTrue();
        await ValidateBfsIntegritySignature(personEntities[0].MunicipalityId);
    }

    [Fact]
    public async Task WhenPersonNotInImportFile_ShouldSoftDeleteAllVersions()
    {
        // 1. import two valid person
        await ImportPeopleFromFile(PersonFileValidTwoPersons);

        // 2. import same two valid person with changed properties
        await ImportPeopleFromFile(PersonFileValidTwoPersonsUpdated);

        // 3. import same file with one deleted person
        await ImportPeopleFromFile(PersonFileValidTwoPersonsDeleted);

        // get person from db
        var importedPeopleSoftDeleted = await _personRepository.Query()
            .IgnoreQueryFilters()
            .Where(p => p.DeletedDate != null)
            .OrderBy(p => p.VersionCount)
            .ToListAsync();

        // match snapshot ignoring generated ids
        importedPeopleSoftDeleted.MatchSnapshot(
            p => p.Id,
            p => p.RegisterId,
            p => p.ImportStatisticId!);
        await ValidateBfsIntegritySignature(importedPeopleSoftDeleted[0].MunicipalityId);

        var persons = await _personRepository.Query()
            .IgnoreQueryFilters()
            .Include(p => p.PersonDois)
            .OrderBy(p => p.VersionCount)
            .ToListAsync();

        foreach (var person in persons)
        {
            person.PersonDois.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task WhenDeletedPersonImportedAgain_ShouldReactivateLatestVersions()
    {
        // 1. import two valid person
        await ImportPeopleFromFile(PersonFileValidTwoPersons);

        // 2. import same two valid person with changed properties
        await ImportPeopleFromFile(PersonFileValidTwoPersonsUpdated);

        // 3. import same file with one deleted person and first one changed
        await ImportPeopleFromFile(PersonFileValidTwoPersonsDeleted);

        // 4. import two valid person as in step one
        await ImportPeopleFromFile(PersonFileValidTwoPersons);

        // get person from db
        var importedPeopleAllVersions = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.SourceSystemId)
            .ThenBy(p => p.VersionCount)
            .ToListAsync();

        // match snapshot ignoring generated ids
        importedPeopleAllVersions.MatchSnapshot(
            p => p.Id,
            p => p.RegisterId,
            p => p.ImportStatisticId!);
        await ValidateBfsIntegritySignature(importedPeopleAllVersions[0].MunicipalityId);
    }

    [Fact]
    public async Task WhenFileValidWithRecordValidationErrors_ShouldIgnoreInvalidRecords()
    {
        var importStatistic = await ImportPeopleFromFile(PeopleFileValidWithRecordValidationErrors);

        importStatistic.MatchSnapshot(p => p.Id!, p => p.TotalElapsedMilliseconds!);

        var personEntities = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.SourceSystemId)
            .ToListAsync();

        personEntities.Count.Should().Be(0);
    }

    [Fact]
    public async Task WhenFileValidWithEntityValidationErrors_ShouldCreatePersonsAndReportErrors()
    {
        var importStatistic = await ImportPeopleFromFile(PeopleFileValidWithEntityValidationErrors);

        importStatistic.EntitiesWithValidationErrors.Should().HaveCount(1);
        importStatistic.MatchSnapshot(
            "importStatistic",
            p => p.Id!,
            p => p.TotalElapsedMilliseconds!,
            p => p.EntitiesWithValidationErrors);

        var personEntities = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.SourceSystemId)
            .ToListAsync();

        personEntities.MatchSnapshot(
            "persons",
            p => p.Id,
            p => p.RegisterId,
            p => p.ImportStatisticId!);
    }

    [Fact]
    public async Task WhenImportSwissabroadWithoutOriginAndOnCanton_ShouldLogValidationError()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileInvalidSwissabroadWithoutOriginAndCanton);
        importStatistic.MatchSnapshot(p => p.Id!, p => p.EntitiesWithValidationErrors!, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task WhenImportPersonWithNoOriginDoiTwice_NoUpdateRequired_ShouldNotCreateSecondVersion()
    {
        const long Vn = 1111111111111L;
        await ImportPeopleFromFile(PersonFileInvalidWithoutOriginAndCanton);
        var count1 = await _personRepository.Query().IgnoreQueryFilters().CountAsync(p => p.Vn == Vn);
        Assert.Equal(1, count1);
        await ImportPeopleFromFile(PersonFileInvalidWithoutOriginAndCanton);
        var count2 = await _personRepository.Query().IgnoreQueryFilters().CountAsync(p => p.Vn == Vn);
        Assert.Equal(1, count2);
    }

    [Fact]
    public async Task ShouldThrowIfNoValidAclAvailable()
    {
        await RunOnDb(async db =>
        {
            var acl = await db.AccessControlListDois.SingleAsync(x => x.Id == Guid.Parse(AclDoiVotingBasisMockedData.SG_Auslandschweizer_L2_MU.Id));
            acl.IsValid = false;
            db.AccessControlListDois.Update(acl);
            await db.SaveChangesAsync();
        });
        var importStatistic = await ImportPeopleFromFile(PersonFileValidOnePerson);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.MatchSnapshot(p => p.Id, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task ShouldThrowIfMultipleValidAclAvailable()
    {
        await RunOnDb(async db =>
        {
            var acl = new AccessControlListDoiEntity
            {
                Id = Guid.NewGuid(),
                Bfs = AclDoiVotingBasisMockedData.SG_Auslandschweizer_L2_MU.Bfs,
                Type = DomainOfInfluenceType.Mu,
                IsValid = true,
            };
            db.AccessControlListDois.Add(acl);
            await db.SaveChangesAsync();
        });
        var importStatistic = await ImportPeopleFromFile(PersonFileValidOnePerson);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.MatchSnapshot(p => p.Id, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task ShouldFailIfDuplicatedPerson()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileWhithDuplicatedEntries);

        importStatistic.Should().NotBeNull();
        importStatistic!.ProcessingErrors.Should().NotBeNull();
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic!.ProcessingErrors.Should().Be("Not allowed person import containing duplicated SourceId (917011111). Import from source system 'Cobra' rejected.");
    }

    [Fact]
    public async Task ShouldFailIfDuplicatedPersonsAndWriteDuplicateIdsInImportStatistic()
    {
        var importStatistic = await ImportPeopleFromFile(PersonFileWhithTwoDuplicatedEntries);

        importStatistic.Should().NotBeNull();
        importStatistic!.ProcessingErrors.Should().NotBeNull();
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic!.ProcessingErrors.Should().ContainAll("917011111", "917011112");
    }

    private async Task<ImportStatisticEntity> ImportPeopleFromFile(string fileName)
    {
        var personImportservice = NewPersonImportService();

        await using var fileStream = File.OpenRead(RessourceHelper.GetFullPathToFile(Path.Combine(_testFilesPath, fileName)));

        var importDataModel = new ImportDataModel(Guid.NewGuid(), _clock.UtcNow, fileName, fileStream, ImportSourceSystem.Cobra);

        await CreateImportStatistic(importDataModel);

        await personImportservice.RunImport(importDataModel);

        return await _importStatisticRepository.Query().IgnoreQueryFilters().Where(i => i.Id.Equals(importDataModel.Id)).SingleAsync();
    }

    private IPersonImportService<CobraPersonCsvRecord> NewPersonImportService()
        => GetService<IPersonImportService<CobraPersonCsvRecord>>();

    private async Task CreateImportStatistic(ImportDataModel importData)
    {
        var importEntity = new ImportStatisticEntity
        {
            AuditInfo = MockedAuditInfo.Get(),
            StartedDate = _clock.UtcNow,
            FileName = importData.Name,
            QueuedFileName = importData.Name,
            Id = importData.Id,
            ImportStatus = ImportStatus.Queued,
            ImportType = ImportType.Person,
            SourceSystem = ImportSourceSystem.Cobra,
            WorkerName = "TestMachine",
        };

        await _importStatisticRepository.Create(importEntity);
    }

    private async Task ValidateBfsIntegritySignature(int bfs)
    {
        var integrity = await RunOnDb(db => db.BfsIntegrities
            .IgnoreQueryFilters()
            .SingleAsync(x => x.Bfs == bfs.ToString() && x.ImportType == ImportType.Person));
        var persons = await RunOnDb(db => db.Persons
            .IgnoreQueryFilters()
            .Where(x => x.MunicipalityId == bfs && !x.IsDeleted && x.IsLatest)
            .Include(x => x.PersonDois)
            .OrderBy(x => x.Id)
            .ToListAsync());
        GetService<IVerifySigningService>().EnsureBfsIntegritySignatureValid(integrity, persons);
    }
}
