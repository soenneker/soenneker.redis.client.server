using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Redis.Client.Registrars;
using Soenneker.Redis.Client.Server.Abstract;

namespace Soenneker.Redis.Client.Server.Registrars;

/// <summary>
/// A utility library for Redis server client accessibility
/// </summary>
public static class RedisServerClientRegistrar
{
    /// <summary>
    /// Adds <see cref="IRedisServerClient"/> as a singleton service. <para/>
    /// </summary>
    public static void AddRedisServerClientAsSingleton(this IServiceCollection services)
    {
        services.AddRedisClientAsSingleton();
        services.TryAddSingleton<IRedisServerClient, RedisServerClient>();
    }
}