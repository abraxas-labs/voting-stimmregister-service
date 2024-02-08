// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// The search parameter of the last executed search of a user for a given tenant.
/// </summary>
public class LastSearchParameterEntity : BaseEntity
{
    public static IEqualityComparer<LastSearchParameterEntity> ExcludeIdComparer { get; } = new ExcludeIdLastSearchParameterEntityComparer();

    public string TenantId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public PersonSearchType SearchType { get; set; }

    public Pageable PageInfo { get; set; } = new(1, 20);

    public ICollection<FilterCriteriaEntity> FilterCriteria { get; set; } = new HashSet<FilterCriteriaEntity>();

    private sealed class ExcludeIdLastSearchParameterEntityComparer : IEqualityComparer<LastSearchParameterEntity>
    {
        public bool Equals(LastSearchParameterEntity? x, LastSearchParameterEntity? y)
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

            return x.TenantId == y.TenantId
                && x.UserId == y.UserId
                && x.SearchType == y.SearchType
                && x.PageInfo.Page.Equals(y.PageInfo.Page)
                && x.PageInfo.PageSize.Equals(y.PageInfo.PageSize)
                && x.FilterCriteria.SequenceEqual(y.FilterCriteria, FilterCriteriaEntity.ExcludeIdComparer);
        }

        public int GetHashCode(LastSearchParameterEntity obj)
            => HashCode.Combine(obj.TenantId, obj.UserId, (int)obj.SearchType, obj.PageInfo.Page, obj.PageInfo.PageSize, obj.FilterCriteria.Count);
    }
}
