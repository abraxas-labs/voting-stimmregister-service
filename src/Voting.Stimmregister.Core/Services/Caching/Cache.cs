// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Common;

namespace Voting.Stimmregister.Core.Services.Caching;

public abstract class Cache<TKey, TElement> : IAsyncDisposable
{
    private readonly CacheNames _cacheName;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MemoryCacheEntryOptions _entryOptions;
    private readonly AsyncLock _resolverLock = new();

    protected Cache(
        CacheNames cacheName,
        IMemoryCache memoryCache,
        IServiceScopeFactory scopeFactory,
        MemoryCacheEntryOptions entryOptions)
    {
        _cacheName = cacheName;
        _memoryCache = memoryCache;
        _scopeFactory = scopeFactory;
        _entryOptions = entryOptions;
    }

    protected enum CacheNames
    {
        CantonBfs,
        MunicipalityIdCanton,
        EVoters,
    }

    public ValueTask DisposeAsync()
        => _resolverLock.DisposeAsync();

    public async Task<TElement?> Get(TKey key)
    {
        var cacheKey = $"{_cacheName}-{key}";
        if (_memoryCache.TryGetValue(cacheKey, out TElement value))
        {
            return value;
        }

        using var locker = await _resolverLock.AcquireAsync();
        if (_memoryCache.TryGetValue(cacheKey, out TElement addedValue))
        {
            return addedValue;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var receivedValue = await Resolve(scope.ServiceProvider, key);
        _memoryCache.Set(cacheKey, receivedValue, _entryOptions);
        return receivedValue;
    }

    protected abstract Task<TElement?> Resolve(IServiceProvider sp, TKey key);
}
