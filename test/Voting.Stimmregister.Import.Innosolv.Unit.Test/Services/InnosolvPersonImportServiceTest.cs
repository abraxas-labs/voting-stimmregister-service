// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABX_Voting_1_0;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Testing.Mocks;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Models;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Import.Services;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Import.Innosolv.Unit.Test.Services;

public class InnosolvPersonImportServiceTest : BaseWriteableDbTest
{
    private const string SinglePersonValid = "single_person_valid.xml";
    private const string SinglePersonValidModified = "single_person_valid_modified.xml";
    private const string SinglePersonValidModifiedDistrict = "single_person_valid_modified_district.xml";
    private const string SinglePersonValidAddedDistrict = "single_person_valid_added_district.xml";
    private const string SinglePersonValidRemovedDistrict = "single_person_valid_removed_district.xml";
    private const string TwoPersonsValid = "two_persons_valid.xml";
    private const string TestFilesBasePath = "_files";
    private const string SinglePersonValid3213 = "single_person_valid_3213.xml";

    public InnosolvPersonImportServiceTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        GetService<IPermissionService>().SetAbraxasAuthIfNotAuthenticated();
        await base.InitializeAsync();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await DomainOfInfluenceMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task WhenSamePersonImported_ShouldNotUpdatePerson()
    {
        await RunImport(SinglePersonValid);

        var importedPersons1 = await RunOnDb(db => db.Persons
            .IgnoreQueryFilters()
            .Include(x => x.PersonDois.OrderBy(y => y.DomainOfInfluenceType).ThenBy(y => y.Name))
            .ToListAsync());
        importedPersons1.Should().HaveCount(1);
        var p1 = importedPersons1[0];
        p1.Signature = Array.Empty<byte>();
        p1.RegisterId = Guid.Empty;
        p1.Id = Guid.Empty;
        foreach (var doi in p1.PersonDois)
        {
            doi.Id = Guid.Empty;
            doi.PersonId = Guid.Empty;
            doi.Person = null;
        }

        p1.MatchSnapshot(p => p.ImportStatisticId!);

        // import same valid person again
        await RunImport(SinglePersonValid);
        var importedPersons2 = await RunOnDb(db => db.Persons
            .IgnoreQueryFilters()
            .Include(x => x.PersonDois.OrderBy(y => y.DomainOfInfluenceType).ThenBy(y => y.Name))
            .ToListAsync());
        var p2 = importedPersons2.Single();
        p2.Signature = Array.Empty<byte>();
        p2.RegisterId = Guid.Empty;
        p2.Id = Guid.Empty;
        foreach (var doi in p2.PersonDois)
        {
            doi.Id = Guid.Empty;
            doi.PersonId = Guid.Empty;
            doi.Person = null;
        }

        p2.MatchSnapshot(p => p.ImportStatisticId!);
    }

    [Fact]
    public async Task WhenPersonImportedAndUpdated_ShouldUpdatePerson()
    {
        await RunImport(SinglePersonValid);

        await RunImport(SinglePersonValidModified);
        var persons = await RunOnDb(db => db.Persons
            .IgnoreQueryFilters()
            .OrderBy(x => x.VersionCount)
            .ToListAsync());
        persons.Should().HaveCount(2);
        var p1 = persons[0];
        p1.OfficialName.Should().Be("Meyer");
        p1.VersionCount.Should().Be(0);
        p1.IsLatest.Should().BeFalse();

        var p2 = persons[1];
        p2.OfficialName.Should().Be("Gerber");
        p2.VersionCount.Should().Be(1);
        p2.IsLatest.Should().BeTrue();
    }

