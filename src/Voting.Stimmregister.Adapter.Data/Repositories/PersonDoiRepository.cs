// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

/// <inheritdoc cref="PersonDoiRepository"/>
public class PersonDoiRepository : DbRepository<DataContext, PersonDoiEntity>, IPersonDoiRepository
{
    public PersonDoiRepository(DataContext context)
        : base(context)
    {
    }
}
