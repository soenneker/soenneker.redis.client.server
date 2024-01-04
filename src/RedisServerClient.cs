using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.ValueTask;
using Soenneker.Redis.Client.Abstract;
using Soenneker.Redis.Client.Server.Abstract;
using Soenneker.Utils.AsyncSingleton;
using StackExchange.Redis;

namespace Soenneker.Redis.Client.Server;

/// <inheritdoc cref="IRedisServerClient"/>
public class RedisServerClient : IRedisServerClient
{
    private readonly AsyncSingleton<IServer> _client;

    public RedisServerClient(ILogger<RedisServerClient> logger, IRedisClient redisClient)
    {
        _client = new AsyncSingleton<IServer>(async () =>
        {
            logger.LogDebug(">> RedisServerClient: Building IServer from multiplexer...");

            ConnectionMultiplexer connectionMultiplexer = await redisClient.GetClient().NoSync();

            EndPoint[] endpoints = connectionMultiplexer.GetEndPoints();
            IServer client = connectionMultiplexer.GetServer(endpoints[0]);

            return client;
        });
    }

    public ValueTask<IServer> GetClient()
    {
        return _client.Get();
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