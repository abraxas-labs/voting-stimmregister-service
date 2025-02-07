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
using Voting.Stimmregister.Abstractions.Core.Import.Services.Loganto;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Core.Import.Loganto.Unit.Tests.Services;

public class LogantoDomainOfInfluenceImportTest : BaseWriteableDbTest
{
    private const string DoiFileValid = "DomainOfInfluence_valid.csv";
    private const string DoiFileValidOneDeleted = "DomainOfInfluence_valid_one_deleted.csv";
    private const string DoiFileValidOneUpdated = "DomainOfInfluence_valid_one_updated.csv";
    private const string DoiFileInvalidBfsNotUnique = "DomainOfInfluence_invalid_bfs_not_unique.csv";
    private const string DoiFileInvalidBfsBlacklisted = "DomainOfInfluence_invalid_bfs_blacklisted.csv";
    private const string DoiFileInvalidHeader = "DomainOfInfluence_invalid_header.csv";
    private const string DoiFileInvalidMaxRecordValidations = "DomainOfInfluence_invalid_max_record_validation_exceeded.csv";
    private const string DoiFileValidWithEntityErrors = "DomainOfInfluence_valid_with_entity_validation_errors.csv";
    private const string DoiFileValidWithRecordErrors = "DomainOfInfluence_valid_with_record_validation_errors.csv";

    private static readonly string _testFilesPath = Path.Combine("Services", "_files", "DomainOfInfluence");

    private readonly IImportStatisticRepository _importStatisticRepository;
    private readonly IDomainOfInfluenceRepository _domainOfInfluenceRepository;
    private readonly IClock _clock;

    public LogantoDomainOfInfluenceImportTest(TestApplicationFactory factory)
        : base(factory)
    {
        _importStatisticRepository = GetService<IImportStatisticRepository>();
        _domainOfInfluenceRepository = GetService<IDomainOfInfluenceRepository>();
        _clock = GetService<IClock>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldQueueDomainOfInfluenceImports()
    {
        await ImportDomainOfInfluenceFromFile(DoiFileValid);
        await ImportDomainOfInfluenceFromFile(DoiFileValid);
        await ImportDomainOfInfluenceFromFile(DoiFileValid);

        var importStatisticEntities = await _importStatisticRepository.Query()
            .Where(i => i.FileName.Equals(DoiFileValid))
            .ToListAsync();

        Assert.True(importStatisticEntities.Count == 3);
        importStatisticEntities.MatchSnapshot(i => i.Id);
    }

    [Fact]
    public async Task ShouldCreateDomainOfInfluencesWhenFileValid()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValid);

        var domainOfInfluenceEntities = await _domainOfInfluenceRepository
            .Query()
            .IgnoreQueryFilters()
            .Where(d => d.ImportStatisticId.Equals(importStatistic.Id))
            .OrderBy(d => d.DomainOfInfluenceId)
            .ToListAsync();

