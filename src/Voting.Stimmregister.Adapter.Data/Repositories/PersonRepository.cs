// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.Lib.Database.Models;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Constants;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

/// <inheritdoc cref="IPersonRepository"/>
public class PersonRepository : DbRepository<DataContext, PersonEntity>, IPersonRepository
{
    private const char MultipleFilterSeparator = ',';

    private readonly ILogger<PersonRepository> _logger;

    public PersonRepository(ILogger<PersonRepository> logger, DataContext context)
        : base(context)
    {
        _logger = logger;
    }

    public Task<Dictionary<Guid, PersonEntity>> GetLatestPersonsByBfsNumberIgnoreAcl(int municipalityId)
    {
        // ignore the query filters (acl)
        // as this filters by municipality id anyway
        // and should only be consumed by an importer which runs asynchronously without user context anyway.
        // this query uses an index, if adjusted, the index may need adjustments too! (see PersonModelBuilder)
        return Set
            .IgnoreQueryFilters()
            .Include(p => p.PersonDois)
            .Where(p => p.IsLatest && p.MunicipalityId == municipalityId)
            .ToDictionaryAsync(p => p.Id);
    }

    public IAsyncEnumerable<PersonEntity> StreamLatestByBfsIgnoreAclAndDeleted(int municipalityId)
    {
        // ignore the query filters (acl)
        // as this filters by municipality id anyway
        // and should only be consumed by an importer which runs asynchronously without user context anyway.
        // this query uses an index, if adjusted, the index may need adjustments too! (see PersonModelBuilder)
        return Set
            .IgnoreQueryFilters()
            .Include(p => p.PersonDois)
            .Where(p => p.IsLatest && !p.IsDeleted && p.MunicipalityId == municipalityId)
            .OrderBy(x => x.Id) // this query is needed to verify the persons signature, therefore sort by id
            .AsAsyncEnumerable();
    }

    /// <inheritdoc />
    public async Task<PersonSearchResultPage<PersonEntity>> GetPersonByFilter(IReadOnlyCollection<FilterCriteriaEntity> criteria, DateOnly referenceKeyDate, bool includeDois, Pageable? paging)
    {
        var queryable = QueryIsLatest();
        if (includeDois)
        {
            queryable = queryable.Include(x => x.PersonDois);
        }

        // if the order is adjusted,
        // the index probably needs adjustments too
        queryable = FilterForCriterias(queryable, criteria, referenceKeyDate)
            .OrderBy(i => i.OfficialName).ThenBy(p => p.FirstName);
        return await ExecuteSearchQueryWithInvalidCount(queryable, paging);
    }

    public async Task<PersonCountsModel> GetCountsByFilter(IReadOnlyCollection<FilterCriteriaEntity> criteria, DateOnly referenceKeyDate)
    {
        var queryable = QueryIsLatest();
        queryable = FilterForCriterias(queryable, criteria, referenceKeyDate);
        var personsCount = await queryable.CountAsync();
        var personsInvalidCount = await queryable.CountAsync(x => !x.IsValid);
        return new PersonCountsModel(personsCount, personsInvalidCount);
    }

    /// <inheritdoc />
    public async Task<PersonSearchResultPage<PersonEntity>> GetPersonsByFilterVersionId(Guid filterVersionId, Pageable? paging)
    {
        var queryable = Context.FilterVersionPersons
            .Where(version => version.FilterVersionId == filterVersionId)
            .Include(version => version.Person)
            .Select(version => version.Person!);

        queryable = queryable.OrderBy(i => i.OfficialName).ThenBy(p => p.FirstName);
        return await ExecuteSearchQueryWithInvalidCount(queryable, paging);
    }

    /// <inheritdoc />
    public async Task<PersonEntity> GetPersonByRegisterIdIncludingDoIs(Guid personRegisterId)
    {
        var searchResult = await QueryIsLatest()
            .Include(x => x.PersonDois)
            .Where(p => p.RegisterId.Equals(personRegisterId)) // RegisterId is unique per person
            .ToListAsync();

        foreach (var result in searchResult)
        {
            // sort person dois in memory instead of inside the database
            // since we want to sort by the int representation of the doi type
            // but we store the string representation of the enum
            result.PersonDois = result.PersonDois
                .OrderBy(x => x.DomainOfInfluenceType)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Identifier)
                .ToList();
        }

        if (searchResult.Count != 1)
        {
            throw new InvalidSearchFilterCriteriaException("No person found for register Id.");
        }

