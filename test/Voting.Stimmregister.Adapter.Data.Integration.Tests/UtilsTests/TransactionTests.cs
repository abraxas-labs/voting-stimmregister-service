// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Adapter.Data.Integration.Tests.UtilsTests;

public class TransactionTests : BaseWriteableDbTest
{
    private readonly IPersonRepository _personRepository;
    private readonly IDataContext _dataContext;

    public TransactionTests(TestApplicationFactory factory)
        : base(factory)
    {
        _personRepository = GetService<IPersonRepository>();
        _dataContext = GetService<IDataContext>();
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await PersonMockedData.Seed(RunScoped);
    }

    /// <summary>
    /// Test case:
    /// <list type="number">
    ///     <item>start transaction</item>
    ///     <item>get a seeded person from person repository by first name</item>
    ///     <item>update the person's first name</item>
    ///     <item>update the entity via repository</item>
    ///     <item>throw a demo exception to ensure transaction is incomplete</item>
    ///     <item>assert demo exception is thrown</item>
    ///     <item>get the same seeded person from person repository by first name</item>
    ///     <item>assert person gets resolved</item>
    /// </list>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ShouldRollBackDbChangesWhenTransactionNotCompleted()
    {
        _ = await Assert.ThrowsAsync<DemoException>(async () =>
        {
#pragma warning disable S1854 // Unused assignments should be removed
            await using var transaction = await _dataContext.BeginTransaction();
#pragma warning restore S1854 // Unused assignments should be removed
            var person = await _personRepository.Query()
                .IgnoreQueryFilters()
                .Where(p => p.FirstName.Equals(PersonMockedData.Person_3203_StGallen_2.FirstName))
                .FirstAsync();
            person.FirstName = "This value should not be updated due to rollback";
            await _personRepository.Update(person);
            throw new DemoException();
        });

        // will throw a System.InvalidOperationException if entity was updated in the db context
        await _personRepository.Query()
                .IgnoreQueryFilters()
                .Where(p => p.FirstName.Equals(PersonMockedData.Person_3203_StGallen_2.FirstName))
                .FirstAsync();
    }

    public class DemoException : Exception
    {
    }
}
