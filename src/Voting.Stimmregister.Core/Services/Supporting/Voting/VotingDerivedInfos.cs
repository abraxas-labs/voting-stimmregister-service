// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Voting.Stimmregister.Domain.Constants;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services.Supporting.Voting;

public class VotingDerivedInfos : IVotingDerivedInfos
{
    public void ComputeInfos(IReadOnlyCollection<PersonEntityModel> persons, DateOnly referenceKeyDate)
    {
        foreach (var person in persons)
        {
            ComputeInfos(person, referenceKeyDate);
        }
    }

    public void ComputeInfos(PersonEntityModel person, DateOnly referenceKeyDate)
    {
        person.IsNationalityValidForVotingRights = IsNationalityValidForVotingRights(person.Country);
        person.IsBirthDateValidForVotingRights = IsBirthDateValidForVotingRights(referenceKeyDate, person.DateOfBirth);
        person.IsVotingAllowed = IsVotingAllowed(person, referenceKeyDate);
        person.Origins = GetOrigins(person);
    }

    private static bool IsNationalityValidForVotingRights(string? country)
        => country?.Equals(Countries.Switzerland, StringComparison.OrdinalIgnoreCase) == true;

    private static bool IsBirthDateValidForVotingRights(DateOnly referenceKeyDate, DateOnly dateOfBirth)
    {
        const int ageOfMajority = 18;
        return referenceKeyDate.AddYears(-ageOfMajority).CompareTo(dateOfBirth) >= 0 && dateOfBirth > DateOnly.MinValue;
    }

    private static bool IsReferenceKeyDateAfterOrEqualMoveInArrivalDate(DateOnly referenceKeyDate, DateOnly? moveInArrivalDate)
    {
        // An empty moveInArrivalDate will result in true, as this is valid from a business point of view.
        return referenceKeyDate.CompareTo(moveInArrivalDate) >= 0;
    }

    private static bool IsVotingAllowed(PersonEntityModel person, DateOnly referenceKeyDate)
    {
        // The correct Version of the person record is (needed to be) filtered directly in the query.
        return IsBirthDateValidForVotingRights(referenceKeyDate, person.DateOfBirth) &&
               IsReferenceKeyDateAfterOrEqualMoveInArrivalDate(referenceKeyDate, person.MoveInArrivalDate) &&
               IsReferenceKeyDateBeforeOrEqualDeletedDateOrNotDeleted(referenceKeyDate, person.DeletedDate) &&
               person is
               {
                   RestrictedVotingAndElectionRightFederation: false,
                   IsNationalityValidForVotingRights: true,
               };
    }

    private static bool IsReferenceKeyDateBeforeOrEqualDeletedDateOrNotDeleted(DateOnly referenceKeyDate, DateTime? deletedAtDate)
    {
        return deletedAtDate == null || referenceKeyDate.CompareTo(DateOnly.FromDateTime(deletedAtDate.Value)) <= 0;
    }

    private static List<string> GetOrigins(PersonEntityModel person)
    {
        return person.PersonDois
            .Where(x => x.DomainOfInfluenceType == DomainOfInfluenceType.Og)
            .Select(x => string.IsNullOrEmpty(x.Canton) ? x.Name : $"{x.Name} ({x.Canton})")
            .ToList();
    }
}
