// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Voting.Lib.Testing.Mocks;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Proto.V1.Services.Responses;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;
using FilterDataType = Voting.Stimmregister.Proto.V1.Services.Models.FilterDataType;
using FilterReference = Voting.Stimmregister.Proto.V1.Services.Models.FilterReference;
using PersonSearchType = Voting.Stimmregister.Proto.V1.Services.Models.PersonSearchType;

namespace Voting.Stimmregister.WebService.Integration.Tests.PersonGrpcServiceTests;

public class GetAllTest : BaseWriteableDbGrpcTest<PersonService.PersonServiceClient>
{
    public GetAllTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await PersonMockedData.Seed(RunScoped);
        await BfsIntegrityMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task WhenFilterByString_ShouldResolvePerson()
    {
        var filterValues = new Dictionary<FilterReference, string>
        {
            [FilterReference.MunicipalityName] = PersonMockedData.Person_3203_StGallen_5.MunicipalityName!,
            [FilterReference.OfficialName] = PersonMockedData.Person_3203_StGallen_5.OfficialName,
            [FilterReference.FirstName] = PersonMockedData.Person_3203_StGallen_5.FirstName,
            [FilterReference.OriginalName] = PersonMockedData.Person_3203_StGallen_5.OriginalName!,
            [FilterReference.AllianceName] = PersonMockedData.Person_3203_StGallen_5.AllianceName!,
            [FilterReference.AliasName] = PersonMockedData.Person_3203_StGallen_5.AliasName!,
            [FilterReference.OtherName] = PersonMockedData.Person_3203_StGallen_5.OtherName!,
            [FilterReference.CallName] = PersonMockedData.Person_3203_StGallen_5.CallName!,
            [FilterReference.ContactAddressExtensionLine1] = PersonMockedData.Person_3203_StGallen_5.ContactAddressExtensionLine1!,
            [FilterReference.ContactAddressExtensionLine2] = PersonMockedData.Person_3203_StGallen_5.ContactAddressExtensionLine2!,
            [FilterReference.ContactAddressStreet] = PersonMockedData.Person_3203_StGallen_5.ContactAddressStreet!,
            [FilterReference.ContactAddressHouseNumber] = PersonMockedData.Person_3203_StGallen_5.ContactAddressHouseNumber!,
            [FilterReference.ContactAddressPostOfficeBoxText] = PersonMockedData.Person_3203_StGallen_5.ContactAddressPostOfficeBoxText!,
            [FilterReference.ContactAddressTown] = PersonMockedData.Person_3203_StGallen_5.ContactAddressTown!,
            [FilterReference.ContactAddressLocality] = PersonMockedData.Person_3203_StGallen_5.ContactAddressLocality!,
            [FilterReference.ContactAddressZipCode] = PersonMockedData.Person_3203_StGallen_5.ContactAddressZipCode!,
            [FilterReference.ResidenceAddressExtensionLine1] = PersonMockedData.Person_3203_StGallen_5.ResidenceAddressExtensionLine1!,
            [FilterReference.ResidenceAddressExtensionLine2] = PersonMockedData.Person_3203_StGallen_5.ResidenceAddressExtensionLine2!,
            [FilterReference.ResidenceAddressStreet] = PersonMockedData.Person_3203_StGallen_5.ResidenceAddressStreet!,
            [FilterReference.ResidenceAddressHouseNumber] = PersonMockedData.Person_3203_StGallen_5.ResidenceAddressHouseNumber!,
            [FilterReference.ResidenceAddressPostOfficeBoxText] = PersonMockedData.Person_3203_StGallen_5.ResidenceAddressPostOfficeBoxText!,
            [FilterReference.ResidenceAddressTown] = PersonMockedData.Person_3203_StGallen_5.ResidenceAddressTown!,
            [FilterReference.ResidenceAddressZipCode] = PersonMockedData.Person_3203_StGallen_5.ResidenceAddressZipCode!,
            [FilterReference.MoveInMunicipalityName] = PersonMockedData.Person_3203_StGallen_5.MoveInMunicipalityName!,
            [FilterReference.MoveInCantonAbbreviation] = PersonMockedData.Person_3203_StGallen_5.MoveInCantonAbbreviation!,
            [FilterReference.MoveInComesFrom] = PersonMockedData.Person_3203_StGallen_5.MoveInComesFrom!,
            [FilterReference.MoveInCountryNameShort] = PersonMockedData.Person_3203_StGallen_5.MoveInCountryNameShort!,
            [FilterReference.PoliticalCircleId] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_PoliticalCircle.Identifier,
            [FilterReference.PoliticalCircleName] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_PoliticalCircle.Name,
            [FilterReference.CatholicCircleId] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_CatholicCircle.Identifier,
            [FilterReference.CatholicCircleName] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_CatholicCircle.Name,
            [FilterReference.EvangelicCircleId] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_EvangelicCircle.Identifier,
            [FilterReference.EvangelicCircleName] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_EvangelicCircle.Name,
            [FilterReference.SchoolCircleId] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_SchoolCircle.Identifier,
            [FilterReference.SchoolCircleName] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_SchoolCircle.Name,
            [FilterReference.TrafficCircleId] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_TrafficCircle.Identifier,
            [FilterReference.TrafficCircleName] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_TrafficCircle.Name,
            [FilterReference.ResidentialDistrictCircleId] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_ResidentialDistrictCircle.Identifier,
            [FilterReference.ResidentialDistrictCircleName] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_ResidentialDistrictCircle.Name,
            [FilterReference.PeopleCircleId] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_PeopleCircle.Identifier,
            [FilterReference.PeopleCircleName] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_PeopleCircle.Name,
            [FilterReference.OriginName17] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_Origin.Name,
            [FilterReference.OriginOnCanton17] = PersonDoiMockedData.PersonDoi_Person_3203_StGallen_5_Origin.Canton,
        };

        foreach (var item in filterValues)
        {
            var response = await GetAllByFilter(item.Key, item.Value, FilterOperator.Equals, FilterDataType.String);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(item.Key, item.Value[1..^1], FilterOperator.Contains, FilterDataType.String);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(item.Key, item.Value[..^1], FilterOperator.StartsWith, FilterDataType.String);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(item.Key, item.Value[1..], FilterOperator.EndsWith, FilterDataType.String);
            response.People.OrderBy(p => p.Id).MatchSnapshot();
        }
    }