        return searchResult[0];
    }

    /// <inheritdoc />
    public async Task<PersonEntity?> GetSingleOrDefaultWithVotingRightsByVnAndCantonBfsIgnoreAcl(long vn, short cantonBfs)
    {
        return await Set
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.IsLatest && !p.IsDeleted && !p.RestrictedVotingAndElectionRightFederation && p.Vn == vn && p.CantonBfs == cantonBfs);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<PersonEntity> StreamPersonsByFilterVersion(Guid filterVersionId)
        => QueryFilterVersionPersons(filterVersionId).AsAsyncEnumerable();

    /// <inheritdoc />
    public async Task<PersonSearchStreamedResultModel<PersonEntity>> StreamPersonsByFilterVersionWithCount(Guid filterVersionId)
    {
        var queryable = QueryFilterVersionPersons(filterVersionId);
        var invalidCount = await queryable.CountAsync(x => !x.IsValid);
        var count = await queryable.CountAsync();
        return new(count, invalidCount, queryable.AsAsyncEnumerable());
    }

    public async Task<IReadOnlySet<Guid>> ListPersonIdsInclInvalid(IReadOnlyCollection<FilterCriteriaEntity> criteria, DateOnly referenceKeyDate)
    {
        var queryable = QueryIsLatest();
        var ids = await FilterForCriterias(queryable, criteria, referenceKeyDate)
            .Select(x => x.Id)
            .ToListAsync();
        return ids.ToHashSet();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<PersonEntity> StreamPersons(
        IReadOnlyCollection<FilterCriteriaEntity> criteria,
        DateOnly referenceKeyDate)
    {
        return QueryPersons(criteria, referenceKeyDate).AsAsyncEnumerable();
    }

    /// <inheritdoc />
    public async Task<PersonSearchStreamedResultModel<PersonEntity>> StreamPersonsWithCounts(
        IReadOnlyCollection<FilterCriteriaEntity> criteria,
        DateOnly referenceKeyDate)
    {
        var queryable = QueryPersons(criteria, referenceKeyDate);
        var invalidCount = await queryable.CountAsync(x => !x.IsValid);
        var count = await queryable.CountAsync();
        return new(count, invalidCount, queryable.AsAsyncEnumerable());
    }

    private static void AssertFilterCriteriaIsValid(FilterCriteriaEntity criteria)
    {
        if (string.IsNullOrEmpty(criteria.FilterValue) ||
            criteria.ReferenceId == FilterReference.Unspecified ||
            !(HasProperty(criteria.ReferenceId.ToString()) || IsCalculatedFilterField(criteria.ReferenceId)) ||
            criteria.FilterType == FilterDataType.Unspecified ||
            criteria.FilterOperator == FilterOperatorType.Unspecified)
        {
            throw new InvalidSearchFilterCriteriaException(criteria);
        }
    }

    private static bool HasProperty(string propertyName)
    {
        return typeof(PersonEntity).GetProperty(propertyName) != null;
    }

    private static bool IsCalculatedFilterField(FilterReference filterReference)
    {
        return filterReference == FilterReference.Age
            || filterReference == FilterReference.SwissCitizenship
            || filterReference == FilterReference.OriginName17
            || filterReference == FilterReference.OriginOnCanton17
            || filterReference == FilterReference.ContactAddressLine17
            || filterReference == FilterReference.HasValidationErrors
            || IsCircleIdProperty(filterReference)
            || IsCircleNameProperty(filterReference);
    }

    private static bool IsCircleNameProperty(FilterReference filterReference)
    {
        return filterReference == FilterReference.PoliticalCircleName
            || filterReference == FilterReference.TrafficCircleName
            || filterReference == FilterReference.CatholicCircleName
            || filterReference == FilterReference.EvangelicCircleName
            || filterReference == FilterReference.PeopleCircleName
            || filterReference == FilterReference.ResidentialDistrictCircleName
            || filterReference == FilterReference.SchoolCircleName;
    }

    private static bool IsCircleIdProperty(FilterReference filterReference)
    {
        return filterReference == FilterReference.PoliticalCircleId
            || filterReference == FilterReference.TrafficCircleId
            || filterReference == FilterReference.CatholicCircleId
            || filterReference == FilterReference.EvangelicCircleId
            || filterReference == FilterReference.PeopleCircleId
            || filterReference == FilterReference.ResidentialDistrictCircleId
            || filterReference == FilterReference.SchoolCircleId;
    }

    private static Type GetMemberType<T>(string propertyName)
    {
        var memberType = typeof(T).GetProperty(propertyName)?.PropertyType
            ?? throw new InvalidSearchFilterCriteriaException($"Member type could not be resolved for field: '{propertyName}'");

        if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var arg = memberType.GetGenericArguments();
            if (arg.Length > 0)
            {
                memberType = arg[0];
            }
        }

        return memberType;
    }

    private static void AssertFilterOperatorIsValidForFilterDataType(FilterOperatorType filterOperator, FilterDataType filterDataType)
    {
        var isFilterOperatorValid = filterOperator switch
        {
            FilterOperatorType.Contains => filterDataType
                is FilterDataType.String
                or FilterDataType.Multiselect,
            FilterOperatorType.Equals => filterDataType
                is FilterDataType.String
                or FilterDataType.Date
                or FilterDataType.Boolean
                or FilterDataType.Numeric
                or FilterDataType.Select
                or FilterDataType.Multiselect,
            FilterOperatorType.EndsWith => filterDataType == FilterDataType.String,
            FilterOperatorType.StartsWith => filterDataType == FilterDataType.String,

            FilterOperatorType.Less => filterDataType == FilterDataType.Date,
            FilterOperatorType.LessEqual => filterDataType == FilterDataType.Date,
            FilterOperatorType.Greater => filterDataType == FilterDataType.Date,
            FilterOperatorType.GreaterEqual => filterDataType == FilterDataType.Date,
            _ => false,
        };

        if (!isFilterOperatorValid)
        {
            throw new InvalidSearchFilterCriteriaException($"FilterOperator '{filterOperator}' is not valid for selected FilterDataType '{filterDataType}'");
        }
    }

    private static void AssertFilterValueIsValidForFilterDataType(string filterValue, FilterDataType filterDataType)
    {
        if (filterDataType != FilterDataType.Select)
        {
            return;
        }

        if (filterValue.Contains(MultipleFilterSeparator))
        {
            throw new InvalidSearchFilterCriteriaException($"Multiple values are not allowed for FilterDataType '{FilterDataType.Select}'");
        }
    }

    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForAge(
       ParameterExpression parameterExpression,
       MemberExpression memberExpression,
       FilterCriteriaEntity filterCriteria,
       DateOnly referenceKeyDate)
    {
        if (!int.TryParse(filterCriteria.FilterValue, out var parsedAgeValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(Int32)}'");
        }

        var ageDate = referenceKeyDate.AddYears(-parsedAgeValue);
        return filterCriteria.FilterOperator switch
        {
            FilterOperatorType.Equals =>
                BuildFilterExpressionForDateOnlyBetween(parameterExpression, memberExpression, ageDate.AddYears(-1).AddDays(1), ageDate),
            FilterOperatorType.LessEqual =>
                BuildFilterExpressionForDateOnlyBetween(parameterExpression, memberExpression, ageDate.AddYears(-1).AddDays(1), DateOnly.MaxValue),
            FilterOperatorType.Less =>
                BuildFilterExpressionForDateOnlyBetween(parameterExpression, memberExpression, ageDate.AddDays(1), DateOnly.MaxValue),
            FilterOperatorType.GreaterEqual =>
                BuildFilterExpressionForDateOnlyBetween(parameterExpression, memberExpression, DateOnly.MinValue, ageDate),
            FilterOperatorType.Greater =>
                BuildFilterExpressionForDateOnlyBetween(parameterExpression, memberExpression, DateOnly.MinValue, ageDate.AddYears(-1)),
            _ => throw new Exception(),
        };
    }

    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForDateOnlyBetween(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        DateOnly fromDate,
        DateOnly toDate)
    {
        var gteFromDateExpression = Expression.GreaterThanOrEqual(
            memberExpression, Expression.Constant(fromDate));
        var lteToDateExpression = Expression.LessThanOrEqual(
            memberExpression, Expression.Constant(toDate));
        var methodCall = Expression.And(gteFromDateExpression, lteToDateExpression);
        return Expression.Lambda<Func<PersonEntity, bool>>(methodCall, parameterExpression);
    }

    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForHasValidationErrors(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        if (!bool.TryParse(filterCriteria.FilterValue, out var parsedHasValidationErrorsValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(Boolean)}'");
        }

        Expression methodCall = Expression.Call(memberExpression, typeof(bool).GetMethod(nameof(bool.Equals), new[] { typeof(bool), })!, Expression.Constant(!parsedHasValidationErrorsValue));
        return Expression.Lambda<Func<PersonEntity, bool>>(methodCall, parameterExpression);
    }

    private static Expression<Func<PersonEntity, bool>>? BuildFilterExpressionForSwissCitizenship(
        ParameterExpression parameterExpression,
        Expression countryMemberExpression,
        DateOnly referenceKeyDate,
        FilterCriteriaEntity filterCriteria)
    {
        var residencePermitValidFromMemberExpression =
            Expression.Property(parameterExpression, nameof(PersonEntity.ResidencePermitValidFrom));
        var residencePermitValidTillMemberExpression =
            Expression.Property(parameterExpression, nameof(PersonEntity.ResidencePermitValidTill));

        if (!Enum.TryParse(filterCriteria.FilterValue, true, out SwissCitizenship parsedSwissCitizenshipValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(SwissCitizenship)}'");
        }

        if (parsedSwissCitizenshipValue == SwissCitizenship.Undefined)
        {
            return null;
        }

        Expression methodCall = Expression.Call(
            countryMemberExpression,
            typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), })!,
            Expression.Constant(Countries.Switzerland));

        if (parsedSwissCitizenshipValue == SwissCitizenship.No)
        {
            methodCall = Expression.Not(methodCall);
            var residencePermitDateInRangeExpression = ResidencePermitDateInRangeExpression(residencePermitValidFromMemberExpression, residencePermitValidTillMemberExpression, referenceKeyDate);
            methodCall = Expression.And(methodCall, residencePermitDateInRangeExpression);
        }

        return Expression.Lambda<Func<PersonEntity, bool>>(methodCall, parameterExpression);
    }

    private static BinaryExpression ResidencePermitDateInRangeExpression(
        Expression residencePermitValidFromMemberExpression,
        Expression residencePermitValidTillMemberExpression,
        DateOnly referenceKeyDate)
    {
        var gteResidencePermitValidFromExpression = Expression.GreaterThanOrEqual(
            Expression.Constant(referenceKeyDate),
            Expression.Coalesce(residencePermitValidFromMemberExpression, Expression.Constant(DateOnly.MinValue)));
        var lteResidencePermitValidTillExpression = Expression.LessThanOrEqual(
            Expression.Constant(referenceKeyDate),
            Expression.Coalesce(residencePermitValidTillMemberExpression, Expression.Constant(DateOnly.MaxValue)));
        var gteResidencePermitValidFromExpressionWithNullCheck = Expression.Condition(
            Expression.Equal(residencePermitValidFromMemberExpression, Expression.Constant(null)),
            Expression.Constant(true),
            gteResidencePermitValidFromExpression);
        var lteResidencePermitValidTillExpressionWithNullCheck = Expression.Condition(
            Expression.Equal(residencePermitValidTillMemberExpression, Expression.Constant(null)),
            Expression.Constant(true),
            lteResidencePermitValidTillExpression);
        return Expression.And(gteResidencePermitValidFromExpressionWithNullCheck, lteResidencePermitValidTillExpressionWithNullCheck);
    }

    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForMultipleStrings(
        ParameterExpression parameterExpression,
        IEnumerable<MemberExpression> memberExpressions,
        FilterCriteriaEntity filterCriteria)
    {
        var methodInfo = typeof(string).GetMethod(filterCriteria.FilterOperator.ToString(), new Type[] { typeof(string), });
        AssertMethodNotNull<string>(filterCriteria, methodInfo);
        var orExpression = memberExpressions.Aggregate(default(Expression), (exp, memberExpression) =>
        {
            Expression expression = memberExpression;
            var filterValue = filterCriteria.FilterValue;
            ApplyStringCaseInsensitive(ref expression, ref filterValue); // case insensitive search workaround:

            var methodCall = Expression.Call(expression, methodInfo!, Expression.Constant(filterValue));
            return exp == null ? methodCall : Expression.Or(exp, methodCall);
        });
        return Expression.Lambda<Func<PersonEntity, bool>>(orExpression!, parameterExpression);
    }

    private static void AssertMethodNotNull<T>(FilterCriteriaEntity filterCriteria, MethodInfo? method)
    {
        if (method == null)
        {
            throw new InvalidSearchFilterCriteriaException($"Method '{filterCriteria.FilterOperator}' is not available for field type '{nameof(T)}'");
        }
    }

    private static void ApplyStringCaseInsensitive(ref Expression expression, ref string filterValue)
    {
        var toUpper = typeof(string).GetMethod(nameof(string.ToUpper), Array.Empty<Type>());
        expression = Expression.Call(expression, toUpper!);
        filterValue = filterValue.ToUpper();
    }

    private static void AssertFilterDataTypeIsValidForMemberType(FilterDataType filterDataType, Type memberType)
    {
        var isFilterDataTypeValid = filterDataType switch
        {
            FilterDataType.String => memberType == typeof(string) || memberType == typeof(Guid),
            FilterDataType.Boolean => memberType == typeof(bool),
            FilterDataType.Date => memberType == typeof(DateOnly) || memberType == typeof(DateTime),
            FilterDataType.Numeric => memberType == typeof(int) || memberType == typeof(long),
            FilterDataType.Select => memberType == typeof(string) || memberType == typeof(ResidenceType),
            FilterDataType.Multiselect => memberType == typeof(string)
                || memberType == typeof(SexType)
                || memberType == typeof(ReligionType),
            _ => false,
        };

        if (!isFilterDataTypeValid)
        {
            throw new InvalidSearchFilterCriteriaException($"FilterDataType '{filterDataType}' is not valid for selected field type '{memberType}'");
        }
    }

    /// <summary>
    /// Applies a non-null check to the specified filter expression if the datatype of memberExpression is nullable,
    /// otherwise the expression is returned directly.
    /// e.g. nullable: ((p.DateOfBirth != null) AndAlso Convert(p.DateOfBirth, DateOnly).Equals(01.01.2023))
    ///      not null: p.DateOfBirth.Equals(01.01.2023).
    /// </summary>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filter">The filter criteria as function.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression NotNullExpression(MemberExpression memberExpression, Func<Expression, Expression> filter)
    {
        var propertyInfo = memberExpression.Member as PropertyInfo;
        if (propertyInfo?.PropertyType.IsGenericType != true ||
            propertyInfo.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
        {
            return filter(memberExpression); // Expression without null check
        }

        var arg = propertyInfo.PropertyType.GetGenericArguments();
        if (arg.Length == 0)
        {
            return filter(memberExpression); // Expression without null check
        }

        var notNullCheck = Expression.NotEqual(memberExpression, Expression.Constant(null));
        var castToBaseType = Expression.Convert(memberExpression, arg[0]);
        return Expression.AndAlso(notNullCheck, filter(castToBaseType));
    }

    /// <summary>
    /// Builds a lambda expression built for the filter criteria for type <see cref="string"/>.
    /// The following methods must be handled:
    /// <list type="bullet">
    ///     <item>stringValue.Contains(filterValue);</item>
    ///     <item>stringValue.Equals(filterValue);</item>
    ///     <item>stringValue.EndsWith(filterValue);</item>
    ///     <item>stringValue.StartsWith(filterValue);</item>
    /// </list>
    /// </summary>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForString(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        var methodInfo = typeof(string).GetMethod(filterCriteria.FilterOperator.ToString(), new Type[]
        {
                typeof(string),
        });

        AssertMethodNotNull<string>(filterCriteria, methodInfo);
        Expression expression = memberExpression;
        var filterValue = filterCriteria.FilterValue;
        ApplyStringCaseInsensitive(ref expression, ref filterValue); // case insensitive search workaround:

        var methodCall = Expression.Call(expression, methodInfo!, Expression.Constant(filterValue));
        return Expression.Lambda<Func<PersonEntity, bool>>(methodCall, parameterExpression);
    }

    /// <summary>
    /// Builds a lambda expression built for the filter criteria for type <see cref="int"/>.
    /// The following methods must be handled:
    /// <list type="bullet">
    ///     <item>intValue.Equals(filterValue);</item>
    /// </list>
    /// </summary>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForInt(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        if (!int.TryParse(filterCriteria.FilterValue, out var parsedIntValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(Int32)}'");
        }

        return BuildFilterExpressionForComparable(parameterExpression, memberExpression, filterCriteria, parsedIntValue);
    }

    /// <summary>
    /// Builds a lambda expression built for the filter criteria for type <see cref="long"/>.
    /// The following methods must be handled:
    /// <list type="bullet">
    ///     <item>longValue.Equals(filterValue);</item>
    /// </list>
    /// </summary>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForLong(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        if (!long.TryParse(filterCriteria.FilterValue, out var parsedLongValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(Int64)}'");
        }

        return BuildFilterExpressionForComparable(parameterExpression, memberExpression, filterCriteria, parsedLongValue);
    }

    /// <summary>
    /// Builds a lambda expression built for the filter criteria for type <see cref="bool"/>.
    /// The following methods must be handled:
    /// <list type="bullet">
    ///     <item>boolValue.Equals(filterValue);</item>
    /// </list>
    /// </summary>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForBoolean(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        if (!bool.TryParse(filterCriteria.FilterValue, out var parsedBoolValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(Boolean)}'");
        }

        var methodInfo = typeof(bool).GetMethod(filterCriteria.FilterOperator.ToString(), new Type[]
        {
                typeof(bool),
        });

        AssertMethodNotNull<bool>(filterCriteria, methodInfo);

        var methodCall = NotNullExpression(memberExpression, e => Expression.Call(e, methodInfo!, Expression.Constant(parsedBoolValue)));

        return Expression.Lambda<Func<PersonEntity, bool>>(methodCall, parameterExpression);
    }

    /// <summary>
    /// Builds a lambda expression built for the filter criteria for type <see cref="Guid"/>.
    /// The following methods must be handled:
    /// <list type="bullet">
    ///     <item>boolValue.Equals(filterValue);</item>
    /// </list>
    /// </summary>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForGuid(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        if (!Guid.TryParse(filterCriteria.FilterValue, out var parsedGuidValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(Guid)}'");
        }

        var methodInfo = typeof(Guid).GetMethod(filterCriteria.FilterOperator.ToString(), new Type[]
        {
                typeof(Guid),
        });

        AssertMethodNotNull<Guid>(filterCriteria, methodInfo);

        var methodCall = Expression.Call(memberExpression, methodInfo!, Expression.Constant(parsedGuidValue));

        return Expression.Lambda<Func<PersonEntity, bool>>(methodCall, parameterExpression);
    }

    /// <summary>
    /// Builds a lambda expression built for the filter criteria for type <see cref="DateOnly"/>.
    /// The following methods must be handled:
    /// <list type="bullet">
    ///     <item>dateOnlyValue.Equals(filterValue);</item>
    /// </list>
    /// </summary>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForDateOnly(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        if (!DateOnly.TryParse(filterCriteria.FilterValue, out var parsedDateOnlyValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(DateOnly)}'");
        }

        return BuildFilterExpressionForComparable(parameterExpression, memberExpression, filterCriteria, parsedDateOnlyValue);
    }

    /// <summary>
    /// Builds a lambda expression built for the filter criteria for type <see cref="DateTime"/>.
    /// The following methods must be handled:
    /// <list type="bullet">
    ///     <item>dateTime.Equals(filterValue);</item>
    /// </list>
    /// </summary>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForDateTime(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        if (!DateTime.TryParse(filterCriteria.FilterValue, out var parsedDateTimeValue))
        {
            throw new InvalidSearchFilterCriteriaException($"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(DateTime)}'");
        }

        return BuildFilterExpressionForComparable(parameterExpression, memberExpression, filterCriteria, parsedDateTimeValue);
    }

    /// <summary>
    /// Applies a comparison expression dependent on the specified filterOperator. (parameterExpression => p)
    /// FilterOperator.Equals: p.MunicipalityId.Equals(3203)
    /// FilterOperator.Less: p.MunicipalityId.CompareTo(3203). < 0
    /// FilterOperator.LessEqual: p.MunicipalityId.CompareTo(3203) <= 0
    /// FilterOperator.Greater: p.MunicipalityId.CompareTo(3203) > 0
    /// FilterOperator.GreaterEqual:p.MunicipalityId.CompareTo(3203) >= 0
    ///  swapFilterValueCompareDirection = true: ((p.DateOfBirth != null) AndAlso (0 > Convert(p.DateOfBirth, DateOnly).CompareTo(06.02.2005)))
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria</param>
    /// <param name="filterValue">The filter value</param>
    /// <param name="swapFilterValueCompareDirection">swapt left and right expression parameter</param>
    /// <returns></returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForComparable<T>(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria,
        T filterValue,
        bool swapFilterValueCompareDirection = false)
        where T : IComparable<T>
    {
        Expression methodCall;
        if (filterCriteria.FilterOperator == FilterOperatorType.Equals)
        {
            var methodInfo = typeof(T).GetMethod(filterCriteria.FilterOperator.ToString(), new Type[] { typeof(T), });
            AssertMethodNotNull<T>(filterCriteria, methodInfo);
            methodCall = NotNullExpression(memberExpression, e => Expression.Call(e, methodInfo!, Expression.Constant(filterValue)));
        }
        else
        {
            var methodInfo = typeof(T).GetMethod(nameof(IComparable.CompareTo), new Type[] { typeof(T) });
            AssertMethodNotNull<T>(filterCriteria, methodInfo);
            methodCall = NotNullExpression(memberExpression, e => CompareExpressionFromFilterOperator(
                filterCriteria.FilterOperator,
                swapFilterValueCompareDirection,
                Expression.Call(e, methodInfo!, Expression.Constant(filterValue)),
                Expression.Constant(0)));
        }

        return Expression.Lambda<Func<PersonEntity, bool>>(methodCall, parameterExpression);
    }

    private static Expression CompareExpressionFromFilterOperator(FilterOperatorType filterOperator, bool swapLeftRight, Expression left, Expression right)
    {
        var argLeft = swapLeftRight ? right : left;
        var argRight = swapLeftRight ? left : right;
        return filterOperator switch
        {
            FilterOperatorType.Greater => Expression.GreaterThan(argLeft, argRight),
            FilterOperatorType.GreaterEqual => Expression.GreaterThanOrEqual(argLeft, argRight),
            FilterOperatorType.Less => Expression.LessThan(argLeft, argRight),
            FilterOperatorType.LessEqual => Expression.LessThanOrEqual(argLeft, argRight),
            _ => throw new InvalidSearchFilterCriteriaException($"Method '{filterOperator}' is not available for field type '{nameof(Expression)}'"),
        };
    }

    /// <summary>
    /// Builds a lambda expression built for the filter criteria for type <see cref="Enum"/>.
    /// </summary>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <param name="memberExpression">The member expression.</param>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <returns>A lambda expression which can be used as filter on the queryable.</returns>
    private static Expression<Func<PersonEntity, bool>> BuildFilterExpressionForEnum<TEnum>(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
        where TEnum : struct
    {
        if (filterCriteria.FilterOperator != FilterOperatorType.Equals && filterCriteria.FilterOperator != FilterOperatorType.Contains)
        {
            throw new InvalidSearchFilterCriteriaException($"FilterOperator '{filterCriteria.FilterOperator}' is not valid for selected FilterDataType '{nameof(TEnum)}'");
        }

        var values = filterCriteria.FilterType == FilterDataType.Multiselect
            ? filterCriteria.FilterValue.Split(MultipleFilterSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : new[] { filterCriteria.FilterValue };

        var enumExpression = values.Select(v =>
        {
            if (!Enum.TryParse(v, true, out TEnum parsedEnumValue))
            {
                throw new InvalidSearchFilterCriteriaException(
                    $"Value '{filterCriteria.FilterValue}' not parsable into member type '{nameof(TEnum)}'");
            }

            return parsedEnumValue;
        }).Aggregate(default(Expression), (exp, parsedEnumValue) =>
        {
            var equalExpression = Expression.Equal(memberExpression, Expression.Constant(parsedEnumValue));
            return exp == null ? equalExpression : Expression.Or(exp, equalExpression);
        });

        return enumExpression == null
            ? throw new InvalidSearchFilterCriteriaException($"FilterValue '{filterCriteria.FilterValue}' is not valid for selected FilterDataType '{nameof(TEnum)}'")
            : Expression.Lambda<Func<PersonEntity, bool>>(enumExpression, parameterExpression);
    }

    private static async Task<Page<PersonEntity>> ExecuteSearchQuery(IQueryable<PersonEntity> queryable, Pageable? paging)
    {
        if (paging == null)
        {
            var searchResult = await queryable.ToListAsync();
            return new Page<PersonEntity>(searchResult, searchResult.Count, 1, int.MaxValue);
        }

        return await queryable.ToPageAsync(paging);
    }

    private static async Task<PersonSearchResultPage<PersonEntity>> ExecuteSearchQueryWithInvalidCount(IQueryable<PersonEntity> queryable, Pageable? paging)
    {
        var page = await ExecuteSearchQuery(queryable, paging);
        var invalidCount = await queryable.CountAsync(x => !x.IsValid);
        return new PersonSearchResultPage<PersonEntity>(page, invalidCount);
    }

    private static MethodInfo GetEnumerableAnyMethodInfo() =>
        typeof(Enumerable)
            .GetMethods()
            .Single(method => method.Name == "Any" && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(PersonDoiEntity));

    private IQueryable<PersonEntity> FilterForCriterias(
        IQueryable<PersonEntity> queryable,
        IReadOnlyCollection<FilterCriteriaEntity> criterias,
        DateOnly referenceKeydate)
    {
        foreach (var criteria in criterias.Where(x => !string.IsNullOrEmpty(x.FilterValue)))
        {
            AssertFilterCriteriaIsValid(criteria);
            queryable = IsCalculatedFilterField(criteria.ReferenceId) ?
                FilterByCalculatedField(queryable, criteria, referenceKeydate) :
                FilterByField(queryable, criteria);
        }

        return queryable;
    }

    private IQueryable<PersonEntity> FilterByCalculatedField(
       IQueryable<PersonEntity> queryable,
       FilterCriteriaEntity filterCriteria,
       DateOnly referenceKeyDate)
    {
        var parameterExpression = Expression.Parameter(typeof(PersonEntity), "p");

        Expression<Func<PersonEntity, bool>> filterExpression;
        if (filterCriteria.ReferenceId == FilterReference.Age)
        {
            if (filterCriteria.FilterOperator is not (FilterOperatorType.Equals or FilterOperatorType.LessEqual
                or FilterOperatorType.GreaterEqual or FilterOperatorType.Less or FilterOperatorType.Greater))
            {
                throw new InvalidSearchFilterCriteriaException($"FilterOperator '{filterCriteria.FilterOperator}' is not valid for filter '{FilterReference.Age}'");
            }

            EnsureIsFilterDataType(filterCriteria, FilterDataType.Numeric);
            var memberExpression = Expression.Property(parameterExpression, nameof(PersonEntity.DateOfBirth));
            filterExpression = BuildFilterExpressionForAge(parameterExpression, memberExpression, filterCriteria, referenceKeyDate);
        }
        else if (filterCriteria.ReferenceId == FilterReference.HasValidationErrors)
        {
            EnsureIsFilterDataType(filterCriteria, FilterDataType.Boolean);
            AssertFilterOperatorIsValidForFilterDataType(filterCriteria.FilterOperator, filterCriteria.FilterType);
            var memberExpression = Expression.Property(parameterExpression, nameof(PersonEntity.IsValid));
            filterExpression = BuildFilterExpressionForHasValidationErrors(parameterExpression, memberExpression, filterCriteria);
        }
        else if (filterCriteria.ReferenceId == FilterReference.SwissCitizenship)
        {
            EnsureIsFilterDataType(filterCriteria, FilterDataType.Select);
            var memberExpression = Expression.Property(parameterExpression, nameof(PersonEntity.Country));
            AssertFilterOperatorIsValidForFilterDataType(filterCriteria.FilterOperator, filterCriteria.FilterType);
            AssertFilterValueIsValidForFilterDataType(filterCriteria.FilterValue, filterCriteria.FilterType);
            var expression = BuildFilterExpressionForSwissCitizenship(
                parameterExpression,
                memberExpression,
                referenceKeyDate,
                filterCriteria);
            if (expression == null)
            {
                return queryable;
            }

            filterExpression = expression;
        }
        else if (filterCriteria.ReferenceId == FilterReference.OriginName17)
        {
            EnsureIsFilterDataType(filterCriteria, FilterDataType.String);
            var memberExpression = Expression.Property(parameterExpression, nameof(PersonEntity.PersonDois));
            filterExpression = BuildFilterExpressionForPersonDoi(memberExpression, filterCriteria, parameterExpression, nameof(PersonDoiEntity.Name), DomainOfInfluenceType.Og);
        }
        else if (filterCriteria.ReferenceId == FilterReference.OriginOnCanton17)
        {
            EnsureIsFilterDataType(filterCriteria, FilterDataType.String);
            var memberExpression = Expression.Property(parameterExpression, nameof(PersonEntity.PersonDois));
            filterExpression = BuildFilterExpressionForPersonDoi(memberExpression, filterCriteria, parameterExpression, nameof(PersonDoiEntity.Canton), DomainOfInfluenceType.Og);
        }
        else if (IsCircleIdProperty(filterCriteria.ReferenceId))
        {
            EnsureIsFilterDataType(filterCriteria, FilterDataType.String);
            AssertFilterOperatorIsValidForFilterDataType(filterCriteria.FilterOperator, filterCriteria.FilterType);
            var memberExpression = Expression.Property(parameterExpression, nameof(PersonEntity.PersonDois));
            var domainOfInfluenceType = GetDomainOfInfluenceTypeByReferenceId(filterCriteria.ReferenceId);
            filterExpression = BuildFilterExpressionForPersonDoi(memberExpression, filterCriteria, parameterExpression, nameof(PersonDoiEntity.Identifier), domainOfInfluenceType);
        }
        else if (IsCircleNameProperty(filterCriteria.ReferenceId))
        {
            EnsureIsFilterDataType(filterCriteria, FilterDataType.String);
            AssertFilterOperatorIsValidForFilterDataType(filterCriteria.FilterOperator, filterCriteria.FilterType);
            var memberExpression = Expression.Property(parameterExpression, nameof(PersonEntity.PersonDois));
            var domainOfInfluenceType = GetDomainOfInfluenceTypeByReferenceId(filterCriteria.ReferenceId);
            filterExpression = BuildFilterExpressionForPersonDoi(memberExpression, filterCriteria, parameterExpression, nameof(PersonDoiEntity.Name), domainOfInfluenceType);
        }
        else if (filterCriteria.ReferenceId == FilterReference.ContactAddressLine17)
        {
            EnsureIsFilterDataType(filterCriteria, FilterDataType.String);
            var memberExpressions = (new string[]
            {
                nameof(PersonEntity.ContactAddressLine1), nameof(PersonEntity.ContactAddressLine2), nameof(PersonEntity.ContactAddressLine3),
                nameof(PersonEntity.ContactAddressLine4), nameof(PersonEntity.ContactAddressLine5), nameof(PersonEntity.ContactAddressLine6), nameof(PersonEntity.ContactAddressLine7),
            }).Select(name => Expression.Property(parameterExpression, name)).ToArray();

            AssertFilterOperatorIsValidForFilterDataType(filterCriteria.FilterOperator, filterCriteria.FilterType);
            filterExpression = BuildFilterExpressionForMultipleStrings(parameterExpression, memberExpressions, filterCriteria);
        }
        else
        {
            throw new InvalidSearchFilterCriteriaException($"No filter expression builder available for ReferenceId {filterCriteria.ReferenceId}");
        }

        _logger.LogDebug("Built expression: {Expression}", filterExpression.ToString());
        return queryable.Where(filterExpression);
    }

    private Expression<Func<PersonEntity, bool>> BuildFilterExpressionForPersonDoi(
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria,
        ParameterExpression parameterExpression,
        string personDoiPropertyName,
        DomainOfInfluenceType domainOfInfluenceType)
    {
        AssertFilterOperatorIsValidForFilterDataType(filterCriteria.FilterOperator, filterCriteria.FilterType);
        var personDoiParameterExpression = Expression.Parameter(typeof(PersonDoiEntity), "pd");
        var domainOfInfluenceTypeMemberExpression = Expression.Property(personDoiParameterExpression, nameof(PersonDoiEntity.DomainOfInfluenceType));
        var stringOperatorMethodInfo = typeof(string).GetMethod(filterCriteria.FilterOperator.ToString(), new[] { typeof(string), });
        AssertMethodNotNull<string>(filterCriteria, stringOperatorMethodInfo);
        Expression nameMemberExpression = Expression.Property(personDoiParameterExpression, personDoiPropertyName);
        var filterValue = filterCriteria.FilterValue;
        ApplyStringCaseInsensitive(ref nameMemberExpression, ref filterValue);
        var domainOfInfluenceTypeEqualsExpression = Expression.Equal(domainOfInfluenceTypeMemberExpression, Expression.Constant(domainOfInfluenceType));
        var operatorMethodCall = Expression.Call(nameMemberExpression, stringOperatorMethodInfo!, Expression.Constant(filterValue));
        var personDoiFilterExpression = Expression.And(operatorMethodCall, domainOfInfluenceTypeEqualsExpression);
        var personDoiLambda = Expression.Lambda<Func<PersonDoiEntity, bool>>(personDoiFilterExpression, personDoiParameterExpression);
        var anyMethodInfo = GetEnumerableAnyMethodInfo();
        var anyMethodCall = Expression.Call(null, anyMethodInfo, memberExpression, personDoiLambda);
        return Expression.Lambda<Func<PersonEntity, bool>>(anyMethodCall, parameterExpression);
    }

    private Expression<Func<PersonEntity, bool>> BuildFilterExpressionForMultiselectString(
        ParameterExpression parameterExpression,
        MemberExpression memberExpression,
        FilterCriteriaEntity filterCriteria)
    {
        if (filterCriteria.FilterOperator is not (FilterOperatorType.Equals or FilterOperatorType.Contains))
        {
            throw new InvalidSearchFilterCriteriaException($"FilterOperator '{filterCriteria.FilterOperator}' is not valid for multiselect value");
        }

        var conditionExpression = filterCriteria.FilterValue
            .Split(MultipleFilterSeparator)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .Aggregate(default(Expression), (exp, value) =>
            {
                var equalExpression = Expression.Equal(memberExpression, Expression.Constant(value));
                return exp == null
                    ? equalExpression
                    : Expression.Or(exp, equalExpression);
            });

        if (conditionExpression == null)
        {
            throw new InvalidSearchFilterCriteriaException($"FilterValue '{filterCriteria.FilterValue}' requires at least one value");
        }

        return Expression.Lambda<Func<PersonEntity, bool>>(conditionExpression, parameterExpression);
    }

    private DomainOfInfluenceType GetDomainOfInfluenceTypeByReferenceId(FilterReference filterReference)
    {
        return filterReference switch
        {
            FilterReference.CatholicCircleId or FilterReference.CatholicCircleName => DomainOfInfluenceType.KiKat,
            FilterReference.EvangelicCircleId or FilterReference.EvangelicCircleName => DomainOfInfluenceType.KiEva,
            FilterReference.PoliticalCircleId or FilterReference.PoliticalCircleName => DomainOfInfluenceType.Sk,
            FilterReference.SchoolCircleId or FilterReference.SchoolCircleName => DomainOfInfluenceType.Sc,
            FilterReference.PeopleCircleId or FilterReference.PeopleCircleName => DomainOfInfluenceType.AnVok,
            FilterReference.ResidentialDistrictCircleId or FilterReference.ResidentialDistrictCircleName => DomainOfInfluenceType.AnWok,
            FilterReference.TrafficCircleId or FilterReference.TrafficCircleName => DomainOfInfluenceType.AnVek,
            _ => DomainOfInfluenceType.Unspecified,
        };
    }

    private IQueryable<PersonEntity> FilterByField(
        IQueryable<PersonEntity> queryable,
        FilterCriteriaEntity filterCriteria)
    {
        var parameterExpression = Expression.Parameter(typeof(PersonEntity), "p");
        var memberExpression = Expression.Property(parameterExpression, filterCriteria.ReferenceId.ToString());
        var memberType = GetMemberType<PersonEntity>(filterCriteria.ReferenceId.ToString());

        if (filterCriteria is { FilterType: FilterDataType.String, ReferenceId: FilterReference.Vn })
        {
            filterCriteria.FilterValue = filterCriteria.FilterValue.Replace(".", string.Empty);
            filterCriteria.FilterType = FilterDataType.Numeric;
        }

        AssertFilterDataTypeIsValidForMemberType(filterCriteria.FilterType, memberType);
        AssertFilterOperatorIsValidForFilterDataType(filterCriteria.FilterOperator, filterCriteria.FilterType);
        AssertFilterValueIsValidForFilterDataType(filterCriteria.FilterValue, filterCriteria.FilterType);

        var filterExpression = memberType.Name switch
        {
            nameof(String) when filterCriteria.ReferenceId == FilterReference.Country
                => BuildFilterExpressionForMultiselectString(parameterExpression, memberExpression, filterCriteria),
            nameof(String) => BuildFilterExpressionForString(parameterExpression, memberExpression, filterCriteria),
            nameof(Int32) => BuildFilterExpressionForInt(parameterExpression, memberExpression, filterCriteria),
            nameof(Int64) => BuildFilterExpressionForLong(parameterExpression, memberExpression, filterCriteria),
            nameof(DateOnly) => BuildFilterExpressionForDateOnly(parameterExpression, memberExpression, filterCriteria),
            nameof(DateTime) => BuildFilterExpressionForDateTime(parameterExpression, memberExpression, filterCriteria),
            nameof(Boolean) => BuildFilterExpressionForBoolean(parameterExpression, memberExpression, filterCriteria),
            nameof(Guid) => BuildFilterExpressionForGuid(parameterExpression, memberExpression, filterCriteria),
            nameof(ReligionType) => BuildFilterExpressionForEnum<ReligionType>(parameterExpression, memberExpression, filterCriteria),
            nameof(ResidenceType) => BuildFilterExpressionForEnum<ResidenceType>(parameterExpression, memberExpression, filterCriteria),
            nameof(SexType) => BuildFilterExpressionForEnum<SexType>(parameterExpression, memberExpression, filterCriteria),
            _ => throw new InvalidSearchFilterCriteriaException($"No filter expression builder available for type {nameof(memberType)}"),
        };

        _logger.LogDebug("Built expression: {Expression}", filterExpression.ToString());

        return queryable.Where(filterExpression);
    }

    private IQueryable<PersonEntity> QueryIsLatest()
    {
        return Query().Where(p => p.IsLatest && !p.IsDeleted);
    }

    private IQueryable<PersonEntity> QueryFilterVersionPersons(Guid filterVersionId)
    {
        // changing the order here may break signatures of filter versions and bfs integrities
        return Context.FilterVersionPersons
            .Where(version => version.FilterVersionId == filterVersionId)
            .Include(p => p.Person!.PersonDois)
            .Select(version => version.Person!)
            .OrderBy(x => x.MunicipalityId)
            .ThenBy(x => x.Id);
    }

    private IQueryable<PersonEntity> QueryPersons(
        IReadOnlyCollection<FilterCriteriaEntity> criteria,
        DateOnly referenceKeyDate)
    {
        var queryable = QueryIsLatest();
        queryable = FilterForCriterias(queryable, criteria, referenceKeyDate);

        return queryable
            .Include(p => p.PersonDois)
            .OrderBy(i => i.MunicipalityId)
            .ThenBy(p => p.Id);
    }

    private void EnsureIsFilterDataType(FilterCriteriaEntity filterCriteria, FilterDataType filterDataType)
    {
        if (filterCriteria.FilterType == filterDataType)
        {
            return;
        }

        throw new InvalidSearchFilterCriteriaException($"FilterDataType '{filterCriteria.FilterType}' is not valid for filter '{filterCriteria.ReferenceId}'");
    }
}
