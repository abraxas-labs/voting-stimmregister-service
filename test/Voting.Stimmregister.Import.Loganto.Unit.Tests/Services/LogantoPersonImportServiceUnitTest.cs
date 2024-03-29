﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Lib.Common;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Models;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Import.Services;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Domain.Models.Utils;
using Voting.Stimmregister.Domain.Utils;
using Voting.Stimmregister.Import.Loganto.Mapping;
using Voting.Stimmregister.Test.Utils.Converters.JsonSerializer.Net;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Import.Loganto.Unit.Tests.Services;

public class LogantoPersonImportServiceUnitTest : BaseWriteableDbTest
{
    private const string PersonFileValid = "OnePerson_valid.csv";
    private const string PersonFileInvalidFirstName = "OnePerson_invalid_firstName.csv";
    private const string PersonFileValid3203 = "OnePerson_valid_3203.csv";
    private const string PersonFileValidWithOriginDoi = "OnePerson_valid_withOriginDoi.csv";
    private const string PersonFileValidWithOriginDoiChanged = "OnePerson_valid_withOriginDoiChanged.csv";
    private const string PersonFileWithCircleDoi = "OnePerson_valid_withCircleDoi.csv";
    private const string PersonFileWithDoiReference = "OnePerson_valid_with_doi_reference.csv";
    private const string PersonFileValidWithCircleDoiChanged = "OnePerson_valid_withCircleDoiChanged.csv";
    private const string PersonFileWithSwisscitizenWithoutOrigin = "OnePerson_invalid_swisscitizenWithoutOrigin.csv";
    private const string PersonFileWithNonSwisscitizenWithoutOrigin = "OnePerson_invalid_nonSwisscitizenWithoutOrigin.csv";
    private const string PersonFileWithValidLanguageOfCorrespondence = "OnePerson_withValidLanguageOfCorrespondence.csv";
    private const string PersonFileWithUnknownLanguageOfCorrespondence = "OnePerson_withUnknownLanguageOfCorrespondence.csv";
    private const string PersonFileValidChanged = "OnePerson_valid_changed.csv";
    private const string PersonFileValidChangedSecondTime = "OnePerson_valid_changed2.csv";
    private const string PeopleFileValid = "TwoPeople_valid.csv";
    private const string PeopleFileValidChanged = "TwoPeople_valid_changed.csv";
    private const string PeopleFileValidOneDeleted = "TwoPeople_valid_one_deleted.csv";
    private const string PersonCountryFileValid = "FourPeople_country_valid.csv";
    private const string CountryUnknown = "Staat unbekannt";

    private static readonly string _testFilesPath = Path.Combine("Services", "_files", "Loganto", "Person");

    private readonly IClock _clock;
    private readonly ImportsConfig _importConfig;
    private readonly Mock<ILogger<PersonCsvImportService<LogantoPersonCsvRecord>>> _logger;
    private readonly IPersonRepository _personRepository;
    private readonly IPersonDoiRepository _personDoiRepository;
    private readonly IImportStatisticRepository _importStatisticRepository;
    private readonly IDomainOfInfluenceRepository _domainOfInfluenceRepository;
    private readonly Mock<IIntegrityRepository> _integrityRepositoryMock;
    private readonly Mock<ICreateSignatureService> _createSigningMock;
    private readonly Mock<ICountryHelperService> _countryHelperServiceMock;
    private readonly IMapper _mapper;

    public LogantoPersonImportServiceUnitTest(TestApplicationFactory factory)
        : base(factory)
    {
        _personRepository = GetService<IPersonRepository>();
        _personDoiRepository = GetService<IPersonDoiRepository>();
        _importStatisticRepository = GetService<IImportStatisticRepository>();
        _domainOfInfluenceRepository = GetService<IDomainOfInfluenceRepository>();
        _clock = GetService<IClock>();
        _importConfig = GetService<ImportsConfig>();
        _logger = new Mock<ILogger<PersonCsvImportService<LogantoPersonCsvRecord>>>();
        _integrityRepositoryMock = new Mock<IIntegrityRepository>();
        _countryHelperServiceMock = CreateCountryHelperServiceMock();
        _mapper = GetService<IMapper>();

        _createSigningMock = new Mock<ICreateSignatureService>();

        _createSigningMock.Setup(x => x.SignIntegrity(It.IsAny<BfsIntegrityEntity>(), It.IsAny<IReadOnlyCollection<PersonEntity>>()))
            .Callback<BfsIntegrityEntity, IReadOnlyCollection<PersonEntity>>((integrity, _) =>
            {
                integrity.SignatureVersion = 1;
                integrity.SignatureKeyId = "SomeSignatureKeyId";
                integrity.Signature = new byte[] { 1, 0, 1 };
            });

        _createSigningMock.Setup(x => x.BulkSignPersons(It.IsAny<IReadOnlyCollection<PersonEntity>>()))
            .Callback<IReadOnlyCollection<PersonEntity>>(persons =>
            {
                foreach (var person in persons)
                {
                    person.SignatureVersion = 1;
                    person.SignatureKeyId = "SomeSignatureKeyId";
                    person.Signature = new byte[] { 1, 0, 1 };
                }
            });
    }

