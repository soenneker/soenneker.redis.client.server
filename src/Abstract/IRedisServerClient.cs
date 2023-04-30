using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Soenneker.Redis.Client.Server.Abstract;

/// <summary>
/// A utility library for Redis server client accessibility
/// </summary>
public interface IRedisServerClient : IDisposable, IAsyncDisposable
{
    ValueTask<IServer> GetClient();
}