namespace DistributedCache.Cache;

public interface ICacheSyncronizer
{
    Task<bool> PublishCachedItem<TItem>(string channelKey, string operationType, string key, TItem? item);
    Task<bool> ConsumeSyncronizerChannel(string channelKey, ICacheProvider cacheProvider);
}