    public override async Task InitializeAsync()
    {
        GetService<IPermissionService>().SetAbraxasAuthIfNotAuthenticated();
        await base.InitializeAsync();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await DomainOfInfluenceMockedData.Seed(RunScoped);
    }

    /// <summary>
    /// Test case:
    /// <list type="number">
    ///     <item>import valid user</item>
    ///     <item>import valid user again</item>
    ///     <item>assert user is skipped and snapshot is same.</item>
    /// </list>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task WhenSameUserImported_ShouldNotUpdateUser()
    {
        // 1. import valid user
        await ImportPeopleFromFile(PersonFileValid);

        var importedPersons1 = await _personRepository.Query().IgnoreQueryFilters().ToListAsync();
        Assert.True(importedPersons1.Count == 1);

        // 2. import same valid user again
        await ImportPeopleFromFile(PersonFileValid);
        var importedPersons2 = await _personRepository.Query().IgnoreQueryFilters().ToListAsync();
        Assert.True(importedPersons2.Count == 1);

        // assert unchanged
        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new DateOnlyJsonConverter());

        var compareLeft = JsonSerializer.Serialize(importedPersons1[0], jsonOptions);
        var compareRight = JsonSerializer.Serialize(importedPersons2[0], jsonOptions);

        Assert.Equal(compareLeft, compareRight);
    }

    /// <summary>
    /// Test case:
    /// <list type="number">
    ///     <item>import valid person</item>
    ///     <item>import same valid person with changed properties</item>
    ///     <item>import same valid person with other changes</item>
    ///     <item>get person from db</item>
    ///     <item>match snapshot</item>
    ///     <item>assert 3 entries created</item>
    ///     <item>assert registerId same for all verions</item>
    ///     <item>assert latest version set only on latest one</item>
    ///     <item>assert modified dates are set on versions but not on first.</item>
    ///     <item>assert filter references are not copied to new versions.</item>
    /// </list>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task WhenExistingPersonUpdated_ShouldCreateNewVersion()
    {
        // 1. import valid person
        await ImportPeopleFromFile(PersonFileValid);

        // 2. import same valid person with changed properties
        await ImportPeopleFromFile(PersonFileValidChanged);

        // 3. import same valid person with other changes
        await ImportPeopleFromFile(PersonFileValidChangedSecondTime);

        // get person from db
        var personEntities = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.CreatedDate).ThenBy(p => p.VersionCount)
            .ToListAsync();

        // match snapshot ignoring generated ids
        personEntities.MatchSnapshot(p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);

        // assert there are three versions created and latest flag is set on most recent.
        Assert.True(personEntities.Count == 3);
        Assert.False(personEntities[0].IsLatest);
        Assert.False(personEntities[1].IsLatest);
        Assert.True(personEntities[2].IsLatest);

        // assert all versions have the same register id
        Assert.True(personEntities.Select(p => p.RegisterId).Distinct().Count() == 1);
    }

    /// <summary>
    /// Test case:
    /// <list type="number">
    ///     <item>import two valid person</item>
    ///     <item>import same two valid person with changed properties</item>
    ///     <item>import with person absent</item>
    ///     <item>get person from db</item>
    ///     <item>match snapshot</item>
    ///     <list type="bullet">
    ///         <item>assert both versions are flaged as deleted (new property on person)</item>
    ///         <item>assert both versions have deleted date set</item>
    ///         <item>assert registerId same for all verions</item>
    ///         <item>assert latest version set only on latest one</item>
    ///     </list>
    /// </list>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task WhenPersonNotInImportFile_ShouldSoftDeleteLatestVersions()
    {
        // 1. import two valid person
        var importstatistic = await ImportPeopleFromFile(PeopleFileValid);

        var importedPeople = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.OfficialName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();

        importedPeople.MatchSnapshot("two_valid", p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);
        importstatistic.DatasetsCountUpdated.Should().Be(0);
        importstatistic.DatasetsCountCreated.Should().Be(2);
        importstatistic.DatasetsCountDeleted.Should().Be(0);

        // 2. import same two valid person with changed properties
        importstatistic = await ImportPeopleFromFile(PeopleFileValidChanged);

        var importedPeopleLatestVersions = await _personRepository.Query()
            .IgnoreQueryFilters()
            .Where(p => p.IsLatest)
            .OrderBy(p => p.OfficialName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();

        importedPeopleLatestVersions.MatchSnapshot("two_valid_latest_versions", p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);
        importstatistic.DatasetsCountUpdated.Should().Be(2);
        importstatistic.DatasetsCountCreated.Should().Be(0);
        importstatistic.DatasetsCountDeleted.Should().Be(0);

        // 3. import same file with one deleted person
        importstatistic = await ImportPeopleFromFile(PeopleFileValidOneDeleted);

        var importedPeopleSoftDeleted = await _personRepository.Query()
            .IgnoreQueryFilters()
            .Where(p => p.DeletedDate != null)
            .OrderBy(p => p.VersionCount)
            .ToListAsync();

        importedPeopleSoftDeleted.MatchSnapshot("two_valid_one_deleted", p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);
        importstatistic.DatasetsCountUpdated.Should().Be(0);
        importstatistic.DatasetsCountCreated.Should().Be(0);
        importstatistic.DatasetsCountDeleted.Should().Be(1);

        // 4. import same file from step 3 with one deleted person
        importstatistic = await ImportPeopleFromFile(PeopleFileValidOneDeleted);

        importstatistic.DatasetsCountUpdated.Should().Be(0);
        importstatistic.DatasetsCountCreated.Should().Be(0);
        importstatistic.DatasetsCountDeleted.Should().Be(0);
    }

    /// <summary>
    /// Test case:
    /// <list type="number">
    ///     <item>import two valid person</item>
    ///     <item>import same two valid person with changed properties</item>
    ///     <item>import with person absent</item>
    ///     <item>import again as in step one</item>
    ///     <item>get all versions from db</item>
    ///     <item>match snapshot</item>
    ///     <list type="bullet">
    ///         <item>assert latest version is reactivated</item>
    ///         <item>assert no deleted date and deleted by is set on latest, but on older versions still deleted</item>
    ///         <item>assert a third version is created with same properties as first version of the temporarly soft deleted version</item>
    ///         <item>assert latest version set only on latest one</item>
    ///     </list>
    /// </list>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task WhenDeletedPersonImportedAgain_ShouldReactivateLatestVersions()
    {
        // 1. import two valid person
        var importstatistic = await ImportPeopleFromFile(PeopleFileValid);
        importstatistic.DatasetsCountUpdated.Should().Be(0);
        importstatistic.DatasetsCountCreated.Should().Be(2);
        importstatistic.DatasetsCountDeleted.Should().Be(0);

        // 2. import same two valid person with changed properties
        importstatistic = await ImportPeopleFromFile(PeopleFileValidChanged);
        importstatistic.DatasetsCountUpdated.Should().Be(2);
        importstatistic.DatasetsCountCreated.Should().Be(0);
        importstatistic.DatasetsCountDeleted.Should().Be(0);

        // 3. import same file with one deleted person
        importstatistic = await ImportPeopleFromFile(PeopleFileValidOneDeleted);
        importstatistic.DatasetsCountUpdated.Should().Be(0);
        importstatistic.DatasetsCountCreated.Should().Be(0);
        importstatistic.DatasetsCountDeleted.Should().Be(1);

        // 4. import two valid person as in step one
        importstatistic = await ImportPeopleFromFile(PeopleFileValid);

        var importedPeopleAllVersions = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.Vn)
            .ThenBy(p => p.VersionCount)
            .ToListAsync();

        importedPeopleAllVersions.MatchSnapshot(p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);
        importstatistic.DatasetsCountUpdated.Should().Be(1);
        importstatistic.DatasetsCountCreated.Should().Be(1);
        importstatistic.DatasetsCountDeleted.Should().Be(0);
    }

    /// <summary>
    /// Test when invalid person gets imported that it won't store it and log an error.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task WhenInvalidPersonImported_ShouldNotImportButLogError()
    {
        // 1. import the invalid file
        await ImportPeopleFromFile(PersonFileInvalidFirstName);

        // 2. should not import any person records
        var importedPersonCount = await _personRepository.Query().IgnoreQueryFilters().CountAsync();
        Assert.Equal(0, importedPersonCount);

        // should log an error
        Assert.Equal(1, _logger.Invocations.Count(i => i.Arguments.OfType<LogLevel>().Any(a => a == LogLevel.Error)));

        // should log official name, first name, vn, date of birth to record validation error
        var importStatisticEntry = await _importStatisticRepository.Query().IgnoreQueryFilters()
            .SingleOrDefaultAsync(p =>
                p.SourceSystem == ImportSourceSystem.Loganto
                && p.ImportType == ImportType.Person);
        Assert.NotNull(importStatisticEntry?.RecordValidationErrors);
        Assert.True(importStatisticEntry?.RecordValidationErrors.Contains("1978") ?? false);
        Assert.True(importStatisticEntry?.RecordValidationErrors.Contains("Muster") ?? false);
    }

    /// <summary>
    /// Test case:
    /// <list type="number">
    ///     <item>Person with mapped loganto country code to Iso2 code</item>
    /// </list>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task WhenUserWithCountryImported_ShouldMapLogantoCountrytoIso2Code()
    {
        // 1. import person with mapped loganto country code to Iso2 code
        await ImportPeopleFromFile(PersonCountryFileValid);

        var importedPersons1 = await _personRepository.Query().IgnoreQueryFilters().ToListAsync();
        Assert.True(importedPersons1.Count == 4);

        // get person from db
        var personEntities = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.Vn).ThenBy(p => p.VersionCount)
            .ToListAsync();

        Assert.True(personEntities[0].Country!.Equals("DE"));
        Assert.True(personEntities[1].CountryNameShort!.Equals(CountryUnknown));
        Assert.True(personEntities[2].CountryNameShort!.Equals(CountryUnknown));
        Assert.True(personEntities[3].Country!.Equals("AD"));
    }

    [Fact]
    public async Task WhenValidPersonWithMultipleOriginImported_ShouldCreatePersonWithPersonDoiEntries()
    {
        await ImportPeopleFromFile(PersonFileValidWithOriginDoi);
        var personDois1 = GetPersonDois();
        personDois1.MatchSnapshot("PersonDois1_2");

        // same person with same DOI information imported again does not create a new person version
        await ImportPeopleFromFile(PersonFileValidWithOriginDoi);
        var personDois2 = GetPersonDois();
        personDois2.MatchSnapshot("PersonDois1_2");

        await ImportPeopleFromFile(PersonFileValidWithOriginDoiChanged);
        var personDois3 = GetPersonDois();
        personDois3.MatchSnapshot("PersonDois3");
    }

    [Fact]
    public async Task WhenValidPersonWithMultipleCircleImported_ShouldCreatePersonWithPersonDoiEntries()
    {
        await ImportPeopleFromFile(PersonFileWithCircleDoi);
        var personDois1 = GetPersonDois();
        personDois1.MatchSnapshot("PersonDois1");

        // same person with same DOI information imported again does not create a new person version
        await ImportPeopleFromFile(PersonFileWithCircleDoi);
        var personDois2 = GetPersonDois();
        personDois2.MatchSnapshot("PersonDois1");

        await ImportPeopleFromFile(PersonFileValidWithCircleDoiChanged);
        var personDois3 = GetPersonDois();
        personDois3.MatchSnapshot("PersonDois3");
    }

    [Fact]
    public async Task WhenValidPersonWithDoiReferenceImportedTwice_ShouldNotUpdate()
    {
        await ImportPeopleFromFile(PersonFileWithDoiReference);
        var personDois1 = GetPersonDois();
        personDois1.MatchSnapshot();

        // same person with same DOI information imported again should not update.
        await ImportPeopleFromFile(PersonFileWithDoiReference);
        var personDois2 = GetPersonDois();

        personDois2.MatchSnapshot();
    }

    [Fact]
    public async Task WhenImportSwisscitizenWithoutOrigin_ShouldLogValidationError()
    {
        const long vn = 7562220000022L;
        var person = await TestOriginAndOnCantonValidation(vn, PersonFileWithSwisscitizenWithoutOrigin, true);
        person.Signature = Array.Empty<byte>();
        person.MatchSnapshot(p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);
    }

    [Fact]
    public async Task WhenImportNonSwisscitizenWithoutOriginAndOnCanton_ShouldNotLogValidationError()
    {
        const long vn = 7562220000021L;
        var person = await TestOriginAndOnCantonValidation(vn, PersonFileWithNonSwisscitizenWithoutOrigin, false);
        person.Signature = Array.Empty<byte>();
        person.MatchSnapshot(p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);
    }

    [Fact]
    public async Task WhenImportValidLanguageOfCorrespondence_ShouldMapLanguageToIso()
    {
        const long vn = 7562220000087;
        await ImportPeopleFromFile(PersonFileWithValidLanguageOfCorrespondence);
        var person = _personRepository.Query().IgnoreQueryFilters().Single(p => p.Vn == vn);
        Assert.Equal("fr", person.LanguageOfCorrespondence);
    }

    [Fact]
    public async Task WhenImportUnknownLanguageOfCorrespondence_ShouldMapLanguageToGerman()
    {
        const long vn = 7562220000088;
        await ImportPeopleFromFile(PersonFileWithUnknownLanguageOfCorrespondence);
        var person = _personRepository.Query().IgnoreQueryFilters().Single(p => p.Vn == vn);
        Assert.Equal("de", person.LanguageOfCorrespondence);
    }

    [Fact]
    public async Task WhenReimportWithUnchangedLanguage_NoUpdate_NoPersonVersionAdded()
    {
        const long vn = 7562220000087;
        await ImportPeopleFromFile(PersonFileWithValidLanguageOfCorrespondence);
        Assert.Equal(1, _personRepository.Query().IgnoreQueryFilters().Count(p => p.Vn == vn));
        await ImportPeopleFromFile(PersonFileWithValidLanguageOfCorrespondence);
        Assert.Equal(1, _personRepository.Query().IgnoreQueryFilters().Count(p => p.Vn == vn));
    }

    [Fact]
    public async Task ShouldThrowIfNoValidAclAvailable()
    {
        await RunOnDb(async db =>
        {
            var acl = await db.AccessControlListDois.SingleAsync(x => x.Id == Guid.Parse(AclDoiVotingBasisMockedData.SG_Goldach_L5_MU.Id));
            acl.IsValid = false;
            db.AccessControlListDois.Update(acl);
            await db.SaveChangesAsync();
        });
        var importStatistic = await ImportPeopleFromFile(PeopleFileValid);
        importStatistic.ImportStatus.Should().Be(ImportStatus.Failed);
        importStatistic.MatchSnapshot(p => p.Id, p => p.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task WhenImportedWithNotAllowedImportSourceSystem_ShouldNotImportButLogError()
    {
        await ImportPeopleFromFile(PersonFileValid3203);

        var importedPersonCount = await _personRepository.Query().IgnoreQueryFilters().CountAsync();
        Assert.Equal(0, importedPersonCount);

        Assert.Equal(1, _logger.Invocations.Count(i => i.Arguments.OfType<LogLevel>().Any(a => a == LogLevel.Error)));

        var importStatisticEntry = await _importStatisticRepository.Query().IgnoreQueryFilters()
            .SingleOrDefaultAsync(p =>
                p.SourceSystem == ImportSourceSystem.Loganto
                && p.ImportType == ImportType.Person);

        Assert.Equal("MunicipalityId '3203' does not allow person imports from import source 'Loganto'. Matches item with starting date '01.01.2020 00:00:00'", importStatisticEntry?.ProcessingErrors);
    }

    private static Mock<ICountryHelperService> CreateCountryHelperServiceMock()
    {
        var mock = new Mock<ICountryHelperService>();
        mock.Setup(x => x.GetLogantoCountryTwoLetterIsoAndShortNameDe(It.IsAny<string?>()))
            .Returns(new CountryHelperServiceResultModel
            {
                Iso2Id = string.Empty,
                ShortNameDe = CountryUnknown,
            });
        mock.Setup(x => x.GetLogantoCountryTwoLetterIsoAndShortNameDe("CH"))
            .Returns(new CountryHelperServiceResultModel
            {
                Iso2Id = "CH",
                ShortNameDe = "Schweiz",
            });
        mock.Setup(x => x.GetLogantoCountryTwoLetterIsoAndShortNameDe("AND"))
            .Returns(new CountryHelperServiceResultModel
            {
                Iso2Id = "AD",
                ShortNameDe = "Andorra",
            });
        mock.Setup(x => x.GetLogantoCountryTwoLetterIsoAndShortNameDe("D"))
            .Returns(new CountryHelperServiceResultModel
            {
                Iso2Id = "DE",
                ShortNameDe = "Deutschland",
            });
        return mock;
    }

    private async Task<PersonEntity> TestOriginAndOnCantonValidation(
        long vn,
        string fileName,
        bool expectValidationErrors)
    {
        await ImportPeopleFromFile(fileName);
        var importStatistic = _importStatisticRepository.Query().IgnoreQueryFilters()
            .Single(importStatistic => importStatistic.FileName == fileName);
        Assert.Equal(ImportStatus.FinishedSuccessfully, importStatistic.ImportStatus);
        Assert.False(_personDoiRepository
            .Query()
            .Where(p => p.DomainOfInfluenceType == DomainOfInfluenceType.Og)
            .IgnoreQueryFilters()
            .Any());
        var person = _personRepository.Query().IgnoreQueryFilters().Single(person => person.Vn == vn);
        person.Signature = Array.Empty<byte>();
        Assert.Equal(!expectValidationErrors, person.IsValid);
        Assert.Equal(expectValidationErrors, person.ValidationErrors != null);
        return person;
    }

    private PersonDoiEntity[] GetPersonDois()
    {
        var personDois = _personDoiRepository
            .Query()
            .IgnoreQueryFilters()
            .ToList()
            .OrderBy(e => e.DomainOfInfluenceType)
            .ThenBy(e => e.Name)
            .ThenBy(e => e.Identifier);
        foreach (var personDoi in personDois)
        {
            personDoi.Id = Guid.Empty;
            personDoi.PersonId = Guid.Empty;
            personDoi.Person = null!;
        }

        return personDois.ToArray();
    }

    private async Task<ImportStatisticEntity> ImportPeopleFromFile(string fileName)
    {
        var personImportService = NewPersonImportService();

        await using var fileStream = File.OpenRead(RessourceHelper.GetFullPathToFile(Path.Combine(_testFilesPath, fileName)));

        var importDataModel = new ImportDataModel(Guid.NewGuid(), _clock.UtcNow, fileName, fileStream, ImportSourceSystem.Loganto);

        await CreateImportEntityAsync(
            importDataModel,
            ImportType.Person,
            ImportSourceSystem.Loganto);

        await personImportService.RunImport(importDataModel);
        return await _importStatisticRepository.Query().IgnoreQueryFilters().Where(i => i.Id.Equals(importDataModel.Id)).SingleAsync();
    }

    private PersonCsvImportService<LogantoPersonCsvRecord> NewPersonImportService() => new(
                _clock,
                _importStatisticRepository,
                _integrityRepositoryMock.Object,
                _logger.Object,
                GetService<IValidator<LogantoPersonCsvRecord>>(),
                GetService<IValidator<PersonEntity>>(),
                GetService<IPermissionService>(),
                GetService<IAccessControlListDoiService>(),
                GetService<IDataContext>(),
                new LogantoPersonMapper(_countryHelperServiceMock.Object, _clock, new LogantoPersonImportConfig()),
                _importConfig,
                _personRepository,
                _domainOfInfluenceRepository,
                GetService<IEVotersCache>(),
                _createSigningMock.Object,
                _mapper,
                GetService<IMunicipalityIdCantonCache>(),
                GetService<ICantonBfsCache>());

    private async Task CreateImportEntityAsync(
        ImportDataModel importData,
        ImportType importType,
        ImportSourceSystem sourceSystem)
    {
        var importEntity = new ImportStatisticEntity
        {
            AuditInfo = MockedAuditInfo.Get(),
            FileName = importData.Name,
            Id = importData.Id,
            ImportStatus = ImportStatus.Queued,
            ImportType = importType,
            SourceSystem = sourceSystem,
        };

        await _importStatisticRepository.Create(importEntity);
    }
}
