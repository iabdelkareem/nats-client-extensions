using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace NATS.Client.Extensions.DependencyInjection.Tests
{
    [CollectionDefinition(nameof(NatsTestsCollectionFixture))]
    public class NatsTestsCollectionFixture : ICollectionFixture<NatsTestStartup>
    {

    }
}