    [Fact]
    public async Task WhenFilterByNumber_ShouldResolvePerson()
    {
        var filterValues = new Dictionary<FilterReference, long>
        {
            [FilterReference.Vn] = PersonMockedData.Person_3203_StGallen_5.Vn!.Value,
            [FilterReference.MunicipalityId] = PersonMockedData.Person_3203_StGallen_5.MunicipalityId,
        };

        foreach (var item in filterValues)
        {
            var response = await GetAllByFilter(item.Key, item.Value.ToString(), FilterOperator.Equals, FilterDataType.Numeric);
            response.People.OrderBy(p => p.Id).MatchSnapshot();
        }
    }

    [Fact]
    public async Task WhenFilterByBoolean_ShouldResolvePerson()
    {
        var filterValues = new Dictionary<FilterReference, bool>
        {
            [FilterReference.RestrictedVotingAndElectionRightFederation] = PersonMockedData.Person_3203_StGallen_5.RestrictedVotingAndElectionRightFederation,
            [FilterReference.EVoting] = PersonMockedData.Person_3203_StGallen_5.EVoting,
            [FilterReference.DateOfBirthAdjusted] = PersonMockedData.Person_3203_StGallen_5.DateOfBirthAdjusted,
            [FilterReference.MoveInUnknown] = PersonMockedData.Person_3203_StGallen_5.MoveInUnknown!.Value,
            [FilterReference.SendVotingCardsToDomainOfInfluenceReturnAddress] = PersonMockedData.Person_3203_StGallen_5.SendVotingCardsToDomainOfInfluenceReturnAddress,
            [FilterReference.HasValidationErrors] = !PersonMockedData.Person_3203_StGallen_5.IsValid,
        };

        foreach (var item in filterValues)
        {
            var response = await GetAllByFilter(item.Key, item.Value.ToString(), FilterOperator.Equals, FilterDataType.Boolean);
            response.People.OrderBy(p => p.Id).MatchSnapshot();
        }
    }

