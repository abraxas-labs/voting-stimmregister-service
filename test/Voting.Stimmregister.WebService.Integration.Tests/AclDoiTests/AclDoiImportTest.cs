// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;
using DomainOfInfluenceCanton = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceCanton;
using DomainOfInfluenceType = Abraxas.Voting.Basis.Shared.V1.DomainOfInfluenceType;

namespace Voting.Stimmregister.WebService.Integration.Tests.AclDoiTests;

public class AclDoiImportTest : BaseAclDoiTest
{
    public AclDoiImportTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ShouldImportAclsStGallenAndThurgau()
    {
        await ImportAcl(
            AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH,
            AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH);

        var result = await RunScoped<IAccessControlListDoiRepository, IEnumerable<AccessControlListDoiEntity>>(async r =>
            await r.Query().OrderBy(a => a.Id).ToListAsync());

        result.MatchSnapshot(e => e.ImportStatisticId!);
    }

    [Fact]
    public async Task ShouldImportAclsIncrementally()
    {
        // Incremental import 1
        var import1 = AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH;
        import1.Children.Clear();
        await ImportAcl(import1);

        // Incremental import 2
        await ImportAcl(AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH);

        // Incremental import 3 (full)
        await ImportAcl(
            AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH,
            AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH);

        var result = await RunScoped<IAccessControlListDoiRepository, IEnumerable<AccessControlListDoiEntity>>(async r =>
            await r.Query().OrderBy(a => a.Id).ToListAsync());

        result.MatchSnapshot(e => e.ImportStatisticId!);
    }

    [Fact]
    public async Task ShouldImportAclsAndDeleteStGallenRootTree()
    {
        // Full import
        await ImportAcl(
            AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH,
            AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH);

        // Delete StGallenRootTree
        await ImportAcl(AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH);

        var result = await RunScoped<IAccessControlListDoiRepository, IEnumerable<AccessControlListDoiEntity>>(async r =>
            await r.Query().OrderBy(a => a.Id).ToListAsync());

        result.MatchSnapshot(e => e.ImportStatisticId!);
    }

    [Fact]
    public async Task ShouldImportAclsAndListStatistics()
    {
        // Full import SG_Kanton_StGallen_L1_CH
        await ImportAcl(AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH);

        // Prepare import:
        //  > Add TG_Kanton_Thurgau_L1_CH with all children (2 entities)
        //  > Update SG_Kanton_StGallen_L1_CH name attribute (1 entity)
        //  > Delete SG_Kanton_StGallen_L2_CT subtree (4 entities)
        var sgKantonStGallenL1CH = AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L1_CH;
        sgKantonStGallenL1CH.Name = $"{sgKantonStGallenL1CH.Name} (updated)";
        sgKantonStGallenL1CH.Children.Remove(sgKantonStGallenL1CH.Children.First(e => e.Id == AclDoiVotingBasisMockedData.SG_Kanton_StGallen_L2_CT.Id));
        await ImportAcl(sgKantonStGallenL1CH, AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH);

        var result = await RunScoped<IServiceProvider, IEnumerable<ImportStatisticEntity>>(async s =>
        {
            var statisticsRepo = s.GetRequiredService<IImportStatisticRepository>();
            var acldoiRepo = s.GetRequiredService<IAccessControlListDoiRepository>();

            var statistics = await statisticsRepo
                .Query()
                .Where(e => e.ImportType == ImportType.Acl && e.SourceSystem == ImportSourceSystem.VotingBasis)
                .ToListAsync();

            var statisticIds = statistics.ConvertAll(stat => stat.Id);

            var acls = await acldoiRepo
                .Query()
                .OrderBy(a => a.Id)
                .Where(acl => acl.ImportStatisticId.HasValue && statisticIds.Contains(acl.ImportStatisticId!.Value))
                .ToListAsync();

            Assert.Equal(4, acls.Count);
            return statistics;
        });

        result.MatchSnapshot(e => e.Id, e => e.TotalElapsedMilliseconds!);
    }

    [Fact]
    public async Task ShouldImportAclsAndUpdateInformation()
    {
        // Full import
        await ImportAcl(AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH);

        // Update information
        var doi = AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH;
        doi.Name = $"{AclDoiVotingBasisMockedData.TG_Kanton_Thurgau_L1_CH.Name} (updated)";
        doi.Bfs = "9999";
        doi.TenantName = $"{doi.TenantName} (updated)";
        doi.TenantId = $"{doi.TenantId} (updated)";
        doi.Type = DomainOfInfluenceType.Mu;
        doi.Canton = DomainOfInfluenceCanton.Tg;

        await ImportAcl(doi);

        var result = await RunScoped<IAccessControlListDoiRepository, IEnumerable<AccessControlListDoiEntity>>(async r =>
            await r.Query().OrderBy(a => a.Id).ToListAsync());

        result.MatchSnapshot(e => e.ImportStatisticId!);
    }
}
