// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.AclDoiTests;

public class AclDoiServiceTest : BaseAclDoiTest
{
    public AclDoiServiceTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await ImportAcl(
            AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH,
            AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH);
    }

    [Fact]
    public async Task ShouldGetDoiAclListByTenantIdDienstFuerPolitischeRechte()
    {
        var result = await RunScopedAuthenticated<IAccessControlListDoiService, List<AccessControlListDoiEntity>>(async service =>
            (await service.GetDoiAccessControlListByTenantId(AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH.TenantId))
                .Where(e => e.Parent == null)
                .OrderBy(e => e.Id)
                .ToList());

        IgnoreDynamicProperties(result);
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldGetDoiAclListByTenantIdStadtStGallen()
    {
        var result = await RunScopedAuthenticated<IAccessControlListDoiService, List<AccessControlListDoiEntity>>(async service =>
            (await service.GetDoiAccessControlListByTenantId(AclDoiVotingBasisMockedData.SG_StGallen_L5_MU.TenantId))
                .OrderBy(e => e.Id)
                .ToList());

        result.ForEach(r => r.Parent = null);
        result.ForEach(r => r.ImportStatisticId = null);
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldGetBfsNumberAclListByTenantIdDienstFuerPolitischeRechteOfTypeMu()
    {
        var result = await RunScopedAuthenticated<IAccessControlListDoiService, List<string>>(async service =>
            (await service.GetBfsNumberAccessControlListByTenantId(AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH.TenantId, DomainOfInfluenceType.Mu))
                .OrderBy(e => e)
                .ToList());

        result.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldGetBfsNumberAclListByTenantIdStadtStGallenOfTypeMu()
    {
        var result = await RunScopedAuthenticated<IAccessControlListDoiService, List<string>>(async service =>
            (await service.GetBfsNumberAccessControlListByTenantId(AclDoiVotingBasisMockedData.SG_StGallen_L5_MU.TenantId, DomainOfInfluenceType.Mu))
                .OrderBy(e => e)
                .ToList());

        result.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldGetEmptyBfsNumberAclListByNonExistingTenantIdOfTypeMu()
    {
        var result = await RunScopedAuthenticated<IAccessControlListDoiService, List<string>>(async service =>
            (await service.GetBfsNumberAccessControlListByTenantId("0000000", DomainOfInfluenceType.Mu))
                .OrderBy(e => e)
                .ToList());

        Assert.Empty(result);
    }

    private void IgnoreDynamicProperties(
        IEnumerable<AccessControlListDoiEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.ImportStatisticId = Guid.Empty;
            entity.ParentId = Guid.Empty;
            IgnoreDynamicProperties(entity.Children);
        }
    }
}
