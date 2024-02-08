// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Core.Configuration;
using Voting.Stimmregister.Domain.Cache;

namespace Voting.Stimmregister.Core.Services.Caching;

public class EVotersCache : Cache<short, HashSet<long>>, IEVotersCache
{
    private readonly IMunicipalityIdCantonCache _municipalityIdCantonCache;
    private readonly ICantonBfsCache _cantonBfsCache;

    public EVotersCache(
        IMemoryCache memoryCache,
        IServiceScopeFactory scopeFactory,
        MemoryCacheConfig config,
        IMunicipalityIdCantonCache municipalityIdCantonCache,
        ICantonBfsCache cantonBfsCache)
        : base(CacheNames.EVoters, memoryCache, scopeFactory, config.EVoters)
    {
        _municipalityIdCantonCache = municipalityIdCantonCache;
        _cantonBfsCache = cantonBfsCache;
    }

    public async Task<HashSet<long>> GetEnabledAhvN13ForCantonWithMunicipalityId(int municipalityId)
    {
        var canton = await _municipalityIdCantonCache.GetCantonByMunicipalityId(municipalityId);
        var bfs = await _cantonBfsCache.GetBfsOfCanton(canton);
        if (bfs == null)
        {
            return new HashSet<long>();
        }

        return await Get(short.Parse(bfs))
            ?? new HashSet<long>();
    }

    protected override async Task<HashSet<long>?> Resolve(IServiceProvider sp, short cantonBfs)
        => await sp.GetRequiredService<IEVoterRepository>().GetEnabledAhvN13(cantonBfs);
}
