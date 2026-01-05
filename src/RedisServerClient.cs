using Microsoft.Extensions.Logging;
using Soenneker.Extensions.ValueTask;
using Soenneker.Redis.Client.Abstract;
using Soenneker.Redis.Client.Server.Abstract;
using Soenneker.Utils.AsyncSingleton;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Redis.Client.Server;

/// <inheritdoc cref="IRedisServerClient"/>
public sealed class RedisServerClient : IRedisServerClient
{
    private readonly ILogger<RedisServerClient> _logger;
    private readonly IRedisClient _redisClient;

    private readonly AsyncSingleton<IServer> _client;

    public RedisServerClient(ILogger<RedisServerClient> logger, IRedisClient redisClient)
    {
        _logger = logger;
        _redisClient = redisClient;

        // No closure: method group
        _client = new AsyncSingleton<IServer>(CreateServerAsync);
    }

    private async ValueTask<IServer> CreateServerAsync(CancellationToken token)
    {
        _logger.LogDebug(">> RedisServerClient: Building IServer from multiplexer...");

        ConnectionMultiplexer mux = await _redisClient.Get(token).NoSync();

        EndPoint[] endpoints = mux.GetEndPoints();

        // Defensive: GetEndPoints() should return at least one; if not, fail loudly.
        if (endpoints is null || endpoints.Length == 0)
            throw new InvalidOperationException("Redis ConnectionMultiplexer returned no endpoints.");

        return mux.GetServer(endpoints[0]);
    }

    public ValueTask<IServer> Get(CancellationToken cancellationToken = default) =>
        _client.Get(cancellationToken);

    public void Dispose() => _client.Dispose();

    public ValueTask DisposeAsync() => _client.DisposeAsync();
}