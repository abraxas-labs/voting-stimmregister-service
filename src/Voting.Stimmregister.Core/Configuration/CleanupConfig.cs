// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Scheduler;

namespace Voting.Stimmregister.Core.Configuration;

public class CleanupConfig : CronJobConfig
{
    public bool IsActive { get; set; }

    public int FilterVersionMinimumLifetimeInDays { get; set; }

    public int PersonVersionMinimumLifetimeInDays { get; set; }

    public int SqlCommandTimeoutInSeconds { get; set; }
}
