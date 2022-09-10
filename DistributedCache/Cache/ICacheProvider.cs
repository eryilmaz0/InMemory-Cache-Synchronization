namespace DistributedCache.Cache;

public interface ICacheProvider
{
    Task<bool> IsKeyExist(string key);
    Task<TItem> GetFromCache<TItem>(string key);
    Task<object> GetFromCache(string key);
    Task<bool> RemoveFromCache(string key);
    Task<bool> InsertToCache<TItem>(string key, TItem item, bool overrideIfExist = true);
    Task<bool> UpdateOnCache<TItem>(string key, TItem item);
}