    [Fact]
    public async Task WhenFilterByDate_ShouldResolvePerson()
    {
        var filterValues = new Dictionary<FilterReference, DateOnly>
        {
            [FilterReference.DateOfBirth] = PersonMockedData.Person_3203_StGallen_5.DateOfBirth,
            [FilterReference.ResidencePermitValidFrom] = PersonMockedData.Person_3203_StGallen_5.ResidencePermitValidFrom!.Value,
            [FilterReference.ResidencePermitValidTill] = PersonMockedData.Person_3203_StGallen_5.ResidencePermitValidTill!.Value,
            [FilterReference.ResidenceEntryDate] = PersonMockedData.Person_3203_StGallen_5.ResidenceEntryDate!.Value,
            [FilterReference.MoveInArrivalDate] = PersonMockedData.Person_3203_StGallen_5.MoveInArrivalDate!.Value,
        };

        foreach (var item in filterValues)
        {
            const string dateFormat = "yyyy-MM-dd";
            var response = await GetAllByFilter(item.Key, item.Value.ToString(dateFormat), FilterOperator.Equals, FilterDataType.Date);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(item.Key, item.Value.ToString(dateFormat), FilterOperator.LessEqual, FilterDataType.Date);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(item.Key, item.Value.ToString(dateFormat), FilterOperator.GreaterEqual, FilterDataType.Date);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(item.Key, item.Value.AddDays(1).ToString(dateFormat), FilterOperator.Less, FilterDataType.Date);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(item.Key, item.Value.AddDays(-1).ToString(dateFormat), FilterOperator.Greater, FilterDataType.Date);
            response.People.OrderBy(p => p.Id).MatchSnapshot();
        }
    }

    [Theory]
    [InlineData(28, FilterOperator.Equals)]
    [InlineData(28, FilterOperator.LessEqual)]
    [InlineData(28, FilterOperator.GreaterEqual)]
    [InlineData(29, FilterOperator.Less)]
    [InlineData(27, FilterOperator.Greater)]
    public async Task WhenFilterByAge_ShouldResolvePerson(int filterValue, FilterOperator filterOperator)
    {
        var response = await GetAllByFilter(FilterReference.Age, filterValue.ToString(), filterOperator, FilterDataType.Numeric);
        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Fact]
    public async Task WhenFilterByVnString_ShouldResolvePerson()
    {
        var response = await GetAllByFilter(FilterReference.Vn, "756.3521.9874.24", FilterOperator.Equals, FilterDataType.String);
        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Fact]
    public async Task WhenFilterByContactAddressLine17_ShouldResolvePerson()
    {
        var filterValues = new[]
        {
            PersonMockedData.Person_3203_StGallen_5.ContactAddressLine1!,
            PersonMockedData.Person_3203_StGallen_5.ContactAddressLine2!,
            PersonMockedData.Person_3203_StGallen_5.ContactAddressLine3!,
            PersonMockedData.Person_3203_StGallen_5.ContactAddressLine4!,
            PersonMockedData.Person_3203_StGallen_5.ContactAddressLine5!,
            PersonMockedData.Person_3203_StGallen_5.ContactAddressLine6!,
            PersonMockedData.Person_3203_StGallen_5.ContactAddressLine7!,
        };

        foreach (var value in filterValues)
        {
            var response = await GetAllByFilter(FilterReference.ContactAddressLine17, value, FilterOperator.Equals, FilterDataType.String);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(FilterReference.ContactAddressLine17, value[1..^1], FilterOperator.Contains, FilterDataType.String);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(FilterReference.ContactAddressLine17, value[..^1], FilterOperator.StartsWith, FilterDataType.String);
            response.People.OrderBy(p => p.Id).MatchSnapshot();

            response = await GetAllByFilter(FilterReference.ContactAddressLine17, value[1..], FilterOperator.EndsWith, FilterDataType.String);
            response.People.OrderBy(p => p.Id).MatchSnapshot();
        }
    }

