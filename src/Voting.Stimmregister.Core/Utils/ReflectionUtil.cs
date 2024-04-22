// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Reflection;

namespace Voting.Stimmregister.Core.Utils;

public static class ReflectionUtil
{
    public static bool IsNullable(PropertyInfo property)
    {
        var nullabilityInfoContext = new NullabilityInfoContext();
        var info = nullabilityInfoContext.Create(property);
        return info.WriteState == NullabilityState.Nullable || info.ReadState == NullabilityState.Nullable;
    }
}
