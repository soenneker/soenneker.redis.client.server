[![](https://img.shields.io/nuget/v/Soenneker.Redis.Client.Server.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Redis.Client.Server/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.client.server/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.redis.client.server/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Redis.Client.Server.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Redis.Client.Server/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.client.server/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.redis.client.server/actions/workflows/codeql.yml)

# Soenneker.Redis.Client.Server

A utility library for Redis server client accessibility.

## Install

```bash
dotnet add package Soenneker.Redis.Client.Server
```

## Quick start

```csharp
using Soenneker.Redis.Client.Server.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddRedisServerClientAsSingleton();
```

Adds `IRedisServerClient` as a singleton service.

## What you get

- `IRedisServerClient` — A utility library for Redis server client accessibility.
- `RedisServerClientRegistrar` — A utility library for Redis server client accessibility.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `RedisServerClientRegistrar.AddRedisServerClientAsSingleton(services)` | Adds `IRedisServerClient` as a singleton service. | The same service collection, so additional registrations can be chained. |
| `RedisServerClientRegistrar.AddRedisServerClientAsScoped(services)` | Registers Redis Server Client with a scoped lifetime. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Dispose instances you own when their scope ends so held resources can be released.
