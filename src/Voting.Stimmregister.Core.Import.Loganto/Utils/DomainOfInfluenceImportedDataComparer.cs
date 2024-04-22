// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Import.Loganto.Utils;

public sealed class DomainOfInfluenceImportedDataComparer : IEqualityComparer<DomainOfInfluenceEntity>
{
    public static readonly DomainOfInfluenceImportedDataComparer Instance = new();

    private DomainOfInfluenceImportedDataComparer()
    {
    }

    public bool Equals(DomainOfInfluenceEntity? x, DomainOfInfluenceEntity? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.MunicipalityId == y.MunicipalityId
            && x.DomainOfInfluenceId == y.DomainOfInfluenceId
            && x.Street == y.Street
            && x.HouseNumber == y.HouseNumber
            && x.HouseNumberAddition == y.HouseNumberAddition
            && x.SwissZipCode == y.SwissZipCode
            && x.Town == y.Town
            && x.IsPartOfPoliticalMunicipality == y.IsPartOfPoliticalMunicipality
            && x.PoliticalCircleId == y.PoliticalCircleId
            && x.PoliticalCircleName == y.PoliticalCircleName
            && x.CatholicChurchCircleId == y.CatholicChurchCircleId
            && x.CatholicChurchCircleName == y.CatholicChurchCircleName
            && x.EvangelicChurchCircleId == y.EvangelicChurchCircleId
            && x.EvangelicChurchCircleName == y.EvangelicChurchCircleName
            && x.SchoolCircleId == y.SchoolCircleId
            && x.SchoolCircleName == y.SchoolCircleName
            && x.TrafficCircleId == y.TrafficCircleId
            && x.TrafficCircleName == y.TrafficCircleName
            && x.ResidentialDistrictCircleId == y.ResidentialDistrictCircleId
            && x.ResidentialDistrictCircleName == y.ResidentialDistrictCircleName
            && x.PeopleCouncilCircleId == y.PeopleCouncilCircleId
            && x.PeopleCouncilCircleName == y.PeopleCouncilCircleName
            && x.IsValid == y.IsValid
            && x.ValidationErrors == y.ValidationErrors;
    }

    public int GetHashCode(DomainOfInfluenceEntity obj)
    {
        var hashCode = default(HashCode);
        hashCode.Add(obj.MunicipalityId);
        hashCode.Add(obj.DomainOfInfluenceId);
        hashCode.Add(obj.Street);
        hashCode.Add(obj.HouseNumber);
        hashCode.Add(obj.HouseNumberAddition);
        hashCode.Add(obj.SwissZipCode);
        hashCode.Add(obj.Town);
        hashCode.Add(obj.IsPartOfPoliticalMunicipality);
        hashCode.Add(obj.PoliticalCircleId);
        hashCode.Add(obj.PoliticalCircleName);
        hashCode.Add(obj.CatholicChurchCircleId);
        hashCode.Add(obj.CatholicChurchCircleName);
        hashCode.Add(obj.EvangelicChurchCircleId);
        hashCode.Add(obj.EvangelicChurchCircleName);
        hashCode.Add(obj.SchoolCircleId);
        hashCode.Add(obj.SchoolCircleName);
        hashCode.Add(obj.TrafficCircleId);
        hashCode.Add(obj.TrafficCircleName);
        hashCode.Add(obj.ResidentialDistrictCircleId);
        hashCode.Add(obj.ResidentialDistrictCircleName);
        hashCode.Add(obj.PeopleCouncilCircleId);
        hashCode.Add(obj.PeopleCouncilCircleName);
        hashCode.Add(obj.IsValid);
        hashCode.Add(obj.ValidationErrors);
        return hashCode.ToHashCode();
    }
}
