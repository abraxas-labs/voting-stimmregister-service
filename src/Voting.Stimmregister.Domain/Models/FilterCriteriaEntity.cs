// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

public class FilterCriteriaEntity : AuditedEntity
{
    public static IEqualityComparer<FilterCriteriaEntity> ExcludeIdComparer { get; } = new ExcludeIdFilterCriteriaEntityComparer();

    /// <summary>
    /// Gets or sets the 'reference id' / 'field name' on which the filter criteria should be applied to, i.e. 'FirstName'.
    /// The reference id must be an exact match to the field name since it is used with reflection.
    /// </summary>
    public FilterReference ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the filter value, i.e. the user input term.
    /// </summary>
    public string FilterValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter data type, i.e. 'string'.
    /// </summary>
    public FilterDataType FilterType { get; set; }

    /// <summary>
    /// Gets or sets the filter operator, i.e. 'contains'.
    /// </summary>
    public FilterOperatorType FilterOperator { get; set; }

    /// <summary>
    /// Gets or sets the index of this criteria inside a filter, filter version or a last search parameter set.
    /// </summary>
    public int SortIndex { get; set; }

    // References
    public Guid? FilterId { get; set; }

    public Guid? FilterVersionId { get; set; }

    public Guid? LastSearchParameterId { get; set; }

    public FilterEntity? Filter { get; set; }

    public FilterVersionEntity? FilterVersion { get; set; }

    public LastSearchParameterEntity? LastSearchParameter { get; set; }

    private sealed class ExcludeIdFilterCriteriaEntityComparer : IEqualityComparer<FilterCriteriaEntity>
    {
        public bool Equals(FilterCriteriaEntity? x, FilterCriteriaEntity? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null)
            {
                return false;
            }

            if (y is null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.ReferenceId == y.ReferenceId
                && x.FilterValue == y.FilterValue
                && x.FilterType == y.FilterType
                && x.FilterOperator == y.FilterOperator
                && x.SortIndex == y.SortIndex;
        }

        public int GetHashCode(FilterCriteriaEntity obj)
            => HashCode.Combine((int)obj.ReferenceId, obj.FilterValue, (int)obj.FilterType, (int)obj.FilterOperator, obj.SortIndex);
    }
}
