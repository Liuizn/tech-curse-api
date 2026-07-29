using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;
using tech_curse_api.src.Application.Interfaces;

namespace tech_curse_api.src.Application.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redisConnection;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redisConnection)
    {
        _cache = cache;
        _redisConnection = redisConnection;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedData = await _cache.GetStringAsync(key);
        if (cachedData == null) return default;

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromHours(1)
        };

        var serializedData = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedData, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefixKey)
    {
        var endpoints = _redisConnection.GetEndPoints();
        var server = _redisConnection.GetServer(endpoints.First());

        var keys = server.Keys(pattern: $"TechCurseAPI_{prefixKey}*").ToArray();

        var db = _redisConnection.GetDatabase();
        await db.KeyDeleteAsync(keys);
    }
}
