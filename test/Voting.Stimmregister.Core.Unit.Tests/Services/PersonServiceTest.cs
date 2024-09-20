// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Lib.Database.Models;
using Voting.Lib.Testing.Mocks;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Configuration;
using Voting.Stimmregister.Core.Services;
using Voting.Stimmregister.Domain.Constants;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;
using Xunit;

namespace Voting.Stimmregister.Core.Unit.Tests.Services;

public class PersonServiceTest
{
    private readonly PersonService _personService;
    private readonly Mock<ILogger<IPersonService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly MockedClock _clockMock = new();
    private readonly Mock<IPersonRepository> _personRepositoryMock = new();
    private readonly Mock<IFilterRepository> _filterRepositoryMock = new();
    private readonly Mock<IFilterVersionRepository> _filterVersionRepository = new();
    private readonly Mock<IBfsIntegrityRepository> _bfsIntegrityRepositoryMock = new();
    private readonly Mock<ILastSearchParameterService> _lastSearchParameterServiceMock = new();
    private readonly PersonConfig _personConfig = new();

    public PersonServiceTest()
    {
        _personService = new PersonService(
            _loggerMock.Object,
            _mapperMock.Object,
            _clockMock,
            _personRepositoryMock.Object,
            _filterRepositoryMock.Object,
            _filterVersionRepository.Object,
            _bfsIntegrityRepositoryMock.Object,
            _personConfig,
            _lastSearchParameterServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_WhenSearchParametersAreValidAndMultiplePersonsStored_ShouldReturnAllData()
    {
        // Arrange
        var searchParameters = new PersonSearchParametersModel
        {
            Criteria = new List<PersonSearchFilterCriteriaModel> { new() },
            Paging = new Pageable { PageSize = 1 },
        };

        var personEntity1 = new PersonEntity { FirstName = "Carmen", MunicipalityId = 1 };
        var personEntity2 = new PersonEntity { FirstName = "Marco", MunicipalityId = 2 };
        var personEntityModel1 = new PersonEntityModel { FirstName = "Carmen", MunicipalityId = 1 };
        var personEntityModel2 = new PersonEntityModel { FirstName = "Marco", MunicipalityId = 2 };

        var personEntityResult = new PersonSearchResultPageModel<PersonEntity>(new Page<PersonEntity>(new[] { personEntity1, personEntity2 }, 2, 1, 2), 0);
        var personEntityModelResult = new PersonSearchResultPageModel<PersonEntityModel>(new Page<PersonEntityModel>(new[] { personEntityModel1, personEntityModel2 }, 2, 1, 2), 0);

        var integrityEntities = new Dictionary<string, BfsIntegrityEntity>
        {
            ["1"] = new() { LastUpdated = _clockMock.UtcNow },
            ["2"] = new() { LastUpdated = _clockMock.UtcNow.AddDays(-2) },
        };

        _personRepositoryMock
            .Setup(m => m.GetPersonByFilter(It.IsAny<List<FilterCriteriaEntity>>(), _clockMock.Today, false, searchParameters.Paging))
            .Returns(Task.FromResult(personEntityResult));

        _bfsIntegrityRepositoryMock
            .Setup(m => m.ListForBfs(ImportType.Person, It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyDictionary<string, BfsIntegrityEntity>>(integrityEntities));

        _mapperMock
            .Setup(m => m.Map(It.IsAny<PersonSearchResultPageModel<PersonEntity>>(), It.IsAny<Action<IMappingOperationOptions<object, PersonSearchResultPageModel<PersonEntityModel>>>>()))
            .Returns<PersonSearchResultPageModel<PersonEntity>, Action<IMappingOperationOptions<object, PersonSearchResultPageModel<PersonEntityModel>>>>((__, _) => personEntityModelResult);

        // Act
        var result = await _personService.GetAll(searchParameters, requiredValidPageSize: true);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(new[] { personEntityModel1, personEntityModel2 });
        result.TotalItemsCount.Should().Be(2);

        result.Items[0].Actuality.Should().BeTrue();
        result.Items[0].ActualityDate.Should().Be(_clockMock.UtcNow);

        result.Items[1].Actuality.Should().BeFalse();
        result.Items[1].ActualityDate.Should().Be(_clockMock.UtcNow.AddDays(-2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2222)]
    public async Task GetAll_WhenSearchInvalideParametersPageSize_ShouldThrowInvalidArgument(int pageSize)
    {
        // Arrange
        var searchParameters = new PersonSearchParametersModel
        {
            Criteria = new List<PersonSearchFilterCriteriaModel> { new PersonSearchFilterCriteriaModel() },
            Paging = new Pageable { PageSize = pageSize },
        };

        const bool requiredValidPageSize = true;
        var repositoryResult = new PersonSearchResultPageModel<PersonEntity>(new Page<PersonEntity>(new[] { new PersonEntity(), new PersonEntity() }, 2, 1, 2), 0);
        _personRepositoryMock
            .Setup(m => m.GetPersonByFilter(new List<FilterCriteriaEntity> { new() }, _clockMock.Today, false, searchParameters.Paging))
            .Returns(Task.FromResult(repositoryResult));

        // Act
        var errorText = await Assert.ThrowsAsync<InvalidSearchFilterCriteriaException>(async () => await _personService.GetAll(searchParameters, requiredValidPageSize));

        // Assert
        Assert.Equal($"Page size '{pageSize}' out of range (1 - 100)", errorText.Message);
    }

    [Fact]
    public async Task GetSingleIncludingDoIs_WhenRegisterIdNotSet_ShouldThrowInvalidSearchFilterCriteriaException()
    {
        // Arrange
        var personEntityModel1 = new PersonEntityModel { Id = Guid.NewGuid(), RegisterId = Guid.Empty, FirstName = "Carmen" };

        // Act / ASsert
        var errorText = await Assert.ThrowsAsync<InvalidSearchFilterCriteriaException>(async () => await _personService.GetPersonModelIncludingDoIs(personEntityModel1.RegisterId));

        // Assert
        Assert.Equal("Register Id search parameter must not be null or Guid.Empty.", errorText.Message);
    }

    [Theory]
    [InlineData("01.01.1980", false, Countries.Switzerland, true, true, true)]
    [InlineData("01.01.1981", true, Countries.Switzerland, false, true, true)]
    [InlineData("01.01.1982", false, "DE", false, true, false)]
    [InlineData("01.01.3000", false, Countries.Switzerland, false, false, true)]
    public async Task GetSingleIncludingDoIs_WhenSwissAbroad_ShouldHaveValidVotingRightFlags(
        string dateOfBirthString,
        bool restrictedVotingAndElectionRightFederation,
        string? country,
        bool expectedIsVotingAllowed,
        bool expectedIsBirthDateValidForVotingRights,
        bool expectedIsNationalityValidForVotingRights)
    {
        // Arrange
        var dateOfBirth = DateOnly.ParseExact(dateOfBirthString, "dd.MM.yyyy");
        var personEntity1 = new PersonEntity
        {
            RegisterId = Guid.NewGuid(),
            IsSwissAbroad = true,
            FirstName = "Carmen",
            DateOfBirth = dateOfBirth,
            RestrictedVotingAndElectionRightFederation = restrictedVotingAndElectionRightFederation,
            Country = country,
        };

        var personEntityModel1 = new PersonEntityModel
        {
            RegisterId = personEntity1.RegisterId,
            IsSwissAbroad = personEntity1.IsSwissAbroad,
            FirstName = personEntity1.FirstName,
            DateOfBirth = personEntity1.DateOfBirth,
            RestrictedVotingAndElectionRightFederation = personEntity1.RestrictedVotingAndElectionRightFederation,
            Country = personEntity1.Country,
        };

        _personRepositoryMock
            .Setup(m => m.GetPersonByRegisterIdIncludingDoIs(It.IsAny<Guid>()))
            .Returns(Task.FromResult(personEntity1));

        _mapperMock
            .Setup(m => m.Map(It.IsAny<PersonEntity>(), It.IsAny<Action<IMappingOperationOptions<object, PersonEntityModel>>>()))
            .Returns<PersonEntity, Action<IMappingOperationOptions<object, PersonEntityModel>>>((__, _) => personEntityModel1);

        // Act
        var result = await _personService.GetPersonModelIncludingDoIs(personEntityModel1.RegisterId);

        // Assert
        result.Should().NotBeNull();
        result.IsVotingAllowed.Should().Be(expectedIsVotingAllowed);
        result.IsBirthDateValidForVotingRights.Should().Be(expectedIsBirthDateValidForVotingRights);
        result.IsNationalityValidForVotingRights.Should().Be(expectedIsNationalityValidForVotingRights);
    }

    [Theory]
    [InlineData("01.01.1981", false, Countries.Switzerland, "SG", true, true, true, "01.01.2019")]
    [InlineData("01.01.1989", false, Countries.Switzerland, "SG", true, true, true, "10.01.2020")]
    [InlineData("01.01.1990", false, Countries.Switzerland, "SG", true, true, true, null)]
    [InlineData("01.01.1986", false, Countries.Switzerland, "SG", false, true, true, "06.06.2021")]
    [InlineData("01.01.3000", false, Countries.Switzerland, "SG", false, false, true, "01.01.1999")]
    [InlineData("01.01.1982", true, Countries.Switzerland, "SG", false, true, true, "01.08.1959")]
    [InlineData("01.01.1983", false, "DE", "SG", false, true, false, "01.01.2011")]
    [InlineData("01.01.1985", false, Countries.Switzerland, null, true, true, true, "01.01.1978")]
    [InlineData("01.01.3001", true, "DE", null, false, false, false, "01.01.2002")]
    public async Task GetSingleIncludingDoIs_WhenOtherThanSwissAbroad_ShouldHaveValidVotingRightFlags(
        string dateOfBirthString,
        bool restrictedVotingAndElectionRightFederation,
        string? country,
        string? residenceCantonAbbreviation,
        bool expectedIsVotingAllowed,
        bool expectedIsBirthDateValidForVotingRights,
        bool expectedIsNationalityValidForVotingRights,
        string? moveInArrivalDateString)
    {
        // Arrange
        var dateOfBirth = DateOnly.ParseExact(dateOfBirthString, "dd.MM.yyyy");
        DateOnly? moveInArrivalDate = moveInArrivalDateString != null ? DateOnly.ParseExact(moveInArrivalDateString, "dd.MM.yyyy") : null;
        var personEntity1 = new PersonEntity
        {
            RegisterId = Guid.NewGuid(),
            IsSwissAbroad = false,
            FirstName = "Carmen",
            DateOfBirth = dateOfBirth,
            RestrictedVotingAndElectionRightFederation = restrictedVotingAndElectionRightFederation,
            Country = country,
            ResidenceCantonAbbreviation = residenceCantonAbbreviation,
            MunicipalityId = 1,
            MoveInArrivalDate = moveInArrivalDate,
        };

        var personEntityModel1 = new PersonEntityModel
        {
            RegisterId = personEntity1.RegisterId,
            IsSwissAbroad = personEntity1.IsSwissAbroad,
            FirstName = personEntity1.FirstName,
            DateOfBirth = personEntity1.DateOfBirth,
            RestrictedVotingAndElectionRightFederation = personEntity1.RestrictedVotingAndElectionRightFederation,
            Country = personEntity1.Country,
            ResidenceCantonAbbreviation = personEntity1.ResidenceCantonAbbreviation,
            MunicipalityId = 1,
            MoveInArrivalDate = personEntity1.MoveInArrivalDate,
        };

        _personRepositoryMock
            .Setup(m => m.GetPersonByRegisterIdIncludingDoIs(It.IsAny<Guid>()))
            .Returns(Task.FromResult(personEntity1));

        _mapperMock
            .Setup(m => m.Map(It.IsAny<PersonEntity>(), It.IsAny<Action<IMappingOperationOptions<object, PersonEntityModel>>>()))
            .Returns<PersonEntity, Action<IMappingOperationOptions<object, PersonEntityModel>>>((__, _) => personEntityModel1);

        _bfsIntegrityRepositoryMock
            .Setup(x => x.Get(ImportType.Person, "1"))
            .Returns(Task.FromResult<BfsIntegrityEntity?>(new() { LastUpdated = _clockMock.UtcNow }));

        // Act
        var result = await _personService.GetPersonModelIncludingDoIs(personEntityModel1.RegisterId);

        // Assert
        result.Should().NotBeNull();
        result.IsVotingAllowed.Should().Be(expectedIsVotingAllowed);
        result.IsBirthDateValidForVotingRights.Should().Be(expectedIsBirthDateValidForVotingRights);
        result.IsNationalityValidForVotingRights.Should().Be(expectedIsNationalityValidForVotingRights);
        result.Actuality.Should().BeTrue();
        result.ActualityDate.Should().Be(_clockMock.UtcNow);
    }

    [Theory]
    [InlineData("01.01.1981", false, Countries.Switzerland, "SG", true, true, true, "01.01.2019")]
    [InlineData("01.01.1989", false, Countries.Switzerland, "SG", true, true, true, "10.01.2020")]
    [InlineData("01.01.1990", false, Countries.Switzerland, "SG", true, true, true, null)]
    [InlineData("01.01.1986", false, Countries.Switzerland, "SG", false, true, true, "06.06.2021")]
    [InlineData("01.01.3000", false, Countries.Switzerland, "SG", false, false, true, "01.01.1999")]
    [InlineData("01.01.1982", true, Countries.Switzerland, "SG", false, true, true, "01.08.1959")]
    [InlineData("01.01.1983", false, "DE", "SG", false, true, false, "01.01.2011")]
    [InlineData("01.01.1985", false, Countries.Switzerland, null, true, true, true, "01.01.1978")]
    [InlineData("01.01.3001", true, "DE", null, false, false, false, "01.01.2002")]
    public async Task GetSingleFromEntity_ShouldHaveValidVotingRightFlags(
        string dateOfBirthString,
        bool restrictedVotingAndElectionRightFederation,
        string? country,
        string? residenceCantonAbbreviation,
        bool expectedIsVotingAllowed,
        bool expectedIsBirthDateValidForVotingRights,
        bool expectedIsNationalityValidForVotingRights,
        string? moveInArrivalDateString)
    {
        // Arrange
        var dateOfBirth = DateOnly.ParseExact(dateOfBirthString, "dd.MM.yyyy");
        DateOnly? moveInArrivalDate = moveInArrivalDateString != null ? DateOnly.ParseExact(moveInArrivalDateString, "dd.MM.yyyy") : null;
        var personEntity1 = new PersonEntity
        {
            RegisterId = Guid.NewGuid(),
            IsSwissAbroad = false,
            FirstName = "Carmen",
            DateOfBirth = dateOfBirth,
            RestrictedVotingAndElectionRightFederation = restrictedVotingAndElectionRightFederation,
            Country = country,
            ResidenceCantonAbbreviation = residenceCantonAbbreviation,
            MunicipalityId = 1,
            MoveInArrivalDate = moveInArrivalDate,
        };

        var personEntityModel1 = new PersonEntityModel
        {
            RegisterId = personEntity1.RegisterId,
            IsSwissAbroad = personEntity1.IsSwissAbroad,
            FirstName = personEntity1.FirstName,
            DateOfBirth = personEntity1.DateOfBirth,
            RestrictedVotingAndElectionRightFederation = personEntity1.RestrictedVotingAndElectionRightFederation,
            Country = personEntity1.Country,
            ResidenceCantonAbbreviation = personEntity1.ResidenceCantonAbbreviation,
            MunicipalityId = 1,
            MoveInArrivalDate = personEntity1.MoveInArrivalDate,
        };

        _mapperMock
            .Setup(m => m.Map(It.IsAny<PersonEntity>(), It.IsAny<Action<IMappingOperationOptions<object, PersonEntityModel>>>()))
            .Returns<PersonEntity, Action<IMappingOperationOptions<object, PersonEntityModel>>>((__, _) => personEntityModel1);

        _bfsIntegrityRepositoryMock
            .Setup(x => x.Get(ImportType.Person, "1"))
            .Returns(Task.FromResult<BfsIntegrityEntity?>(new() { LastUpdated = _clockMock.UtcNow }));

        // Act
        var result = await _personService.GetPersonModelFromEntity(personEntity1, true, true);

        // Assert
        result.Should().NotBeNull();
        result.IsVotingAllowed.Should().Be(expectedIsVotingAllowed);
        result.IsBirthDateValidForVotingRights.Should().Be(expectedIsBirthDateValidForVotingRights);
        result.IsNationalityValidForVotingRights.Should().Be(expectedIsNationalityValidForVotingRights);
        result.Actuality.Should().BeTrue();
        result.ActualityDate.Should().Be(_clockMock.UtcNow);
    }
}
