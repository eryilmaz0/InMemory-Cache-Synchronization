using System.Diagnostics;
using DistributedCache.Cache;
using StackExchange.Redis;

namespace DistributedCache.BackgroundService;

public class CacheSyncronizerService :Microsoft.Extensions.Hosting.BackgroundService
{
    private ICacheSyncronizer _cacheSyncronizer;
    private ICacheProvider _cacheProvider;
    
    public CacheSyncronizerService(ICacheSyncronizer cacheSyncronizer, ICacheProvider cacheProvider)
    {
        _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        _cacheSyncronizer = cacheSyncronizer ?? throw new ArgumentNullException(nameof(cacheSyncronizer));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cacheSyncronizer.ConsumeSyncronizerChannel("cache-sync", _cacheProvider);
        return Task.CompletedTask;
    }
}