    [Theory]
    [InlineData(SinglePersonValidModifiedDistrict, new[] { "Evangelische Kirchgemeinde St. Gallen", "Notker" })]
    [InlineData(SinglePersonValidAddedDistrict, new[] { "Katholische Kirchgemeinde St. Gallen", "Notker", "Ortsbürger" })]
    [InlineData(SinglePersonValidRemovedDistrict, new[] { "Notker" })]
    public async Task WhenPersonImportedAndUpdatedDistricts_ShouldUpdatePerson(string testFile, string[] additionalDistricts)
    {
        var defaultDistricts = new[]
        {
            "Kanton St.Gallen (CH)",
            "Kanton St. Gallen SG (CT)",
            "Wahlkreis St.Gallen (BZ)",
            "Gerichtskreis St.Gallen (BZ)",
            "St.Gallen (MU)",
            "Dreissigackerscheid", // Bürgerort
        };

        var expectedDistrictsBeforeModification = new List<string>(defaultDistricts);
        expectedDistrictsBeforeModification.Add("Katholische Kirchgemeinde St. Gallen");
        expectedDistrictsBeforeModification.Add("Notker");

        var expectedDistrictsAfterModification = new List<string>(defaultDistricts);
        expectedDistrictsAfterModification.AddRange(additionalDistricts);

        await RunImport(SinglePersonValid);

        await RunImport(testFile);
        var persons = await RunOnDb(db => db.Persons
            .IgnoreQueryFilters()
            .Include(x => x.PersonDois)
            .OrderBy(x => x.VersionCount)
            .ToListAsync());
        persons.Should().HaveCount(2);
        var p1 = persons[0];
        p1.VersionCount.Should().Be(0);
        p1.IsLatest.Should().BeFalse();
        p1.PersonDois.Should().HaveCount(expectedDistrictsBeforeModification.Count);
        p1.PersonDois.Select(x => x.Name).Should().BeEquivalentTo(expectedDistrictsBeforeModification);

        var p2 = persons[1];
        p2.VersionCount.Should().Be(1);
        p2.IsLatest.Should().BeTrue();
        p2.PersonDois.Should().HaveCount(expectedDistrictsAfterModification.Count);
        p2.PersonDois.Select(x => x.Name).Should().BeEquivalentTo(expectedDistrictsAfterModification);
    }

    [Fact]
    public async Task WhenPersonImportedAndDeleted_ShouldMarkPersonAsDeleted()
    {
        await RunImport(TwoPersonsValid);

        await RunImport(SinglePersonValid);
        var persons = await RunOnDb(db => db.Persons.IgnoreQueryFilters().OrderBy(x => x.Vn).ToListAsync());
        persons.Should().HaveCount(3);
        persons[0].IsDeleted.Should().BeFalse();
        persons[1].IsDeleted.Should().BeFalse();
        persons[2].IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task WhenImportedWithNotAllowedImportSourceSystem_ShouldNotImport()
    {
        await RunImport(SinglePersonValid3213);

        var persons = await RunOnDb(db => db.Persons.IgnoreQueryFilters().OrderBy(x => x.Vn).ToListAsync());
        persons.Should().HaveCount(0);

        var importStatisticEntry = await RunOnDb(db => db.ImportStatistics.IgnoreQueryFilters().SingleOrDefaultAsync(p =>
            p.SourceSystem == ImportSourceSystem.Innosolv
            && p.ImportType == ImportType.Person));

        Assert.Equal("No allowed person import source system configuration is specified for the MunicipalityId '3213'. Import source 'Innosolv' is not allowed.", importStatisticEntry?.ProcessingErrors);
    }

    private async Task RunImport(string fileName)
    {
        var personImportService = GetService<IPersonImportService<PersonInfoType>>();

        await using var fileStream = File.OpenRead(RessourceHelper.GetFullPathToFile(Path.Combine(TestFilesBasePath, fileName)));

        var importDataModel = new ImportDataModel(Guid.NewGuid(), MockedClock.UtcNowDate, fileName, fileStream, ImportSourceSystem.Innosolv);

        var importEntity = new ImportStatisticEntity
        {
            AuditInfo = MockedAuditInfo.Get(),
            FileName = importDataModel.Name,
            Id = importDataModel.Id,
            ImportStatus = ImportStatus.Queued,
            ImportType = ImportType.Person,
            SourceSystem = ImportSourceSystem.Innosolv,
        };

        await GetService<IImportStatisticRepository>().Create(importEntity);
        await personImportService.RunImport(importDataModel);
    }
}
