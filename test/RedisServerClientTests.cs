using FluentAssertions;
using Soenneker.Redis.Client.Server.Abstract;
using Soenneker.Tests.FixturedUnit;
using StackExchange.Redis;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Redis.Client.Server.Tests;

[Collection("Collection")]
public class RedisServerClientTests : FixturedUnitTest
{
    public RedisServerClientTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public async void Get_should_return_client()
    {
        var redisServerClient = Resolve<IRedisServerClient>();

        IServer client = await redisServerClient.Get();

        client.Should().NotBeNull();
    }
}