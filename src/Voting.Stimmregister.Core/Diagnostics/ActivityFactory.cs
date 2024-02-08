// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Voting.Stimmregister.Core.Diagnostics;

public class ActivityFactory<T>
{
    public Activity? Start(string shortName, [CallerMemberName] string operationName = "unknown")
        => StimmregisterCoreActivitySource.Start(shortName, typeof(T).Name, operationName);
}
