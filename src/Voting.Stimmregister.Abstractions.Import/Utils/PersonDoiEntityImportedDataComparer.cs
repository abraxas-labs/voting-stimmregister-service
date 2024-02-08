// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Import.Utils;

/// <summary>
/// A comparer for the <see cref="PersonDoiEntity"/> which compares all business attributes relevant for the import.
/// Ignores data not changed by the imported business data (eg. the id, the person id, ...).
/// </summary>
internal sealed class PersonDoiEntityImportedDataComparer : IEqualityComparer<PersonDoiEntity>
{
    public static readonly PersonDoiEntityImportedDataComparer Instance = new();

    private PersonDoiEntityImportedDataComparer()
    {
    }

    public bool Equals(PersonDoiEntity? x, PersonDoiEntity? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Name.Equals(y.Name, StringComparison.Ordinal)
            && x.Identifier.Equals(y.Identifier, StringComparison.Ordinal)
            && x.Canton.Equals(y.Canton, StringComparison.Ordinal)
            && x.DomainOfInfluenceType == y.DomainOfInfluenceType;
    }

    public int GetHashCode(PersonDoiEntity obj)
        => HashCode.Combine(obj.Name, obj.Identifier, obj.Canton, obj.DomainOfInfluenceType);
}
