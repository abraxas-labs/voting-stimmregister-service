// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Ech0044_4_1;

namespace Voting.Stimmregister.Adapter.Ech.Mapping;

internal static class DatePartiallyKnownMapping
{
    public static DatePartiallyKnownType ToEchDatePartiallyKnown(this DateOnly date)
        => new DatePartiallyKnownType { YearMonthDay = date.ToDateTime(TimeOnly.MinValue) };
}
