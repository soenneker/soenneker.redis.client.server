using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Redis.Client.Server.Abstract;
using Soenneker.Tests.HostedUnit;
using StackExchange.Redis;


namespace Soenneker.Redis.Client.Server.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class RedisServerClientTests : HostedUnitTest
{
    public RedisServerClientTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {
    }

    [Test]
    public async ValueTask Get_should_return_client()
    {
        var redisServerClient = Resolve<IRedisServerClient>();

        IServer client = await redisServerClient.Get();

        client.Should()
              .NotBeNull();
    }
}