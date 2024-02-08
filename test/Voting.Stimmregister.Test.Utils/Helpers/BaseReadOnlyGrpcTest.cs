// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Grpc.Core;

namespace Voting.Stimmregister.Test.Utils.Helpers;

/// <summary>
/// A base test class which runs in parallel. The database is initialized with mock data only once.
/// </summary>
/// <typeparam name="TService">The grpc service client type.</typeparam>
public abstract class BaseReadOnlyGrpcTest<TService> : BaseGrpcTest<TService, TestReadOnlyApplicationFactory, TestReadOnlyDbStartup>
    where TService : ClientBase<TService>
{
    protected BaseReadOnlyGrpcTest(TestReadOnlyApplicationFactory factory)
        : base(factory)
    {
    }
}
