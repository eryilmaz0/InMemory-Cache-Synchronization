using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace DistributedCache.Cache;

public class InMemoryCacheProvider : ICacheProvider
{
    private readonly IMemoryCache _cache;

    public InMemoryCacheProvider(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<bool> IsKeyExist(string key)
    {
        return await Task.FromResult(_cache.TryGetValue(key, out object _));
    }

    public async Task<TItem> GetFromCache<TItem>(string key)
    {
        if (await this.IsKeyExist(key))
            return _cache.Get<TItem>(key);

        return default(TItem);
    }

    public async Task<object> GetFromCache(string key)
    {
        if (await this.IsKeyExist(key))
            return _cache.Get(key);

        return null;
    }

    public async Task<bool> RemoveFromCache(string key)
    {
        if (await this.IsKeyExist(key))
        {
            _cache.Remove(key);
            return true;
        }

        return false;
    }
    public async Task<bool> InsertToCache<TItem>(string key, TItem item, bool overrideIfExist = true)
    {
        if (overrideIfExist || !await this.IsKeyExist(key))
        {
            _cache.Set<TItem>(key, item);
            return true;
        }
        
        return false;
    }

    public async Task<bool> UpdateOnCache<TItem>(string key, TItem item)
    {
        if (await this.IsKeyExist(key))
        {
            return await this.InsertToCache(key, item);
        }

        return false;
    }
}