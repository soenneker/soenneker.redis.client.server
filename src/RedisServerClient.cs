using System;
using System.Net;
using System.Threading;
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
        _client = new AsyncSingleton<IServer>(async (token, _) =>
        {
            logger.LogDebug(">> RedisServerClient: Building IServer from multiplexer...");

            ConnectionMultiplexer connectionMultiplexer = await redisClient.Get(token).NoSync();

            EndPoint[] endpoints = connectionMultiplexer.GetEndPoints();
            return connectionMultiplexer.GetServer(endpoints[0]);
        });
    }

    public ValueTask<IServer> Get(CancellationToken cancellationToken = default)
    {
        return _client.Get(cancellationToken);
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