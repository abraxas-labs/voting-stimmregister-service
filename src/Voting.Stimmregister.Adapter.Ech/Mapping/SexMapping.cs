// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Ech0010_6_0;

namespace Voting.Stimmregister.Adapter.Ech.Mapping;

internal static class SexMapping
{
    public static Ech0044_4_1.SexType ToEchSexType(this Domain.Enums.SexType sex)
    {
        return sex switch
        {
            Domain.Enums.SexType.Male => Ech0044_4_1.SexType.Item1,
            Domain.Enums.SexType.Female => Ech0044_4_1.SexType.Item2,
            _ => Ech0044_4_1.SexType.Item3,
        };
    }

    public static MrMrsType? ToEchMrMrs(this Domain.Enums.SexType sex)
    {
        return sex switch
        {
            Domain.Enums.SexType.Male => MrMrsType.Item2,
            Domain.Enums.SexType.Female => MrMrsType.Item1,
            _ => null,
        };
    }
}