        domainOfInfluenceEntities.MatchSnapshot(d => d.Id, d => d.ImportStatisticId!);
    }

    [Fact]
    public async Task ShouldUpdateDomainOfInfluencesWhenFileValidWithUpdates()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValid);
        importStatistic.DatasetsCountCreated.Should().Be(10);

        importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValidOneUpdated);
        importStatistic.DatasetsCountCreated.Should().Be(0);
        importStatistic.DatasetsCountDeleted.Should().Be(0);
        importStatistic.DatasetsCountUpdated.Should().Be(1);

        var domainOfInfluenceEntities = await _domainOfInfluenceRepository
            .Query()
            .IgnoreQueryFilters()
            .Where(d => d.ImportStatisticId.Equals(importStatistic.Id))
            .OrderBy(d => d.DomainOfInfluenceId)
            .ToListAsync();

        domainOfInfluenceEntities.MatchSnapshot(d => d.Id, d => d.ImportStatisticId!);
    }

    [Fact]
    public async Task ShouldDeleteDomainOfInfluencesWhenFileValidWithOneAbsent()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValid);
        importStatistic.DatasetsCountCreated.Should().Be(10);

        importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValidOneDeleted);
        importStatistic.DatasetsCountCreated.Should().Be(0);
        importStatistic.DatasetsCountDeleted.Should().Be(1);
        importStatistic.DatasetsCountUpdated.Should().Be(0);

        const int deletedDomainOfInfluenceId = 10;
        var shouldNotExistDoi = await _domainOfInfluenceRepository
            .Query()
            .IgnoreQueryFilters()
            .Where(d => d.DomainOfInfluenceId.Equals(deletedDomainOfInfluenceId))
            .OrderBy(d => d.DomainOfInfluenceId)
            .SingleOrDefaultAsync();

        shouldNotExistDoi.Should().BeNull();
    }

    [Fact]
    public async Task ShouldEndWithFailedStatusWhenBfsNotUnique()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileInvalidBfsNotUnique);

        importStatistic.MatchSnapshot(d => d!.TotalElapsedMilliseconds!, d => d!.Id);

        var domainOfInfluenceEntities = await _domainOfInfluenceRepository.Query()
            .Where(d => d.ImportStatisticId.Equals(importStatistic.Id))
            .ToListAsync();

        Assert.Empty(domainOfInfluenceEntities);
    }

    [Fact]
    public async Task ShouldEndWithFailedStatusWhenBfsBlacklisted()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileInvalidBfsBlacklisted);

        importStatistic.MatchSnapshot(d => d!.TotalElapsedMilliseconds!, d => d!.Id);

        var domainOfInfluenceEntities = await _domainOfInfluenceRepository.Query()
            .Where(d => d.ImportStatisticId.Equals(importStatistic.Id))
            .ToListAsync();

        Assert.Empty(domainOfInfluenceEntities);
    }

    [Fact]
    public async Task ShouldEndWithFailedStatusWhenInvalidCsvHeader()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileInvalidHeader);

        // ensure CRLF line endings are comparable on windows and linux
        importStatistic!.ProcessingErrors = importStatistic.ProcessingErrors!.Replace("\r\n", "\n");
        importStatistic.ProcessingErrors = importStatistic.ProcessingErrors!.Replace("CharCount: 399", "CharCount: 398");

        importStatistic.MatchSnapshot(d => d.TotalElapsedMilliseconds!, d => d.Id);

        var domainOfInfluenceEntities = await _domainOfInfluenceRepository.Query()
            .Where(d => d.ImportStatisticId.Equals(importStatistic.Id))
            .ToListAsync();

        Assert.Empty(domainOfInfluenceEntities);
    }

    [Fact]
    public async Task ShouldEndWithFailedStatusWhenMaxRecordValidationsExceeded()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileInvalidMaxRecordValidations);

        importStatistic.MatchSnapshot(d => d!.TotalElapsedMilliseconds!, d => d!.Id);

        var domainOfInfluenceEntities = await _domainOfInfluenceRepository.Query()
            .Where(d => d.ImportStatisticId.Equals(importStatistic.Id))
            .ToListAsync();

        Assert.Empty(domainOfInfluenceEntities);
    }

    [Fact]
    public async Task ShouldFinishWithRecordValidationErrors()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValidWithRecordErrors);

        // ensure CRLF line endings are comparable on windows and linux
        importStatistic.ProcessingErrors = importStatistic.ProcessingErrors!.Replace("\r\n", "\n");
        importStatistic.ProcessingErrors = importStatistic.ProcessingErrors.Replace("CharCount: 504", "CharCount: 502");

        importStatistic.MatchSnapshot("importStatistic", d => d.TotalElapsedMilliseconds!, d => d.Id);

        var domainOfInfluenceEntities = await _domainOfInfluenceRepository
            .Query()
            .IgnoreQueryFilters()
            .Where(d => d.ImportStatisticId.Equals(importStatistic.Id))
            .OrderBy(d => d.DomainOfInfluenceId)
            .ToListAsync();

        domainOfInfluenceEntities.MatchSnapshot("domainOfInfluences", d => d.Id, d => d.ImportStatisticId!);
    }

    [Fact]
    public async Task ShouldFinishWithEntityValidationErrors()
    {
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValidWithEntityErrors);

        importStatistic.MatchSnapshot(
            "importStatistic",
            d => d.TotalElapsedMilliseconds!,
            d => d.Id,
            d => d.EntitiesWithValidationErrors);

        var domainOfInfluenceEntities = await _domainOfInfluenceRepository
            .Query()
            .IgnoreQueryFilters()
            .Where(d => d.ImportStatisticId.Equals(importStatistic.Id))
            .OrderBy(d => d.DomainOfInfluenceId)
            .ToListAsync();

        domainOfInfluenceEntities.MatchSnapshot("domainOfInfluences", d => d.Id, d => d.ImportStatisticId!);
    }

    [Fact]
    public async Task ShouldThrowIfNoValidAclAvailable()
    {
        await RunOnDb(async db =>
        {
            var acl = await db.AccessControlListDois.SingleAsync(x => x.Id == Guid.Parse(AclDoiVotingBasisMockedData.SG_Moerschwil_L5_MU.Id));
            acl.IsValid = false;
            db.AccessControlListDois.Update(acl);
            await db.SaveChangesAsync();
        });
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValid);
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
                Bfs = AclDoiVotingBasisMockedData.SG_Moerschwil_L5_MU.Bfs,
                Type = DomainOfInfluenceType.Mu,
                IsValid = true,
            };
            db.AccessControlListDois.Add(acl);
            await db.SaveChangesAsync();
        });
        var importStatistic = await ImportDomainOfInfluenceFromFile(DoiFileValid);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.MatchSnapshot(p => p.Id, p => p.TotalElapsedMilliseconds!);
    }

    private async Task<ImportStatisticEntity> ImportDomainOfInfluenceFromFile(string fileName)
    {
        var domainOfInfluenceImportservice = NewDomainOfInfluenceImportService();

        await using var fileStream = File.OpenRead(RessourceHelper.GetFullPathToFile(Path.Combine(_testFilesPath, fileName)));

        var importDataModel = new ImportDataModel(Guid.NewGuid(), _clock.UtcNow, fileName, fileStream, ImportSourceSystem.Loganto);

        await CreateImportStatistic(importDataModel);

        await domainOfInfluenceImportservice.RunImport(importDataModel);

        return await _importStatisticRepository.Query().IgnoreQueryFilters().Where(i => i.Id.Equals(importDataModel.Id)).SingleAsync();
    }

    private IDomainOfInfluenceImportService<LogantoDomainOfInfluenceCsvRecord> NewDomainOfInfluenceImportService()
        => GetService<IDomainOfInfluenceImportService<LogantoDomainOfInfluenceCsvRecord>>();

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
            ImportType = ImportType.DomainOfInfluence,
            SourceSystem = ImportSourceSystem.Loganto,
            WorkerName = "TestMachine",
        };

        await _importStatisticRepository.Create(importEntity);
    }
}
