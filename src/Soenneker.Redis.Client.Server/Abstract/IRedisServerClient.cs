using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Soenneker.Redis.Client.Server.Abstract;

/// <summary>
/// Provides cached Redis server endpoints backed by shared connection multiplexers.
/// </summary>
public interface IRedisServerClient : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the first server endpoint for the configured Redis connection.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The cached server endpoint.</returns>
    ValueTask<IServer> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first server endpoint for a Redis connection string.
    /// </summary>
    /// <param name="connectionString">Connection string used to open the backing service.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The cached server endpoint for <paramref name="connectionString"/>.</returns>
    ValueTask<IServer> Get(string connectionString, CancellationToken cancellationToken = default);
}
