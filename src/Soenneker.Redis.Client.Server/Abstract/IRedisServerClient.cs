using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Soenneker.Redis.Client.Server.Abstract;

/// <summary>
/// A utility library for Redis server client accessibility
/// </summary>
public interface IRedisServerClient : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Returns the configured server used by the Redis Server Client.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested server.</returns>
    ValueTask<IServer> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the configured server used by the Redis Server Client.
    /// </summary>
    /// <param name="connectionString">Connection string used to open the backing service.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested server.</returns>
    ValueTask<IServer> Get(string connectionString, CancellationToken cancellationToken = default);
}
