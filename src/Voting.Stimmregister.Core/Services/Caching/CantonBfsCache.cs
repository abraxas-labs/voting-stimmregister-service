// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Core.Configuration;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Core.Services.Caching;

public class CantonBfsCache : Cache<Canton, string?>, ICantonBfsCache
{
    public CantonBfsCache(
        IMemoryCache memoryCache,
        IServiceScopeFactory scopeFactory,
        MemoryCacheConfig config)
        : base(CacheNames.CantonBfs, memoryCache, scopeFactory, config.CantonBfs)
    {
    }

    public Task<string?> GetBfsOfCanton(Canton canton)
        => Get(canton);

    protected override async Task<string?> Resolve(IServiceProvider sp, Canton canton)
        => await sp.GetRequiredService<IAccessControlListDoiRepository>().GetCantonBfsByCanton(canton);
}