    [Theory]
    [InlineData(FilterReference.TypeOfResidence, nameof(ResidenceType.HWS), FilterDataType.Select, FilterOperator.Equals)]
    [InlineData(FilterReference.SwissCitizenship, nameof(SwissCitizenship.Yes), FilterDataType.Select, FilterOperator.Equals)]
    [InlineData(FilterReference.ResidencePermit, "30", FilterDataType.Select, FilterOperator.Equals)]
    [InlineData(FilterReference.Sex, nameof(SexType.Female), FilterDataType.Multiselect, FilterOperator.Equals)]
    [InlineData(FilterReference.Sex, nameof(SexType.Female), FilterDataType.Multiselect, FilterOperator.Contains)]
    [InlineData(FilterReference.Religion, nameof(ReligionType.Evangelic), FilterDataType.Multiselect, FilterOperator.Equals)]
    [InlineData(FilterReference.Religion, nameof(ReligionType.Evangelic), FilterDataType.Multiselect, FilterOperator.Contains)]
    [InlineData(FilterReference.Country, "CH", FilterDataType.Multiselect, FilterOperator.Equals)]
    [InlineData(FilterReference.Country, "CH", FilterDataType.Multiselect, FilterOperator.Contains)]
    public async Task WhenFilterByEnum_ShouldResolvePerson(FilterReference filterReference, string filterValue, FilterDataType filterDataType, FilterOperator filterOperator)
    {
        var response = await GetAllByFilter(filterReference, filterValue, filterOperator, filterDataType);
        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Theory]
    [InlineData(FilterOperator.Equals)]
    [InlineData(FilterOperator.Contains)]
    public async Task WhenFilterBySex_Multiselect_ShouldResolvePerson(FilterOperator filterOperator)
    {
        var filter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = FilterReference.Sex,
            FilterValue = $"{nameof(SexType.Female)},{nameof(SexType.Male)},3",
            FilterDataType = FilterDataType.Multiselect,
            FilterOperator = filterOperator,
        };

