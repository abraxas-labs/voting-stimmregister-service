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

public class MunicipalityIdCantonCache : Cache<int, Canton>, IMunicipalityIdCantonCache
{
    public MunicipalityIdCantonCache(
        IMemoryCache memoryCache,
        IServiceScopeFactory scopeFactory,
        MemoryCacheConfig config)
        : base(CacheNames.MunicipalityIdCanton, memoryCache, scopeFactory, config.MunicipalityIdCanton)
    {
    }

    public Task<Canton> GetCantonByMunicipalityId(int municipalityId)
        => Get(municipalityId);

    protected override async Task<Canton> Resolve(IServiceProvider sp, int municipalityId)
        => await sp.GetRequiredService<IAccessControlListDoiRepository>().GetCantonByBfsNumber(municipalityId);
}
