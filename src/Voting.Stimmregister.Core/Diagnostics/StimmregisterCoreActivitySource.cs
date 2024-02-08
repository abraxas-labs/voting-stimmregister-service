// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Voting.Stimmregister.Core.Diagnostics;

internal static class StimmregisterCoreActivitySource
{
    private static readonly ActivitySource _instance = new("Voting.Stimmregister.Core");

    public static Activity? Start(string shortName, string serviceName, [CallerMemberName] string operationName = "unknown")
    {
        var activity = _instance.StartActivity(shortName);
        if (activity == null)
        {
            return null;
        }

        activity.DisplayName = $"{operationName} ({serviceName})";
        return activity;
    }
}
