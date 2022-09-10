using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DistributedCache.Cache;

public class RedisCacheSyncronizer : ICacheSyncronizer
{
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    private ISubscriber _subscriber;
    private ICacheProvider _cacheProvider;

    public RedisCacheSyncronizer(ICacheProvider cacheProvider)
    {
        this._cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        _connection = ConnectionMultiplexer.Connect("Redis:6379");
        _subscriber = _connection.GetSubscriber();
        _database = _connection.GetDatabase(0);
    }
    
    public async Task<bool> PublishCachedItem<TItem>(string channelKey, string operationType, string key, TItem? item)
    {
        var formattedMessage = FormatChannelMessage(operationType, key, item);
        await this.UpdateRedis(operationType, key, JsonSerializer.Serialize(item));
        await _subscriber.PublishAsync(channelKey, formattedMessage);
        return true;
    }

    public async Task<bool> ConsumeSyncronizerChannel(string channelKey, ICacheProvider cacheProvider)
    {
        await _subscriber.SubscribeAsync(channelKey, async(channel, value) =>
        {
            var resolvedMessage = ResolveMessage(value);
            bool result = resolvedMessage.operationType switch
            {
                "insert" => await cacheProvider.InsertToCache(resolvedMessage.key, resolvedMessage.value),
                "update" => await cacheProvider.UpdateOnCache(resolvedMessage.key, resolvedMessage.value),
                "remove" => await cacheProvider.RemoveFromCache(resolvedMessage.key)
            };
        });
        return true;
    }


    private async Task<bool> UpdateRedis(string operationType, string key, string? value)
    {
        return operationType switch
        {
            "insert" => await _database.StringSetAsync(key, value),
            "update" => await _database.StringSetAsync(key, value),
            "remove" => await _database.KeyDeleteAsync(key)
        };
    }

    private (string operationType, string key, string? value) ResolveMessage(string message)
    {
        var splittedMessage = message.Split('.');

        if(splittedMessage.Length.Equals(2))
            return (splittedMessage[0], splittedMessage[1], null);
        else
            return (splittedMessage[0], splittedMessage[1], splittedMessage[2]);
    }


    private string FormatChannelMessage(string operationType, string key, object? value)
    {
        bool isOperationRemove = operationType == "remove";
        var message = isOperationRemove ? string.Empty : JsonSerializer.Serialize(value);

        if (isOperationRemove)
            return $"{operationType}.{key}";

        return $"{operationType}.{key}.{value}";
    }
}