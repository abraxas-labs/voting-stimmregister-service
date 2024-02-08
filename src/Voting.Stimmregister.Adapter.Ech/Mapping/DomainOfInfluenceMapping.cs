// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Ech0155_4_0;

namespace Voting.Stimmregister.Adapter.Ech.Mapping;

internal static class DomainOfInfluenceMapping
{
    public static DomainOfInfluenceTypeType ToEchDomainOfInfluence(this Domain.Enums.DomainOfInfluenceType doi)
    {
        return doi switch
        {
            Domain.Enums.DomainOfInfluenceType.Ch => DomainOfInfluenceTypeType.Ch,
            Domain.Enums.DomainOfInfluenceType.Ct => DomainOfInfluenceTypeType.Ct,
            Domain.Enums.DomainOfInfluenceType.Bz => DomainOfInfluenceTypeType.Bz,
            Domain.Enums.DomainOfInfluenceType.Mu => DomainOfInfluenceTypeType.Mu,
            Domain.Enums.DomainOfInfluenceType.Sk => DomainOfInfluenceTypeType.Sk,
            Domain.Enums.DomainOfInfluenceType.Sc => DomainOfInfluenceTypeType.Sc,
            Domain.Enums.DomainOfInfluenceType.Ki => DomainOfInfluenceTypeType.Ki,
            Domain.Enums.DomainOfInfluenceType.KiKat => DomainOfInfluenceTypeType.Ki,
            Domain.Enums.DomainOfInfluenceType.KiEva => DomainOfInfluenceTypeType.Ki,
            Domain.Enums.DomainOfInfluenceType.Og => DomainOfInfluenceTypeType.Og,
            Domain.Enums.DomainOfInfluenceType.Ko => DomainOfInfluenceTypeType.Ko,
            Domain.Enums.DomainOfInfluenceType.An => DomainOfInfluenceTypeType.An,
            Domain.Enums.DomainOfInfluenceType.AnVek => DomainOfInfluenceTypeType.An,
            Domain.Enums.DomainOfInfluenceType.AnVok => DomainOfInfluenceTypeType.An,
            _ => DomainOfInfluenceTypeType.An,
        };
    }
}
