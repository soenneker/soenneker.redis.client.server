using Microsoft.Extensions.Logging;
using Soenneker.Dictionaries.Singletons;
using Soenneker.Extensions.ValueTask;
using Soenneker.Redis.Client.Abstract;
using Soenneker.Redis.Client.Server.Abstract;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Redis.Client.Server;

/// <inheritdoc cref="IRedisServerClient"/>
public sealed class RedisServerClient : IRedisServerClient
{
    private const string _defaultKey = "default:";

    private readonly ILogger<RedisServerClient> _logger;
    private readonly IRedisClient _redisClient;

    private readonly SingletonDictionary<IServer, string?> _clients;

    public RedisServerClient(ILogger<RedisServerClient> logger, IRedisClient redisClient)
    {
        _logger = logger;
        _redisClient = redisClient;

        _clients = new SingletonDictionary<IServer, string?>(CreateServer);
    }

    private async ValueTask<IServer> CreateServer(string _, string? connectionString, CancellationToken token)
    {
        _logger.LogDebug(">> RedisServerClient: Building IServer from multiplexer...");

        ConnectionMultiplexer mux = connectionString is null
            ? await _redisClient.Get(token).NoSync()
            : await _redisClient.Get(connectionString, token).NoSync();

        EndPoint[] endpoints = mux.GetEndPoints();

        if (endpoints is null || endpoints.Length == 0)
            throw new InvalidOperationException("Redis ConnectionMultiplexer returned no endpoints.");

        return mux.GetServer(endpoints[0]);
    }

    public ValueTask<IServer> Get(CancellationToken cancellationToken = default) =>
        _clients.Get(_defaultKey, (string?)null, cancellationToken);

    public ValueTask<IServer> Get(string connectionString, CancellationToken cancellationToken = default) =>
        _clients.Get(connectionString, connectionString, cancellationToken);

    /// <summary>
    /// Releases resources used by the current instance.
    /// </summary>
    public void Dispose() => _clients.Dispose();

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask DisposeAsync() => _clients.DisposeAsync();
}