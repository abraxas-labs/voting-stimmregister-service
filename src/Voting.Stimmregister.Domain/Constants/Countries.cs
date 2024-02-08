// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Constants;

public static class Countries
{
    public const string Switzerland = "CH";

    public static bool IsSwitzerland(string? iso2)
        => Switzerland.Equals(iso2, StringComparison.OrdinalIgnoreCase);
}
