// (c) Copyright by Abraxas Informatik AG
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
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Core.Import.Loganto.Mapping;
using Voting.Stimmregister.Core.Services;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Domain.Models.Utils;
using Voting.Stimmregister.Domain.Utils;
using Voting.Stimmregister.Test.Utils.Converters.JsonSerializer.Net;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Voting.Stimmregister.Test.Utils.MockData.EVoting;
using Xunit;

namespace Voting.Stimmregister.Core.Import.Loganto.Unit.Tests.Services;

public class LogantoPersonImportServiceUnitTest : BaseWriteableDbTest
{
    private const string PersonFileValid = "OnePerson_valid.csv";
    private const string PersonFileInvalidFirstName = "OnePerson_invalid_firstName.csv";
    private const string PersonFileValid3203 = "OnePerson_valid_3203.csv";
    private const string PersonFileValidWithOriginDoi = "OnePerson_valid_withOriginDoi.csv";
    private const string PersonFileValidWithOriginDoiChanged = "OnePerson_valid_withOriginDoiChanged.csv";
    private const string PersonFileWithCircleDoi = "OnePerson_valid_withCircleDoi.csv";
    private const string PersonFileWithDoiReference = "OnePerson_valid_with_doi_reference.csv";
    private const string PersonFileWithDoubleEntries = "OnePerson_duplicate.csv";
    private const string PersonFileWithTwoDoubleEntries = "TwoPersons_duplicate.csv";
    private const string PersonFileWithDuplicateAhvn13Entries = "TwoPersons_duplicate_ahvn13.csv";
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
    private const string ShouldHaveSameRegisterIdPersonFileOriginal = "ShouldHaveSameRegisterId_Person_Original.csv";
    private const string ShouldHaveSameRegisterIdPersonFileNewSourceId = "ShouldHaveSameRegisterId_Person_NewSourceId.csv";
    private const string ShouldHaveSameRegisterIdPersonFileEmptyVn = "ShouldHaveSameRegisterId_Person_EmptyVn.csv";
    private const string ShouldHaveSameRegisterIdPersonFileNewName = "ShouldHaveSameRegisterId_Person_NewName.csv";
    private const string PersonCountryFileValid = "FourPeople_country_valid.csv";
    private const string PersonFileValidWithoutAddress = "OnePerson_valid_withoutAddress.csv";
    private const string CountryUnknown = "Staat unbekannt";

    private static readonly string _testFilesPath = Path.Combine("Services", "_files", "Person");

