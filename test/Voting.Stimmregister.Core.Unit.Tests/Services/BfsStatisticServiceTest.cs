// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Domain.Constants;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData.EVoting;
using Xunit;

namespace Voting.Stimmregister.Core.Unit.Tests.Services;

public class BfsStatisticServiceTest : BaseWriteableDbTest
{
    public BfsStatisticServiceTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        GetService<IPermissionService>().SetAbraxasAuthIfNotAuthenticated();
        await base.InitializeAsync();
        await EVotingAuditMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task WhenCreateOrUpdateStatistics_ShouldCreateStatistics()
    {
        var bfsStatisticService = GetService<IBfsStatisticService>();

        var state = new PersonImportStateModel
        {
            MunicipalityId = EVotingBfsMunicipalityMockedData.BfsStGallen,
            MunicipalityName = "St. Gallen",
            EntitiesUnchanged =
            {
                CreatePersonEntity(true, true),
                CreatePersonEntity(false, true),
                CreatePersonEntity(false, false),
            },
        };

        AddNewVoter(state, true, true);
        AddNewVoter(state, false, true);
        AddNewVoter(state, false, false);
        UpdateExistingVoter(state, true, true);
        UpdateExistingVoter(state, false, true);
        UpdateExistingVoter(state, false, false);

        await bfsStatisticService.CreateOrUpdateStatistics(state);

        var bfsStatistics = await RunOnDb(
            db => db.BfsStatistics
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync());

        bfsStatistics.Should().NotBeNull();
        bfsStatistics!.Bfs.Should().Be(EVotingBfsMunicipalityMockedData.BfsStGallen.ToString());
        bfsStatistics!.BfsName.Should().Be("St. Gallen");
        bfsStatistics!.VoterTotalCount.Should().Be(6);
        bfsStatistics!.EVoterTotalCount.Should().Be(3);
        bfsStatistics!.EVoterRegistrationCount.Should().Be(2);
        bfsStatistics!.EVoterDeregistrationCount.Should().Be(1);
    }

    private void AddNewVoter(PersonImportStateModel state, bool isEvoter, bool isVotingAllowed)
    {
        state.Update(
            CreatePersonEntity(isEvoter, isVotingAllowed),
            new PersonEntity
            {
                IsDeleted = true,
                DeletedDate = DateTime.Now,
            });
    }

    private void UpdateExistingVoter(PersonImportStateModel state, bool isEvoter, bool isVotingAllowed)
    {
        state.Update(
            CreatePersonEntity(isEvoter, isVotingAllowed),
            new PersonEntity());
    }

    private PersonEntity CreatePersonEntity(bool isEvoter, bool isVotingAllowed)
    {
        var entity = new PersonEntity { EVoting = isEvoter };

        if (isVotingAllowed)
        {
            entity.DateOfBirth = new DateOnly(2000, 1, 1);
            entity.Country = Countries.Switzerland;
        }

        return entity;
    }
}
