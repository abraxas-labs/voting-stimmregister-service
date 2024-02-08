// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.Extensions.Caching.Memory;

namespace Voting.Stimmregister.Abstractions.Core.Configuration;

public class MemoryCacheConfig
{
    public MemoryCacheEntryOptions MunicipalityIdCanton { get; set; } = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
    };

    public MemoryCacheEntryOptions CantonBfs { get; set; } = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
    };

    public MemoryCacheEntryOptions EVoters { get; set; } = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3),
    };
}
