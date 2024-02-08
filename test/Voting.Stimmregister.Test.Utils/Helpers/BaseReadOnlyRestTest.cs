// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Test.Utils.Helpers;

/// <summary>
/// A base test class which runs in parallel. The database is initialized with mock data only once.
/// </summary>
public abstract class BaseReadOnlyRestTest : BaseRestTest<TestReadOnlyApplicationFactory, TestReadOnlyDbStartup>
{
    protected BaseReadOnlyRestTest(TestReadOnlyApplicationFactory factory)
        : base(factory)
    {
    }
}
