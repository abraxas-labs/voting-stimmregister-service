// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.Import.Loganto.Utils;

public static class LogantoUtil
{
    public const string ExpectedDateFormat = "dd.MM.yyyy";
    public const string DefaultShortSexTypeMale = "M";
    public const string DefaultShortSexTypeFemale = "W";

    private const string UnknownDayOrMonthPlaceholder = "**";
    private const string DateSeparator = ".";
    private const string VotingRightAdditionPermanentPatient = "P";
    private const string VotingRightAdditionDoNotDeliver = "C";

    /// <summary>
    /// Evaluates whether or not a person is restricted to vote. Depending on the
    /// type of residence, the <paramref name="restrictedVotingAndElectionRightFederation"/> defines
    /// if one is restriction or eligible to vote:
    /// <list type="bullet">
    ///   <item>When <paramref name="restrictedVotingAndElectionRightFederation"/> set to 'false' and person is HWS → Eligible to vote on HWS</item>
    ///   <item>When <paramref name="restrictedVotingAndElectionRightFederation"/> set to 'true' and person is HWS → Restricted to vote</item>
    ///   <item>When <paramref name="restrictedVotingAndElectionRightFederation"/> set to 'true' and person is NWS → Eligible to vote for referenced DomainOfInfluenceId</item>
    ///   <item>When <paramref name="restrictedVotingAndElectionRightFederation"/> set to 'false' and person is NWS → Restricted to vote</item>
    /// </list>
    /// </summary>
    /// <param name="typeOfResidence">The type of residence.</param>
    /// <param name="restrictedVotingAndElectionRightFederation">Indicating whether or not a person is restricted to vote depending on main or secondary residence.</param>
    /// <returns>The result for the person's voting restriction.</returns>
    public static bool IsRestrictedToVote(ResidenceType typeOfResidence, bool restrictedVotingAndElectionRightFederation)
    {
        if (typeOfResidence == ResidenceType.HWS)
        {
            return restrictedVotingAndElectionRightFederation;
        }

        if (typeOfResidence == ResidenceType.NWS)
        {
            return !restrictedVotingAndElectionRightFederation;
        }

        return true;
    }

