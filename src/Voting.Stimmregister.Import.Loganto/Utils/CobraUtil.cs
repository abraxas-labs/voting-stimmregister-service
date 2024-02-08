// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Import.Loganto.Utils;

public static class CobraUtil
{
    public const string DefaultCountryValue = "CH";
    public const string DefaultCountryNameShortValue = "Schweiz";

    public const string ExpectedDateFormat = "dd.MM.yyyy";

    public const string DefaultLongSexTypeMale = "männlich";
    public const string DefaultLongSexTypeFemale = "weiblich";

    /// <summary>
    /// Converts a sex type string to <see cref="SexType"/>.
    /// <list type="bullet">
    ///     <item>männlich: male</item>
    ///     <item>weiblich: female</item>
    ///     <item>all others: undefined</item>
    /// </list>
    /// </summary>
    /// <param name="sex">The sex type string.</param>
    /// <returns>A mapped <see cref="SexType"/>.</returns>
    public static SexType ConvertSexType(string? sex)
    {
        return sex switch
        {
            DefaultLongSexTypeMale => SexType.Male,
            DefaultLongSexTypeFemale => SexType.Female,
            _ => SexType.Undefined,
        };
    }

    /// <summary>
    /// Converts the date of birth string into a <see cref="DateOnly"/> type.
    /// The expected input data must match the format of <see cref="ExpectedDateFormat"/>.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth string.</param>
    /// <param name="dateAdjusted">Wether the date of birth required adjustance.</param>
    /// <returns>The parsed <see cref="DateOnly"/> or <see cref="DateOnly.MinValue"/> if conversion failed.</returns>
    public static DateOnly ConvertDateOfBirth(string dateOfBirth, out bool dateAdjusted)
    {
        var convertedDate = ConvertDateOfBirth(dateOfBirth);
        dateAdjusted = convertedDate.Equals(DateOnly.MinValue);
        return convertedDate;
    }

    /// <summary>
    /// Converts the date of birth string into a <see cref="DateOnly"/> type.
    /// The expected input data must match the format of <see cref="ExpectedDateFormat"/>.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth string.</param>
    /// <returns>The parsed <see cref="DateOnly"/> or <see cref="DateOnly.MinValue"/> if conversion failed.</returns>
    public static DateOnly ConvertDateOfBirth(string dateOfBirth)
    {
        return DateOnly.TryParseExact(dateOfBirth, ExpectedDateFormat, out var convertedDate) ? convertedDate : DateOnly.MinValue;
    }
}
