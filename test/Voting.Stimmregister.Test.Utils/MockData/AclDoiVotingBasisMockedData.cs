// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abraxas.Voting.Basis.Services.V1.Models;
using Abraxas.Voting.Basis.Shared.V1;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

public static class AclDoiVotingBasisMockedData
{
    public static PoliticalDomainOfInfluence SG_Kanton_StGallen_L1_CH
        => new()
        {
            Id = "24b12f20-a4a9-4f62-a805-60e7c61359e2",
            Name = "Kanton St.Gallen (CH)",
            ParentId = string.Empty,
            Bfs = "1",
            TenantName = "Dienst für Politische Rechte",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Ch,
            Canton = DomainOfInfluenceCanton.Sg,
            Children = { SG_Auslandschweizer_L2_MU, SG_Kanton_StGallen_L2_CT },
            ReturnAddress = new DomainOfInfluenceVotingCardReturnAddress
            {
                AddressLine1 = "Staatskanzlei St. Gallen",
            },
        };

    public static PoliticalDomainOfInfluence SG_Auslandschweizer_L2_MU
        => new()
        {
            Id = "e4f482e6-20d8-4c8f-9e31-fecf092bf06b",
            Name = "Auslandschweizer SG (MU)",
            ParentId = "24b12f20-a4a9-4f62-a805-60e7c61359e2",
            Bfs = "9170",
            TenantName = "Dienst für Politische Rechte",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Mu,
            Canton = DomainOfInfluenceCanton.Unspecified,
            Children = { },
        };

    public static PoliticalDomainOfInfluence SG_Kanton_StGallen_L2_CT
        => new()
        {
            Id = "5a9772c4-a504-40b3-88b2-620af634d9b5",
            Name = "Kanton St. Gallen SG (CT)",
            ParentId = "24b12f20-a4a9-4f62-a805-60e7c61359e2",
            Bfs = "17",
            TenantName = "Dienst für Politische Rechte",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Ct,
            Canton = DomainOfInfluenceCanton.Unspecified,
            Children = { SG_Gerichtskreis_StGallen_L3_BZ },
        };

    public static PoliticalDomainOfInfluence SG_Gerichtskreis_StGallen_L3_BZ
        => new()
        {
            Id = "27e721c1-0590-40f7-94da-1526ab47c5d4",
            Name = "Gerichtskreis St.Gallen (BZ)",
            ParentId = "5a9772c4-a504-40b3-88b2-620af634d9b5",
            Bfs = string.Empty,
            TenantName = "Dienst für Politische Rechte",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Bz,
            Canton = DomainOfInfluenceCanton.Unspecified,
            Children = { SG_Wahlkreis_StGallen_L4_BZ },
        };

    public static PoliticalDomainOfInfluence SG_Wahlkreis_StGallen_L4_BZ
        => new()
        {
            Id = "03fd4057-9da9-4fd5-8310-d4d420762aef",
            Name = "Wahlkreis St.Gallen (BZ)",
            ParentId = "27e721c1-0590-40f7-94da-1526ab47c5d4",
            Bfs = string.Empty,
            TenantName = "Dienst für Politische Rechte",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Bz,
            Canton = DomainOfInfluenceCanton.Unspecified,
            Children = { SG_StGallen_L5_MU, SG_Goldach_L5_MU, SG_Moerschwil_L5_MU, SG_Rorschacherberg_L5_MU },
        };

    public static PoliticalDomainOfInfluence SG_StGallen_L5_MU
        => new()
        {
            Id = "a500760a-3e0f-4c98-a13a-9a614b74127a",
            Name = "St.Gallen (MU)",
            ParentId = "03fd4057-9da9-4fd5-8310-d4d420762aef",
            Bfs = "3203",
            TenantName = "Stadt St.Gallen",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Mu,
            Canton = DomainOfInfluenceCanton.Unspecified,
        };

    public static PoliticalDomainOfInfluence SG_Goldach_L5_MU
        => new()
        {
            Id = "a8d6078f-5ace-4284-a99f-d33a5df47f23",
            Name = "Goldach (MU)",
            ParentId = "03fd4057-9da9-4fd5-8310-d4d420762aef",
            Bfs = "3213",
            TenantName = "Gemeinde Goldach",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Mu,
            Canton = DomainOfInfluenceCanton.Unspecified,
            ReturnAddress = new DomainOfInfluenceVotingCardReturnAddress
            {
                AddressLine1 = "Gemeindekanzlei Goldach",
                City = "Goldach",
                Country = "SWITZERLAND",
                Street = "Postfach",
                ZipCode = "9403",
            },
        };

    public static PoliticalDomainOfInfluence SG_Rorschacherberg_L5_MU
        => new()
        {
            Id = "b0570f77-a2a8-4fb9-8092-d6916776624f",
            Name = "Rorschacherberg (MU)",
            ParentId = "03fd4057-9da9-4fd5-8310-d4d420762aef",
            Bfs = "3216",
            TenantName = "Gemeinde Rorschacherberg",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Mu,
            Canton = DomainOfInfluenceCanton.Unspecified,
        };

