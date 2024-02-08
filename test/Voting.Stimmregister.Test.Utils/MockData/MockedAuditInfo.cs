// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Testing;
using Voting.Lib.Testing.Mocks;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

public static class MockedAuditInfo
{
    public static AuditInfo Get(int createdDaysDelta = 0, int modifiedDaysDelta = 0)
    {
        return new()
        {
            CreatedAt = MockedClock.GetDate(createdDaysDelta),
            ModifiedAt = MockedClock.GetDate(modifiedDaysDelta),
            CreatedById = TestDefaults.UserId,
            CreatedByName = "MockDataSeeder",
            ModifiedById = TestDefaults.UserId,
            ModifiedByName = "MockDataSeeder",
        };
    }
}
