// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Xunit;

namespace Voting.Stimmregister.Test.Utils.Helpers;

/// <summary>
/// This base class uses the writeable db and the db xunit test collection to ensure only 1 test is run in parallel.
/// During initialization it resets the db and seeds the mock data (for each test).
/// </summary>
[Collection(WriteableDbTestCollection.Name)]
public abstract class BaseWriteableDbTest : BaseDbTest<TestApplicationFactory, TestStartup>
{
    protected BaseWriteableDbTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await ResetDb();
    }
}
