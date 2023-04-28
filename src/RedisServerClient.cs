using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Redis.Client.Abstract;
using Soenneker.Redis.Client.Server.Abstract;
using Soenneker.Redis.Util;
using Soenneker.Redis.Util.Abstract;
using Soenneker.Utils.AsyncSingleton;
using StackExchange.Redis;

namespace Soenneker.Redis.Client.Server;

/// <inheritdoc cref="IRedisServerClient"/>
public class RedisServerClient : IRedisServerClient
{
    private readonly AsyncSingleton<IServer> _client;
    
    private readonly ILogger<RedisServerClient> _logger;
    private readonly IRedisUtil _redisUtil;

    public RedisServerClient(ILogger<RedisServerClient> logger, IRedisUtil redisUtil, IRedisClient redisClient)
    {
        _logger = logger;
        _redisUtil = redisUtil;

        _client = new AsyncSingleton<IServer>(async () =>
        {
            _logger.LogDebug(">> RedisServerClient: Building IServer from multiplexor...");

            ConnectionMultiplexer connectionMultiplexor = await redisClient.GetClient();

            EndPoint[] endpoints = connectionMultiplexor.GetEndPoints();
            IServer client = connectionMultiplexor.GetServer(endpoints[0]);

            return client;
        });
    }

    private ValueTask<IServer> GetClient()
    {
        return _client.Get();
    }

    public async ValueTask<Dictionary<string, T>?> GetKeyValuesByPrefix<T>(string redisKeyPrefix) where T : class
    {
        List<RedisKey>? keys = await GetKeysByPrefixList(redisKeyPrefix);

        if (keys == null)
            return null;

        var dictionary = new Dictionary<string, T>();

        foreach (RedisKey redisKey in keys)
        {
            var redisKeyStr = redisKey.ToString();

            var result = await _redisUtil.Get<T>(redisKeyStr);

            if (result != null)
                dictionary.TryAdd(redisKeyStr, result);
        }

        return dictionary;
    }

    public async ValueTask<Dictionary<string, string>?> GetKeyValuesByPrefixWithoutDeserialization(string redisKeyPrefix)
    {
        List<RedisKey>? keys = await GetKeysByPrefixList(redisKeyPrefix);

        if (keys == null)
            return null;

        var dictionary = new Dictionary<string, string>();

        foreach (RedisKey redisKey in keys)
        {
            var redisKeyStr = redisKey.ToString();

            string? result = await _redisUtil.GetString(redisKeyStr);

            if (result != null)
                dictionary.TryAdd(redisKeyStr, result);
        }

        return dictionary;
    }

    public async ValueTask<Dictionary<string, T>?> GetKeyValueHashesByPrefix<T>(string redisKeyPrefix, string field) where T : class
    {
        List<RedisKey>? keys = await GetKeysByPrefixList(redisKeyPrefix);

        if (keys == null)
            return null;

        var dictionary = new Dictionary<string, T>();

        foreach (RedisKey redisKey in keys)
        {
            var redisKeyStr = redisKey.ToString();

            var result = await _redisUtil.GetHash<T>(redisKeyStr, field);

            if (result != null)
                dictionary.TryAdd(redisKeyStr, result);
        }

        return dictionary;
    }

    public ValueTask<Dictionary<string, T>?> GetKeyValuesByPrefix<T>(string cacheKey, string? prefix) where T : class
    {
        string redisKeyPrefix = RedisUtil.BuildKey(cacheKey, prefix) + '*';

        return GetKeyValuesByPrefix<T>(redisKeyPrefix);
    }

    public ValueTask<Dictionary<string, string>?> GetKeyValuesByPrefixWithoutDeserialization(string cacheKey, string? prefix)
    {
        string redisKeyPrefix = RedisUtil.BuildKey(cacheKey, prefix) + '*';

        return GetKeyValuesByPrefixWithoutDeserialization(redisKeyPrefix);
    }

    public async ValueTask<IAsyncEnumerable<RedisKey>?> GetKeysByPrefix(string redisKeyPrefix)
    {
        var keyPattern = $"{redisKeyPrefix}*";

        var redisValue = new RedisValue(keyPattern);

        IAsyncEnumerable<RedisKey> keys = (await GetClient()).KeysAsync(pattern: redisValue);

        return keys;
    }

    public ValueTask<IAsyncEnumerable<RedisKey>?> GetKeysByPrefix(string cacheKey, string? prefix)
    {
        string redisKeyPrefix = RedisUtil.BuildKey(cacheKey, prefix) + '*';
        return GetKeysByPrefix(redisKeyPrefix);
    }

    public async ValueTask<List<RedisKey>?> GetKeysByPrefixList(string redisKeyPrefix)
    {
        IAsyncEnumerable<RedisKey>? result = await GetKeysByPrefix(redisKeyPrefix);

        if (result == null)
            return null;

        var list = new List<RedisKey>();

        await foreach (RedisKey item in result)
        {
            list.Add(item);
        }

        return list;
    }

    public ValueTask<List<RedisKey>?> GetKeysByPrefixList(string cacheKey, string? prefix)
    {
        string redisKeyPrefix = RedisUtil.BuildKey(cacheKey, prefix) + '*';

        return GetKeysByPrefixList(redisKeyPrefix);
    }

    public ValueTask RemoveByPrefix(string cacheKey, string? prefix = null, bool fireAndForget = false)
    {
        string redisPrefixKey = RedisUtil.BuildKey(cacheKey, prefix) + '*';

        return RemoveByPrefix(redisPrefixKey, fireAndForget);
    }

    public async ValueTask RemoveByPrefix(string redisPrefixKey, bool fireAndForget = false)
    {
        List<RedisKey>? keys = await GetKeysByPrefixList(redisPrefixKey);

        if (keys == null)
            return;

        _logger.LogWarning(">> REDIS: Removing keys matching: {key} ...", redisPrefixKey);

        foreach (RedisKey key in keys)
        {
            try
            {
                var keyStr = key.ToString();

                await _redisUtil.Remove(keyStr, fireAndForget);
            }
            catch (Exception e)
            {
                _logger.LogError(e, ">> REDIS: Error removing keys matching: {key}", redisPrefixKey);
            }
        }
    }

    public async ValueTask Flush()
    {
        _logger.LogWarning(">> RedisServerClient: Flushing...");

        try
        {
            await (await GetClient()).FlushAllDatabasesAsync();

            _logger.LogDebug(">> RedisServerClient: Flushed successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, ">> RedisServerClient: Error flushing redis server");
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _client.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _client.DisposeAsync();
    }
}