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
    /// Gets the value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<IServer> Get(CancellationToken cancellationToken = default);
}