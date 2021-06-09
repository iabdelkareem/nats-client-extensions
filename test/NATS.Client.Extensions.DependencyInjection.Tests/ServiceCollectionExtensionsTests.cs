using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NATS.Client.Extensions.DependencyInjection.Tests
{
    [Collection(nameof(NatsTestsCollectionFixture))]
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddNatsClient_ByDefault_ShouldRegisterConnectionsAsTransient()
        {
            var serviceProvider = new ServiceCollection()
                .AddNatsClient()
                .BuildServiceProvider();

            using var connection = serviceProvider.GetRequiredService<IConnection>();
            using var anotherConnection = serviceProvider.GetRequiredService<IConnection>();
            Assert.NotSame(connection, anotherConnection);

            using var encodedConnection = serviceProvider.GetRequiredService<IEncodedConnection>();
            using var anotherEncodedConnection = serviceProvider.GetRequiredService<IEncodedConnection>();
            Assert.NotSame(encodedConnection, anotherEncodedConnection);

            //Should always be singleton
            var natsClientConnectionFactory = serviceProvider.GetRequiredService<INatsClientConnectionFactory>();
            var anotherNatsClientConnectionFactory = serviceProvider.GetRequiredService<INatsClientConnectionFactory>();
            Assert.Same(natsClientConnectionFactory, anotherNatsClientConnectionFactory);
        }

        [Fact]
        public void AddNatsClient_WhenLifeTimeSetAsSingletonViaArguments_ShouldRegisterConnectionsAsSingleton()
        {
            var serviceProvider = new ServiceCollection()
                .AddNatsClient(connectionServiceLifeTime: ServiceLifetime.Singleton)
                .BuildServiceProvider();

            using var connection = serviceProvider.GetRequiredService<IConnection>();
            using var anotherConnection = serviceProvider.GetRequiredService<IConnection>();
            Assert.Same(connection, anotherConnection);

            using var encodedConnection = serviceProvider.GetRequiredService<IEncodedConnection>();
            using var anotherEncodedConnection = serviceProvider.GetRequiredService<IEncodedConnection>();
            Assert.Same(encodedConnection, anotherEncodedConnection);

            //Should always be singleton
            var natsClientConnectionFactory = serviceProvider.GetRequiredService<INatsClientConnectionFactory>();
            var anotherNatsClientConnectionFactory = serviceProvider.GetRequiredService<INatsClientConnectionFactory>();
            Assert.Same(natsClientConnectionFactory, anotherNatsClientConnectionFactory);
        }

        [Fact]
        public void AddNatsClient_WhenLifeTimeSetAsScopedViaArguments_ShouldRegisterConnectionsAsScoped()
        {
            var serviceProvider = new ServiceCollection()
                .AddNatsClient(connectionServiceLifeTime: ServiceLifetime.Scoped)
                .BuildServiceProvider();


            using var scope = serviceProvider.CreateScope();
            using var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
            using var anotherConnection = scope.ServiceProvider.GetRequiredService<IConnection>();
            Assert.Same(connection, anotherConnection);

            using var encodedConnection = scope.ServiceProvider.GetRequiredService<IEncodedConnection>();
            using var anotherEncodedConnection = scope.ServiceProvider.GetRequiredService<IEncodedConnection>();
            Assert.Same(encodedConnection, anotherEncodedConnection);

            using var anotherScope = serviceProvider.CreateScope();
            using var thirdConnection = anotherScope.ServiceProvider.GetRequiredService<IConnection>();
            Assert.NotSame(connection, thirdConnection);

            using var thirdEncodedConnection = anotherScope.ServiceProvider.GetRequiredService<IEncodedConnection>();
            Assert.NotSame(encodedConnection, thirdEncodedConnection);

            //Should always be singleton
            var natsClientConnectionFactory = scope.ServiceProvider.GetRequiredService<INatsClientConnectionFactory>();
            var anotherNatsClientConnectionFactory =
                anotherScope.ServiceProvider.GetRequiredService<INatsClientConnectionFactory>();
            Assert.Same(natsClientConnectionFactory, anotherNatsClientConnectionFactory);

        }

        [Fact]
        public void AddNatsClient_ShouldUseOptionsWhenConfigured()
        {
            const string name = "TestClient";
            var serviceProvider = new ServiceCollection()
                .AddNatsClient(cfg => { cfg.Name = name; })
                .BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<Options>();
            Assert.Equal(name, options.Name);

            var connection = serviceProvider.GetRequiredService<IConnection>();
            Assert.Equal(options.Name, connection.Opts.Name);

            var encodedConnection = serviceProvider.GetRequiredService<IEncodedConnection>();
            Assert.Equal(options.Name, encodedConnection.Opts.Name);
        }
    }
}