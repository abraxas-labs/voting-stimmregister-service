// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Configuration;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="IDatabaseCleanupService"/>
public class DatabaseCleanupService(
    CleanupConfig config,
    IFilterVersionRepository filterVersionRepository,
    IPersonRepository personRepository,
    IClock clock,
    ILogger<DatabaseCleanupService> logger) : IDatabaseCleanupService
{
    public async Task<DatabaseCleanupResultModel> RunCleanup()
    {
        var cleanupReport = new DatabaseCleanupResultModel();

        try
        {
            cleanupReport.FilterVersionsDeleted = await CleanUpFilterVersions(config.FilterVersionMinimumLifetimeInDays);
            logger.LogInformation(
                "{FilterVersionsDeleted} FilterVersions have been deleted according to the clean up policy",
                cleanupReport.FilterVersionsDeleted);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clean up FilterVersions");
        }

        try
        {
            cleanupReport.PersonVersionsDeleted = await CleanUpPersonVersions(config.PersonVersionMinimumLifetimeInDays);
            logger.LogInformation(
                "{PersonVersionsDeleted} PersonVersions have been deleted according to the clean up policy",
                cleanupReport.PersonVersionsDeleted);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clean up PersonVersions.");
        }

        return cleanupReport;
    }

    private Task<int> CleanUpFilterVersions(int minLifetimeInDays)
    {
        var thresholdDate = GetThresholdDate(minLifetimeInDays);

        return filterVersionRepository.DeleteOutdatedFilterVersions(thresholdDate, config.SqlCommandTimeoutInSeconds);
    }

    private Task<int> CleanUpPersonVersions(int minLifetimeInDays)
    {
        var thresholdDate = GetThresholdDate(minLifetimeInDays);

        return personRepository.DeleteOutdatedPersonVersions(thresholdDate, config.SqlCommandTimeoutInSeconds);
    }

    private DateTime GetThresholdDate(int minLifetimeInDays)
    {
        return clock.UtcNow.AddDays(-minLifetimeInDays);
    }
}
