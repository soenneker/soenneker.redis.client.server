[![](https://img.shields.io/nuget/v/Soenneker.Redis.Client.Server.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Redis.Client.Server/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.client.server/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.redis.client.server/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Redis.Client.Server.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Redis.Client.Server/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.client.server/build-and-test.yml?label=build%20and%20test&style=for-the-badge)](https://github.com/soenneker/soenneker.redis.client.server/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.client.server/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.redis.client.server/actions/workflows/codeql.yml)

# Soenneker.Redis.Client.Server

Provides cached StackExchange.Redis `IServer` instances backed by the shared `Soenneker.Redis.Client` multiplexer.

## Installation

```bash
dotnet add package Soenneker.Redis.Client.Server
```

## Registration and use

The default connection uses `Azure:Redis:ConnectionString`; see `Soenneker.Redis.Client` for the configuration shape.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Redis.Client.Server.Abstract;
using Soenneker.Redis.Client.Server.Registrars;
using StackExchange.Redis;

services.AddRedisServerClientAsSingleton();

IRedisServerClient serverClient = serviceProvider.GetRequiredService<IRedisServerClient>();
IServer server = await serverClient.Get(cancellationToken);

ServerCounters counters = await server.GetCountersAsync();
```

Use `Get(connectionString, cancellationToken)` to maintain a separate cached server for another Redis connection. The selected server is the first endpoint reported by that connection's multiplexer.

The scoped registration intentionally keeps `IRedisClient` singleton while making only the `IRedisServerClient` wrapper scoped. Disposing the scope releases the wrapper cache without destroying the shared multiplexer.

The backing Soenneker Redis client enables StackExchange.Redis administrative commands. Restrict the configured credentials and network access to the server operations the application is allowed to perform.