    public static PoliticalDomainOfInfluence SG_Moerschwil_L5_MU
        => new()
        {
            Id = "f53c1865-30e3-4c1e-88ae-7ea471daa4b4",
            Name = "Mörschwil (MU)",
            ParentId = "03fd4057-9da9-4fd5-8310-d4d420762aef",
            Bfs = "3214",
            TenantName = "Gemeinde Mörschwil",
            TenantId = VotingIamTenantIds.KTSG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Mu,
            Canton = DomainOfInfluenceCanton.Unspecified,
        };

    public static PoliticalDomainOfInfluence TG_Kanton_Thurgau_L1_CH
        => new()
        {
            Id = "cf0fb17f-8e71-4f9e-ab80-6412d42d00aa",
            Name = "Kanton Thurgau (CH)",
            ParentId = string.Empty,
            Bfs = string.Empty,
            TenantName = "Staatskanzlei des Kantons Thurgau",
            TenantId = VotingIamTenantIds.KTTG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Ch,
            Canton = DomainOfInfluenceCanton.Tg,
            Children = { TG_Auslandschweizer_L2_MU },
        };

    public static PoliticalDomainOfInfluence TG_Auslandschweizer_L2_MU
        => new()
        {
            Id = "3e2ff4ab-4147-4b29-bac7-1651e7d8422d",
            Name = "Auslandschweizer (MU)",
            ParentId = "cf0fb17f-8e71-4f9e-ab80-6412d42d00aa",
            Bfs = "4000",
            TenantName = "Staatskanzlei des Kantons Thurgau",
            TenantId = VotingIamTenantIds.KTTG,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Mu,
            Canton = DomainOfInfluenceCanton.Unspecified,
        };

    public static PoliticalDomainOfInfluence Abraxas_L1_CH
        => new()
        {
            Id = "b8bcb873-6307-4cf0-82c1-847b530d4d43",
            Name = "Abraxas (CH)",
            ParentId = string.Empty,
            Bfs = string.Empty,
            TenantName = "Abraxas",
            TenantId = VotingIamTenantIds.Abraxas,
            Type = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType.Ch,
            Canton = DomainOfInfluenceCanton.Sg,
        };

    public static IEnumerable<AccessControlListDoiEntity> All
    {
        get
        {
            yield return GetMockedEntity(SG_Kanton_StGallen_L1_CH);
            yield return GetMockedEntity(SG_Auslandschweizer_L2_MU);
            yield return GetMockedEntity(SG_Kanton_StGallen_L2_CT);
            yield return GetMockedEntity(SG_Gerichtskreis_StGallen_L3_BZ);
            yield return GetMockedEntity(SG_Wahlkreis_StGallen_L4_BZ);
            yield return GetMockedEntity(SG_StGallen_L5_MU);
            yield return GetMockedEntity(SG_Goldach_L5_MU);
            yield return GetMockedEntity(SG_Moerschwil_L5_MU);
            yield return GetMockedEntity(SG_Rorschacherberg_L5_MU);
            yield return GetMockedEntity(TG_Kanton_Thurgau_L1_CH);
            yield return GetMockedEntity(TG_Auslandschweizer_L2_MU);
            yield return GetMockedEntity(Abraxas_L1_CH);
        }
    }

    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped)
    {
        return runScoped(async sp =>
        {
            var db = sp.GetRequiredService<IDataContext>();
            db.AccessControlListDois.AddRange(All);
            await db.SaveChangesAsync();
        });
    }

    private static AccessControlListDoiEntity GetMockedEntity(PoliticalDomainOfInfluence model)
    {
        var entity = new AccessControlListDoiEntity
        {
            Id = Guid.Parse(model.Id),
            Name = model.Name,
            Bfs = model.Bfs,
            TenantName = model.TenantName,
            TenantId = model.TenantId,
            Type = (Domain.Enums.DomainOfInfluenceType)model.Type,
            Canton =
                model.Canton == DomainOfInfluenceCanton.Unspecified
                    ? Canton.Unknown
                    : Enum.Parse<Canton>(model.Canton.ToString(), ignoreCase: true),
            ParentId = string.IsNullOrWhiteSpace(model.ParentId) ? null : Guid.Parse(model.ParentId),
            IsValid = true,
        };

        if (model.ReturnAddress != null)
        {
            entity.ReturnAddress = new AccessControlListDoiReturnAddress
            {
                AddressAddition = model.ReturnAddress.AddressAddition,
                AddressLine1 = model.ReturnAddress.AddressLine1,
                AddressLine2 = model.ReturnAddress.AddressLine2,
                City = model.ReturnAddress.City,
                Country = model.ReturnAddress.Country,
                Street = model.ReturnAddress.Street,
                ZipCode = model.ReturnAddress.ZipCode,
            };
        }

        return entity;
    }
}
