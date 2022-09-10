using DistributedCache.Cache;
using DistributedCache.Models;
using Microsoft.AspNetCore.Mvc;

namespace DistributedCache.Controllers;

[ApiController]
[Route("api/[Controller]/[Action]")]
public class CacheController : ControllerBase
{
    private readonly ICacheProvider _cache;
    private readonly ICacheSyncronizer _cacheSyncronizer;

    public CacheController(ICacheProvider cache, ICacheSyncronizer cacheSyncronizer)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheSyncronizer = cacheSyncronizer ?? throw new ArgumentNullException(nameof(cacheSyncronizer));
    }

    [HttpGet]
    [Route("{key}")]
    public async Task<IActionResult> GetCache(string key)
    {
        return Ok(await _cache.GetFromCache(key));
    }


    [HttpPost]
    public async Task<IActionResult> SetCache([FromBody]SetCacheModel request)
    {
        var result = await _cache.InsertToCache(request.Key, request.Value);
        if (result is false)
            return BadRequest("Item Not Inserted.");

        await _cacheSyncronizer.PublishCachedItem("cache-sync", "insert", request.Key, request.Value);
        return Ok("Item Added Successfully.");
    }


    [HttpPut]
    public async Task<IActionResult> UpdateCache([FromBody]UpdateCacheModel request)
    {
        var result = await _cache.UpdateOnCache(request.Key, request.Value);
        if (result is false)
            return BadRequest("Item Not Updated.");
        await _cacheSyncronizer.PublishCachedItem("cache-sync", "update", request.Key, request.Value);
        return Ok("Item Updated Successfully.");
    }

    [HttpDelete]
    [Route("{key}")]
    public async Task<IActionResult> RemoveCache(string key)
    {
        var result = await _cache.RemoveFromCache(key);
        if (result is false)
            return BadRequest("Item Not Removed.");
        
        await _cacheSyncronizer.PublishCachedItem("cache-sync", "remove", key, string.Empty);
        return Ok("Item Removed Successfully.");
    }
}