    private readonly IClock _clock;
    private readonly ImportsConfig _importConfig;
    private readonly Mock<ILogger<PersonCsvImportService<LogantoPersonCsvRecord>>> _logger;
    private readonly IPersonRepository _personRepository;
    private readonly IPersonDoiRepository _personDoiRepository;
    private readonly IImportStatisticRepository _importStatisticRepository;
    private readonly IDomainOfInfluenceRepository _domainOfInfluenceRepository;
    private readonly IBfsStatisticRepository _bfsStatisticRepository;
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
        _bfsStatisticRepository = GetService<IBfsStatisticRepository>();
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
    }

    public override async Task InitializeAsync()
    {
        GetService<IPermissionService>().SetAbraxasAuthIfNotAuthenticated();
        await base.InitializeAsync();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await DomainOfInfluenceMockedData.Seed(RunScoped);
        await EVotingUserMockedData.Seed(RunScoped);
        await EVotingAuditMockedData.Seed(RunScoped);
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
        Assert.Single(importedPersons1);

        // 2. import same valid user again
        await ImportPeopleFromFile(PersonFileValid);
        var importedPersons2 = await _personRepository.Query().IgnoreQueryFilters().ToListAsync();
        Assert.Single(importedPersons2);

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
        Assert.Equal(3, personEntities.Count);
        Assert.False(personEntities[0].IsLatest);
        Assert.False(personEntities[1].IsLatest);
        Assert.True(personEntities[2].IsLatest);

        // assert all versions have the same register id
        Assert.Single(personEntities.Select(p => p.RegisterId).Distinct());
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
    ///     <list type="bullet">
    ///         <item>assert bfs statistics created</item>
    ///     </list>
    /// </list>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task WhenPersonWithEVotingCreated_ShouldCreateBfsStatistics()
    {
        // 1. import two valid person
        await ImportPeopleFromFile(PeopleFileValid);

        var bfsStatistics = await _bfsStatisticRepository.Query()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Bfs == "3213");

        bfsStatistics.Should().NotBeNull();
        bfsStatistics!.BfsName.Should().Be("Goldach (MU)");
        bfsStatistics!.VoterTotalCount.Should().Be(2);
        bfsStatistics!.EVoterTotalCount.Should().Be(1);
        bfsStatistics!.EVoterRegistrationCount.Should().Be(2);
        bfsStatistics!.EVoterDeregistrationCount.Should().Be(1);

        // 2. import same two valid person with changed properties
        await ImportPeopleFromFile(PeopleFileValidChanged);

        bfsStatistics = await _bfsStatisticRepository.Query()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Bfs == "3213");

        bfsStatistics.Should().NotBeNull();
        bfsStatistics!.BfsName.Should().Be("Goldach (MU)");
        bfsStatistics!.VoterTotalCount.Should().Be(2);
        bfsStatistics!.EVoterTotalCount.Should().Be(1);
        bfsStatistics!.EVoterRegistrationCount.Should().Be(2);
        bfsStatistics!.EVoterDeregistrationCount.Should().Be(1);
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
        Assert.Equal(4, importedPersons1.Count);

        // get person from db
        var personEntities = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.Vn).ThenBy(p => p.VersionCount)
            .ToListAsync();

        Assert.Equal("DE", personEntities[0].Country);
        Assert.Equal(CountryUnknown, personEntities[1].CountryNameShort);
        Assert.Equal(CountryUnknown, personEntities[2].CountryNameShort);
        Assert.Equal("AD", personEntities[3].Country);
    }

    /// <summary>
    /// Test case:
    /// <list type="number">
    ///     <item>import valid person without address</item>
    ///     <item>get person from db</item>
    ///     <item>match snapshot</item>
    ///     <item>assert address properties</item>
    /// </list>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task WhenValidPersonWithoutAddress_ShouldCreateNewVersionWithFallbackAddress()
    {
        // 1. import valid person
        await ImportPeopleFromFile(PersonFileValidWithoutAddress);

        // get person from db
        var personEntities = await _personRepository.Query()
            .IgnoreQueryFilters()
            .OrderBy(p => p.CreatedDate).ThenBy(p => p.VersionCount)
            .ToListAsync();

        // match snapshot ignoring generated ids
        personEntities.MatchSnapshot(p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);

        // assert
        Assert.Single(personEntities);

        var personEntity = personEntities.FirstOrDefault();

        Assert.NotNull(personEntity);
        Assert.False(string.IsNullOrEmpty(personEntity.ContactAddressExtensionLine1));
        Assert.False(string.IsNullOrEmpty(personEntity.ContactAddressCountryIdIso2));
        Assert.False(string.IsNullOrEmpty(personEntity.ContactAddressStreet));
        Assert.False(string.IsNullOrEmpty(personEntity.ContactAddressTown));
        Assert.False(string.IsNullOrEmpty(personEntity.ContactAddressZipCode));
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
        person.MatchSnapshot(p => p.Id, p => p.RegisterId, p => p.ImportStatisticId!);
    }

    [Fact]
    public async Task WhenImportNonSwisscitizenWithoutOriginAndOnCanton_ShouldNotLogValidationError()
    {
        const long vn = 7562220000021L;
        var person = await TestOriginAndOnCantonValidation(vn, PersonFileWithNonSwisscitizenWithoutOrigin, false);
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
        var importStatisticEntry = await ImportPeopleFromFile(PersonFileValid3203);
        var importedPersonCount = await _personRepository.Query().IgnoreQueryFilters().CountAsync();

        importedPersonCount.Should().Be(0);
        _logger.Invocations.Count(i => i.Arguments.OfType<LogLevel>().Any(a => a == LogLevel.Error)).Should().Be(1);
        importStatisticEntry.Should().NotBeNull();
        importStatisticEntry!.ProcessingErrors.Should().NotBeNull();
        importStatisticEntry.ProcessingErrors.Should().Be("MunicipalityId '3203' does not allow person imports from import source 'Loganto'. Matches item with starting date '01.01.2020 00:00:00'");
    }

    [Fact]
    public async Task WhenImportedWithDuplicatedSourceId_ShouldNotImportButLogError()
    {
        var importStatisticEntry = await ImportPeopleFromFile(PersonFileWithDoubleEntries);
        var importedPersonCount = await _personRepository.Query().IgnoreQueryFilters().CountAsync();

        importedPersonCount.Should().Be(0);
        importStatisticEntry.Should().NotBeNull();
        importStatisticEntry!.ProcessingErrors.Should().NotBeNull();
        importStatisticEntry!.ProcessingErrors.Should().Be("Not allowed person import containing duplicated SourceId (1111111). Import from source system 'Loganto' rejected.");
    }

    [Fact]
    public async Task WhenImportedWithDuplicatedSourceId_ShouldNotImportButLogDuplicatedIds()
    {
        var importStatisticEntry = await ImportPeopleFromFile(PersonFileWithTwoDoubleEntries);
        var importedPersonCount = await _personRepository.Query().IgnoreQueryFilters().CountAsync();

        importedPersonCount.Should().Be(0);
        importStatisticEntry.Should().NotBeNull();
        importStatisticEntry!.ProcessingErrors.Should().NotBeNull();
        importStatisticEntry!.ProcessingErrors.Should().ContainAll("1111111", "2222222");
    }

    [Fact]
    public async Task WhenImportedWithDuplicatedAhvn13_ShouldNotImportButLogDuplicatedIds()
    {
        var importStatisticEntry = await ImportPeopleFromFile(PersonFileWithDuplicateAhvn13Entries);
        var importedPersonCount = await _personRepository.Query().IgnoreQueryFilters().CountAsync();

        importedPersonCount.Should().Be(0);
        importStatisticEntry.Should().NotBeNull();
        importStatisticEntry!.ProcessingErrors.Should().NotBeNull();
        importStatisticEntry!.ProcessingErrors.Should().ContainAll("7562220000001", "7562220000002");
    }

    /// <summary>
    /// represent special case from import failure (Jira: VOTING-4358) to ensure the import works as expected
    /// Test case:
    /// <list type="number">
    ///     <item>import First Person with SourceId and Vn set</item>
    ///     <item>import Second valid Person to mark First Person as deleted</item>
    ///     <item>import First Person with other SourceId</item>
    ///     <item>import First Person with same SourceId but empty Vn</item>
    ///     <item>import Second valid Person to mark First Person as deleted again</item>
    ///     <item>import First Person with same SourceId and same Vn but new Name</item>
    /// </list>
    /// In all Imports the Frist Person should recognized as same Person and have the same RegisterId in all Records.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]

    public async Task WhenImportedWithOtherSourceIdAndThenEmptyVn_ShouldHaveSameRegisterId()
    {
        await ImportPeopleFromFile(ShouldHaveSameRegisterIdPersonFileOriginal);
        var firstPerson = _personRepository.Query().IgnoreQueryFilters().FirstOrDefault();
        await ImportPeopleFromFile(PersonFileValid);
        await ImportPeopleFromFile(ShouldHaveSameRegisterIdPersonFileNewSourceId);
        await ImportPeopleFromFile(ShouldHaveSameRegisterIdPersonFileEmptyVn);
        await ImportPeopleFromFile(PersonFileValid);
        await ImportPeopleFromFile(ShouldHaveSameRegisterIdPersonFileNewName);
        var importedPersons = await _personRepository.Query().IgnoreQueryFilters().ToListAsync();
        importedPersons.Should().HaveCount(10);
        importedPersons.FindAll(p => p.RegisterId == firstPerson?.RegisterId).Should().HaveCount(6);
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
                GetService<ICantonBfsCache>(),
                GetService<IBfsStatisticService>());

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
