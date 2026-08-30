using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Redis.Client.Registrars;
using Soenneker.Redis.Client.Server.Abstract;

namespace Soenneker.Redis.Client.Server.Registrars;

/// <summary>
/// Registers Redis server endpoint access.
/// </summary>
public static class RedisServerClientRegistrar
{
    /// <summary>
    /// Adds <see cref="IRedisServerClient"/> and its backing Redis client as singleton services.
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
    /// Adds a scoped <see cref="IRedisServerClient"/> backed by a singleton Redis client.
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
