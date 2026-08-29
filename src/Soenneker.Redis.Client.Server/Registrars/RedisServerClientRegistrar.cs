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
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddRedisServerClientAsSingleton(this IServiceCollection services)
    {
        services.AddRedisClientAsSingleton();
        services.TryAddSingleton<IRedisServerClient, RedisServerClient>();

        return services;
    }

    /// <summary>
    /// Registers Redis Server Client with a scoped lifetime.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddRedisServerClientAsScoped(this IServiceCollection services)
    {
        services.AddRedisClientAsSingleton();
        services.TryAddScoped<IRedisServerClient, RedisServerClient>();

        return services;
    }
}