    public static bool ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(
        LogantoPersonCsvRecord record,
        IReadOnlySet<int> bfsThatAllowSendingVotingCardForPeopleWithAwayAddresses,
        IReadOnlySet<int> bfsThatAllowSendingVotingCardForPeopleWithUnknownMainResidenceAddresses)
    {
        // case 1: Code C
        if (VotingRightAdditionDoNotDeliver.Equals(record.VotingRightAddition, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // case 2: Code P
        if (VotingRightAdditionPermanentPatient.Equals(record.VotingRightAddition, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // case 3: unknown address
        if (record.DomainOfInfluenceId.HasValue
           && !record.HasResidenceAddress()
           && (
             !bfsThatAllowSendingVotingCardForPeopleWithUnknownMainResidenceAddresses.Contains(record.MunicipalityId)
             || (!record.HasContactAddress() && bfsThatAllowSendingVotingCardForPeopleWithUnknownMainResidenceAddresses.Contains(record.MunicipalityId))
           ))
        {
            return true;
        }

        // case 4: away address ("auswärts")
        if (!record.DomainOfInfluenceId.HasValue
        && !bfsThatAllowSendingVotingCardForPeopleWithAwayAddresses.Contains(record.MunicipalityId)
        && record.HasResidenceAddress())
        {
            return true;
        }

        return false;
    }

    public static ResidenceType EvaluateResidenceType(bool hasMainResidence, bool hasSecondaryResidence)
    {
        if (hasMainResidence && hasSecondaryResidence)
        {
            return ResidenceType.Undefined;
        }

        if (!hasMainResidence && !hasSecondaryResidence)
        {
            return ResidenceType.Undefined;
        }

        if (hasMainResidence)
        {
            return ResidenceType.HWS;
        }

        return ResidenceType.NWS;
    }

    public static ReligionType ConvertLogantoReligion(string? religion)
    {
        return religion switch
        {
            "111" => ReligionType.Evangelic,
            "121" => ReligionType.Catholic,
            "122" => ReligionType.ChristCatholic,
            _ => ReligionType.Unknown,
        };
    }

    public static SexType ConvertLogantoSex(string? sex)
    {
        return sex switch
        {
            DefaultShortSexTypeMale => SexType.Male,
            DefaultShortSexTypeFemale => SexType.Female,
            _ => SexType.Undefined,
        };
    }

    /// <summary>
    /// Converts the date of birth string into a <see cref="DateOnly"/> type.
    /// The expected input data must be converted as follows:
    /// <list type="bullet">
    ///     <item>**.**.yyyy → 01.01.yyyy / **.**.2007 → 01.01.2007</item>
    ///     <item>**.mm.yyyy → 01.mm.yyyy / **.11.2006 → 01.11.2006</item>
    /// </list>
    /// </summary>
    /// <param name="dateOfBirth">The date of birth string.</param>
    /// <param name="useFirstDayOrMonth">if set to true, the first day or month is used for undefined placehodlers,
    /// otherwise the last day or month is applied.</param>
    /// <param name="dateAdjusted">Wether the date of birth required adjustance.</param>
    /// <returns>The parsed <see cref="DateOnly"/> or <see cref="DateOnly.MinValue"/> if conversion yielded null.</returns>
    public static DateOnly ConvertLogantoDateOfBirth(string dateOfBirth, bool useFirstDayOrMonth, out bool dateAdjusted)
    {
        var convertedDate = ConvertLogantoDateOfBirth(dateOfBirth, useFirstDayOrMonth);
        dateAdjusted = dateOfBirth.Contains(UnknownDayOrMonthPlaceholder) || convertedDate.Equals(DateOnly.MinValue);
        return convertedDate;
    }

    /// <summary>
    /// Converts the date of birth string into a <see cref="DateOnly"/> type.
    /// The expected input data must be converted as follows:
    /// <list type="bullet">
    ///     <item>**.**.yyyy → 01.01.yyyy / **.**.2007 → 01.01.2007</item>
    ///     <item>**.mm.yyyy → 01.mm.yyyy / **.11.2006 → 01.11.2006</item>
    /// </list>
    /// </summary>
    /// <param name="dateOfBirth">The date of birth string.</param>
    /// <param name="useFirstDayOrMonth">if set to true, the first day or month is used for undefined placehodlers,
    /// otherwise the last day or month is applied.</param>
    /// <returns>The parsed <see cref="DateOnly"/> or <see cref="DateOnly.MinValue"/> if conversion yielded null.</returns>
    public static DateOnly ConvertLogantoDateOfBirth(string dateOfBirth, bool useFirstDayOrMonth)
    {
        var convertedDate = ConvertLogantoDate(dateOfBirth, useFirstDayOrMonth);

        if (convertedDate == null)
        {
            return DateOnly.MinValue;
        }

        return (DateOnly)convertedDate;
    }

    /// <summary>
    /// Converts a date string into a <see cref="DateOnly"/> type.
    /// The expected input data must be converted as follows:
    /// <list type="bullet">
    ///   <item><paramref name="useFirstDayOrMonth"/> is true:  **.**.yyyy → 01.01.yyyy / **.**.2007 → 01.01.2007</item>
    ///   <item><paramref name="useFirstDayOrMonth"/> is true:  **.mm.yyyy → 01.mm.yyyy / **.11.2006 → 01.11.2006</item>
    ///   <item><paramref name="useFirstDayOrMonth"/> is false: **.**.yyyy → 31.12.yyyy / **.**.2007 → 31.12.2007</item>
    ///   <item><paramref name="useFirstDayOrMonth"/> is false: **.mm.yyyy → 31.mm.yyyy / **.02.2024 → 29.02.2024</item>
    /// </list>
    /// </summary>
    /// <param name="dateOnlyString">The date only string to convert to <see cref="DateOnly?"/>.</param>
    /// <param name="useFirstDayOrMonth">if set to true, the first day or month is used for undefined placehodlers,
    /// otherwise the last day or month is applied.</param>
    /// <returns>The parsed date only or null if input is empty or not parsable.</returns>
    public static DateOnly? ConvertLogantoDate(string? dateOnlyString, bool useFirstDayOrMonth = true)
    {
        if (string.IsNullOrEmpty(dateOnlyString))
        {
            return null;
        }

        if (dateOnlyString.Length > 10)
        {
            return null;
        }

        var dateArray = dateOnlyString.Split(DateSeparator);
        if (dateArray.Length < 3)
        {
            return null;
        }

        var sYear = dateArray[2];
        if (!int.TryParse(sYear, out var year))
        {
            return null;
        }

        var sMonth = dateArray[1];
        int month;
        if (sMonth == UnknownDayOrMonthPlaceholder)
        {
            month = useFirstDayOrMonth ? 1 : 12;
        }
        else if (!int.TryParse(sMonth, out month))
        {
            return null;
        }

        var sDay = dateArray[0];
        int day;
        if (sDay == UnknownDayOrMonthPlaceholder)
        {
            day = useFirstDayOrMonth ? 1 : DateTime.DaysInMonth(year, month);
        }
        else if (!int.TryParse(sDay, out day))
        {
            return null;
        }

        var dateString = day.ToString().PadLeft(2, '0') + DateSeparator + month.ToString().PadLeft(2, '0') + DateSeparator + year;

        if (!DateOnly.TryParseExact(dateString, ExpectedDateFormat, out var convertedDate))
        {
            return null;
        }

        return convertedDate;
    }

    /// <summary>
    /// Converts the language in Loganto format to ISO 639-1 language codes.
    /// </summary>
    /// <param name="language">the Loganto language code.</param>
    /// <returns>the ISO 691-1 language code.</returns>
    public static string ConvertLogantoLanguage(string? language)
    {
        return language?.ToUpper() switch
        {
            "F" => "fr",
            "I" => "it",
            "E" => "en",
            "RM" => "rm",
            _ => "de",
        };
    }

    /// <summary>
    /// Gets up to the first two characters of the passed residence permit,
    /// which is the relevant part for stimmregister.
    /// </summary>
    /// <param name="residencePermit">The residence permit code, i.e. '30'.</param>
    /// <returns>Up to the first two characters of the passed residence permit.</returns>
    public static string? GetRelevantResidencePermitPart(string? residencePermit)
    {
        if (string.IsNullOrWhiteSpace(residencePermit) || residencePermit.Length < 2)
        {
            return residencePermit;
        }

        return residencePermit[..2];
    }
}