        var request = NewValidGetAllRequest(x => x.Criteria.Add(filter));
        var response = await SgManagerClient.GetAllAsync(request);

        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Theory]
    [InlineData(FilterOperator.Equals)]
    [InlineData(FilterOperator.Contains)]
    public async Task WhenFilterByReligion_Multiselect_ShouldResolvePerson(FilterOperator filterOperator)
    {
        var filter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = FilterReference.Religion,
            FilterValue = $"{nameof(ReligionType.Evangelic)},121",
            FilterDataType = FilterDataType.Multiselect,
            FilterOperator = filterOperator,
        };

        var request = NewValidGetAllRequest(x => x.Criteria.Add(filter));
        var response = await SgManagerClient.GetAllAsync(request);

        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Theory]
    [InlineData(FilterOperator.Equals)]
    [InlineData(FilterOperator.Contains)]
    public async Task WhenFilterByCountry_Multiselect_ShouldResolvePerson(FilterOperator filterOperator)
    {
        var filter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = FilterReference.Country,
            FilterValue = "CH, DE",
            FilterDataType = FilterDataType.Multiselect,
            FilterOperator = filterOperator,
        };

        var request = NewValidGetAllRequest(x => x.Criteria.Add(filter));
        var response = await SgManagerClient.GetAllAsync(request);

        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Theory]
    [InlineData(FilterReference.DateOfBirth)]
    [InlineData(FilterReference.ResidencePermitValidFrom)]
    [InlineData(FilterReference.ResidencePermitValidTill)]
    [InlineData(FilterReference.ResidenceEntryDate)]
    [InlineData(FilterReference.MoveInArrivalDate)]
    public async Task WhenFilterByInValidDate_ShouldThrow(FilterReference filterReference)
    {
        const string filterValue = "20021206";
        var inValidFilter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = filterReference,
            FilterValue = filterValue,
            FilterOperator = FilterOperator.Equals,
            FilterDataType = FilterDataType.Date,
        };

        await AssertStatus(
            async () => await SgManagerClient.GetAllAsync(
                NewValidGetAllRequest(x => x.Criteria.Add(inValidFilter))),
            StatusCode.InvalidArgument,
            $"Value '{filterValue}' not parsable into member type '{nameof(DateOnly)}'");
    }

    [Theory]
    [InlineData(FilterReference.RestrictedVotingAndElectionRightFederation)]
    [InlineData(FilterReference.EVoting)]
    [InlineData(FilterReference.DateOfBirthAdjusted)]
    [InlineData(FilterReference.MoveInUnknown)]
    [InlineData(FilterReference.HasValidationErrors)]
    [InlineData(FilterReference.SendVotingCardsToDomainOfInfluenceReturnAddress)]
    public async Task WhenFilterByInValidBoolean_ShouldThrow(FilterReference filterReference)
    {
        const string filterValue = "Test";
        var inValidFilter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = filterReference,
            FilterValue = filterValue,
            FilterOperator = FilterOperator.Equals,
            FilterDataType = FilterDataType.Boolean,
        };

        await AssertStatus(
            async () => await SgManagerClient.GetAllAsync(
                NewValidGetAllRequest(x => x.Criteria.Add(inValidFilter))),
            StatusCode.InvalidArgument,
            $"Value '{filterValue}' not parsable into member type '{nameof(Boolean)}'");
    }

    [Theory]
    [InlineData(FilterReference.MunicipalityId)]
    [InlineData(FilterReference.Age)]
    public async Task WhenFilterByInValidNumber_ShouldThrow(FilterReference filterReference)
    {
        const string filterValue = "Test";
        var inValidFilter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = filterReference,
            FilterValue = filterValue,
            FilterOperator = FilterOperator.Equals,
            FilterDataType = FilterDataType.Numeric,
        };

        await AssertStatus(
            async () => await SgManagerClient.GetAllAsync(
                NewValidGetAllRequest(x => x.Criteria.Add(inValidFilter))),
            StatusCode.InvalidArgument,
            $"Value '{filterValue}' not parsable into member type '{nameof(Int32)}'");
    }

    [Theory]
    [InlineData(FilterReference.TypeOfResidence, $"{nameof(ResidenceType.NWS)},{nameof(ResidenceType.HWS)}")]
    [InlineData(FilterReference.ResidencePermit, "01,02")]
    [InlineData(FilterReference.SwissCitizenship, $"{nameof(SwissCitizenship.Yes)},{nameof(SwissCitizenship.No)}")]
    public async Task WhenFilterByEnum_SelectMultipleValues_ShouldThrow(FilterReference filterReference, string filterValue)
    {
        var filter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = filterReference,
            FilterValue = filterValue,
            FilterDataType = FilterDataType.Select,
            FilterOperator = FilterOperator.Equals,
        };

        await AssertStatus(
            async () => await SgManagerClient.GetAllAsync(
                NewValidGetAllRequest(x => x.Criteria.Add(filter))),
            StatusCode.InvalidArgument,
            $"Multiple values are not allowed for FilterDataType '{FilterDataType.Select}'");
    }

    [Fact]
    public async Task WhenFilter_InvalidOperator_ShouldThrow()
    {
        foreach (var item in FilterData.ValidOperatorsByFilter)
        {
            var invalidFilterOperators = Enum.GetValues<FilterOperator>()
                .Except(item.Value.Concat(new[] { FilterOperator.Unspecified }));
            await GetAllWithInvalidOperatorsAndAssertStatus(item.Key, invalidFilterOperators);
        }
    }

    [Fact]
    public async Task WhenFilter_InvalidDataType_ShouldThrow()
    {
        foreach (var item in FilterData.ValidDataTypesByFilter)
        {
            var invalidFilterDataTypes = Enum.GetValues<FilterDataType>()
                .Except(item.Value.Concat(new[] { FilterDataType.Unspecified }));
            await GetAllWithInvalidDataTypesAndAssertStatus(item.Key, invalidFilterDataTypes, "FilterDataType '{0}' is not valid for selected field type '{1}'");
        }
    }

    [Fact]
    public async Task WhenFilterByCalculatedField_InvalidDataType_ShouldThrow()
    {
        foreach (var item in FilterData.ValidDataTypesByCalculatedFilter)
        {
            var invalidFilterDataTypes = Enum.GetValues<FilterDataType>()
                .Except(item.Value.Concat(new[] { FilterDataType.Unspecified }));
            await GetAllWithInvalidDataTypesAndAssertStatus(item.Key, invalidFilterDataTypes, $"FilterDataType '{{0}}' is not valid for filter '{item.Key}'");
        }
    }

    [Fact]
    public async Task WhenNoFilterAndRoleUnauthorized_ShouldPermissionDenied()
    {
        var rolesArray = new[]
        {
            Roles.ApiImporter,
            Roles.ApiExporter,
            Roles.ManualImporter,
            Roles.ManualExporter,
            Roles.ImportObserver,
        };

        var filter = new FilterCriteriaModel();
        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));

        await AssertStatus(
            async () => await client.GetAllAsync(
                NewValidGetAllRequest(x => x.Criteria.Add(filter))),
            StatusCode.PermissionDenied);
    }

    [Theory]
    [InlineData(FilterOperator.Contains)]
    [InlineData(FilterOperator.StartsWith)]
    [InlineData(FilterOperator.EndsWith)]
    public async Task WhenFilterByAge_InvalidOperator_ShouldThrow(FilterOperator filterOperator)
    {
        var filter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = FilterReference.Age,
            FilterValue = "Test",
            FilterDataType = FilterDataType.Numeric,
            FilterOperator = filterOperator,
        };

        await AssertStatus(
            async () => await SgManagerClient.GetAllAsync(
                NewValidGetAllRequest(x => x.Criteria.Add(filter))),
            StatusCode.InvalidArgument,
            $"FilterOperator '{filterOperator}' is not valid for filter '{FilterReference.Age}'");
    }

    [Theory]
    [InlineData(FilterReference.Unspecified, FilterDataType.String, FilterOperator.Equals)]
    [InlineData(FilterReference.FirstName, FilterDataType.Unspecified, FilterOperator.Equals)]
    [InlineData(FilterReference.FirstName, FilterDataType.String, FilterOperator.Unspecified)]
    public async Task WhenFilterPropertiesAreUnknown_ShouldThrow(FilterReference filterReference, FilterDataType filterDataType, FilterOperator filterOperator)
    {
        var inValidFilter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = filterReference,
            FilterValue = "Test",
            FilterDataType = filterDataType,
            FilterOperator = filterOperator,
        };

        await AssertStatus(
            async () => await SgManagerClient.GetAllAsync(
                NewValidGetAllRequest(x => x.Criteria.Add(inValidFilter))),
            StatusCode.InvalidArgument);
    }

    [Fact]
    public async Task WhenSwissCitizenshipYesAndMunicipalityId9170AndAgeGreater18_ShouldResolvePerson()
    {
        var request = NewValidGetAllRequest(x =>
        {
            x.Criteria.Add(CreateFilter(FilterReference.MunicipalityId, dataType: FilterDataType.Numeric, value: "9170"));
            x.Criteria.Add(CreateFilter(FilterReference.RestrictedVotingAndElectionRightFederation, dataType: FilterDataType.Boolean, value: "False"));
            x.Criteria.Add(CreateFilter(FilterReference.Age, filterOperator: FilterOperator.GreaterEqual, dataType: FilterDataType.Numeric, value: "18"));
            x.Criteria.Add(CreateFilter(FilterReference.SwissCitizenship, dataType: FilterDataType.Select, value: nameof(SwissCitizenship.Yes)));
        });
        var response = await SgManagerClient.GetAllAsync(request);
        Assert.Equal(2, response.TotalCount);
        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Fact]
    public async Task WhenSwissCitizenshipYesAndMunicipalityId9170WithoutAgeFilter_ShouldResolvePerson()
    {
        var request = NewValidGetAllRequest(x =>
        {
            x.Criteria.Add(CreateFilter(FilterReference.MunicipalityId, dataType: FilterDataType.Numeric, value: "9170"));
            x.Criteria.Add(CreateFilter(FilterReference.RestrictedVotingAndElectionRightFederation, dataType: FilterDataType.Boolean, value: "False"));
            x.Criteria.Add(CreateFilter(FilterReference.SwissCitizenship, dataType: FilterDataType.Select, value: nameof(SwissCitizenship.Yes)));
        });
        var response = await SgManagerClient.GetAllAsync(request);
        Assert.Equal(3, response.TotalCount);
        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Fact]
    public async Task WhenSwissCitizenshipYesAndMunicipalityId9170WithRestrictedTrue_ShouldResolvePerson()
    {
        var request = NewValidGetAllRequest(x =>
        {
            x.Criteria.Add(CreateFilter(FilterReference.MunicipalityId, dataType: FilterDataType.Numeric, value: "9170"));
            x.Criteria.Add(CreateFilter(FilterReference.RestrictedVotingAndElectionRightFederation, dataType: FilterDataType.Boolean, value: "True"));
            x.Criteria.Add(CreateFilter(FilterReference.Age, filterOperator: FilterOperator.GreaterEqual, dataType: FilterDataType.Numeric, value: "18"));
            x.Criteria.Add(CreateFilter(FilterReference.SwissCitizenship, dataType: FilterDataType.Select, value: nameof(SwissCitizenship.Yes)));
        });
        var response = await SgManagerClient.GetAllAsync(request);
        Assert.Equal(1, response.TotalCount);
        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Fact]
    public async Task WhenSwissCitizenshipNoAndMunicipalityId9170AndAgeGreater18_ShouldResolvePerson()
    {
        var request = NewValidGetAllRequest(x =>
        {
            x.Criteria.Add(CreateFilter(FilterReference.MunicipalityId, dataType: FilterDataType.Numeric, value: "9170"));
            x.Criteria.Add(CreateFilter(FilterReference.RestrictedVotingAndElectionRightFederation, dataType: FilterDataType.Boolean, value: "False"));
            x.Criteria.Add(CreateFilter(FilterReference.Age, filterOperator: FilterOperator.GreaterEqual, dataType: FilterDataType.Numeric, value: "18"));
            x.Criteria.Add(CreateFilter(FilterReference.SwissCitizenship, dataType: FilterDataType.Select, value: nameof(SwissCitizenship.No)));
        });
        var response = await SgManagerClient.GetAllAsync(request);
        Assert.Equal(1, response.TotalCount);
        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Fact]
    public async Task WhenSwissCitizenshipNo_ShouldCheckForResidenceValidDate_ShouldResolvePerson()
    {
        var request = NewValidGetAllRequest(x =>
        {
            x.Criteria.Add(CreateFilter(FilterReference.MunicipalityId, dataType: FilterDataType.Numeric, value: "3203"));
            x.Criteria.Add(CreateFilter(FilterReference.RestrictedVotingAndElectionRightFederation, dataType: FilterDataType.Boolean, value: "False"));
            x.Criteria.Add(CreateFilter(FilterReference.Age, filterOperator: FilterOperator.GreaterEqual, dataType: FilterDataType.Numeric, value: "18"));
            x.Criteria.Add(CreateFilter(FilterReference.SwissCitizenship, dataType: FilterDataType.Select, value: nameof(SwissCitizenship.No)));
        });
        var response = await SgManagerClient.GetAllAsync(request);
        response.People.OrderBy(p => p.Id).MatchSnapshot();
    }

    [Theory]
    [InlineData(FilterOperator.Equals, 29)]
    [InlineData(FilterOperator.Less, 18)]
    [InlineData(FilterOperator.LessEqual, 18)]
    [InlineData(FilterOperator.Greater, 18)]
    [InlineData(FilterOperator.GreaterEqual, 18)]
    public async Task WhenFilterByAge_AllPersons_ShouldResolvePerson(FilterOperator filterOperator, int age)
    {
        var referenceDate = DateOnly.FromDateTime(MockedClock.GetDate()).AddYears(-age);
        var persons = await AgeTest(filterOperator, age);

        persons.Should().HaveCountGreaterThanOrEqualTo(1);

        foreach (var person in persons)
        {
            var dateOfBirth = DateOnly.FromDateTime(person.DateOfBirth.ToDateTime());
            switch (filterOperator)
            {
                case FilterOperator.Equals:
                    dateOfBirth.Should().BeOnOrAfter(referenceDate.AddYears(-1).AddDays(1));
                    dateOfBirth.Should().BeOnOrBefore(referenceDate);
                    break;
                case FilterOperator.Less:
                    dateOfBirth.Should().BeOnOrAfter(referenceDate.AddDays(1));
                    break;
                case FilterOperator.LessEqual:
                    dateOfBirth.Should().BeOnOrAfter(referenceDate.AddYears(-1).AddDays(1));
                    break;
                case FilterOperator.Greater:
                    dateOfBirth.Should().BeOnOrBefore(referenceDate);
                    break;
                case FilterOperator.GreaterEqual:
                    dateOfBirth.Should().BeOnOrBefore(referenceDate.AddYears(-1));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    protected override async Task AuthorizationTestCall(PersonService.PersonServiceClient service)
    {
        await service.GetAllAsync(NewValidGetAllRequest());
    }

    private static PersonServiceGetAllRequest NewValidGetAllRequest(Action<PersonServiceGetAllRequest>? customizer = null)
    {
        var request = new PersonServiceGetAllRequest
        {
            Paging = new PagingModel
            {
                PageIndex = 0,
                PageSize = 10,
            },
            SearchType = PersonSearchType.Person,
        };

        customizer?.Invoke(request);
        return request;
    }

    private static FilterCriteriaModel CreateFilter(
        FilterReference reference,
        FilterOperator filterOperator = FilterOperator.Equals,
        FilterDataType dataType = FilterDataType.String,
        string value = "") =>
        new()
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = reference,
            FilterValue = value,
            FilterDataType = dataType,
            FilterOperator = filterOperator,
        };

    private async Task<PersonModel[]> AgeTest(
    FilterOperator filterOperator,
    int age)
    {
        var request = NewValidGetAllRequest(x =>
        {
            x.Criteria.Add(CreateFilter(
                reference: FilterReference.Age,
                filterOperator: filterOperator,
                dataType: FilterDataType.Numeric,
                value: age.ToString()));
        });
        var response = await SgManagerClient.GetAllAsync(request);
        return response.People.OrderBy(p => p.Id).ToArray();
    }

    private async Task<PersonServiceGetAllResponse> GetAllByFilter(FilterReference filterReference, string filterValue, FilterOperator filterOperator, FilterDataType filterDataType)
    {
        var filter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = filterReference,
            FilterValue = filterValue,
            FilterDataType = filterDataType,
            FilterOperator = filterOperator,
        };

        // add person filter to reduce search result to one person, this allows to reuse this test since the snapshot should be equal
        var personFilter = new FilterCriteriaModel
        {
            Id = Guid.Empty.ToString(),
            ReferenceId = FilterReference.Vn,
            FilterValue = "756.3521.9874.24",
            FilterDataType = FilterDataType.String,
            FilterOperator = FilterOperator.Equals,
        };

        var request = NewValidGetAllRequest(x => x.Criteria.AddRange(new[] { filter, personFilter }));
        return await SgManagerClient.GetAllAsync(request);
    }

    private async Task GetAllWithInvalidDataTypesAndAssertStatus(FilterReference filterReference, IEnumerable<FilterDataType> invalidFilterDataTypes, string statusContent)
    {
        foreach (var invalidFilterDataType in invalidFilterDataTypes)
        {
            var filter = new FilterCriteriaModel
            {
                Id = Guid.Empty.ToString(),
                ReferenceId = filterReference,
                FilterValue = "Test",
                FilterDataType = invalidFilterDataType,
                FilterOperator = FilterOperator.Equals,
            };

            var memberType = typeof(PersonEntity).GetProperty(filterReference.ToString())?.PropertyType;
            if (memberType != null && memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var arg = memberType.GetGenericArguments();
                if (arg.Length > 0)
                {
                    memberType = arg[0];
                }
            }

            await AssertStatus(
                async () => await SgManagerClient.GetAllAsync(
                    NewValidGetAllRequest(x => x.Criteria.Add(filter))),
                StatusCode.InvalidArgument,
                string.Format(statusContent, invalidFilterDataType, memberType));
        }
    }

    private async Task GetAllWithInvalidOperatorsAndAssertStatus(FilterReference filterReference, IEnumerable<FilterOperator> invalidFilterOperators)
    {
        var filterDataType = FilterData.ValidDataTypesByFilter.TryGetValue(filterReference, out var value) ? value[0] : FilterData.ValidDataTypesByCalculatedFilter[filterReference][0];
        foreach (var invalidFilterOperator in invalidFilterOperators)
        {
            var filter = new FilterCriteriaModel
            {
                Id = Guid.Empty.ToString(),
                ReferenceId = filterReference,
                FilterValue = "Test",
                FilterDataType = filterDataType,
                FilterOperator = invalidFilterOperator,
            };

            await AssertStatus(
                async () => await SgManagerClient.GetAllAsync(
                    NewValidGetAllRequest(x => x.Criteria.Add(filter))),
                StatusCode.InvalidArgument,
                $"FilterOperator '{invalidFilterOperator}' is not valid for selected FilterDataType '{filterDataType}'");
        }
    }
}
