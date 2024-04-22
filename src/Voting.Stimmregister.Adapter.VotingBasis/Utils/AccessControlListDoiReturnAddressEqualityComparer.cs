// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.VotingBasis.Utils;

internal sealed class AccessControlListDoiReturnAddressEqualityComparer : IEqualityComparer<AccessControlListDoiReturnAddress>
{
    public static readonly AccessControlListDoiReturnAddressEqualityComparer Instance = new();

    private AccessControlListDoiReturnAddressEqualityComparer()
    {
    }

    public bool Equals(AccessControlListDoiReturnAddress? x, AccessControlListDoiReturnAddress? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return string.Equals(x.AddressLine1, y.AddressLine1, StringComparison.Ordinal) &&
               string.Equals(x.AddressLine2, y.AddressLine2, StringComparison.Ordinal) &&
               string.Equals(x.Street, y.Street, StringComparison.Ordinal) &&
               string.Equals(x.AddressAddition, y.AddressAddition, StringComparison.Ordinal) &&
               string.Equals(x.ZipCode, y.ZipCode, StringComparison.Ordinal) &&
               string.Equals(x.City, y.City, StringComparison.Ordinal) &&
               string.Equals(x.Country, y.Country, StringComparison.Ordinal);
    }

    public int GetHashCode(AccessControlListDoiReturnAddress obj)
    {
        var hashCode = default(HashCode);
        hashCode.Add(obj.AddressLine1);
        hashCode.Add(obj.AddressLine2);
        hashCode.Add(obj.Street);
        hashCode.Add(obj.AddressAddition);
        hashCode.Add(obj.ZipCode);
        hashCode.Add(obj.City);
        hashCode.Add(obj.Country);
        return hashCode.ToHashCode();
    }
}
