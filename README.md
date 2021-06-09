# NATS.Client.Extensions.DependencyInjection

[![Build Status](https://dev.azure.com/ibrahimabdelkareem0432/OpenSource/_apis/build/status/NATS.Client.Extensions?branchName=main)](https://dev.azure.com/ibrahimabdelkareem0432/OpenSource/_build/latest?definitionId=3&branchName=main) [![NuGet](https://badgen.net/nuget/v/NATS.Client.Extensions.DependencyInjection)](https://www.nuget.org/packages/NATS.Client.Extensions.DependencyInjection/)

**NATS.Client.Extensions.DependencyInjection** enables dependency injection for [**NATS.Client**](https://github.com/nats-io/nats.net) connection types (i.e., IConnection, IEncodedConnection) via Microsoft DependencyInjection, And introduces INatsClientConnectionFactory abstraction, And provides an easy way for configuring NATS.Client.Options.


## Installation
Install [NATS.Client.Extensions.DependencyInjection](https://www.nuget.org/packages/NATS.Client.Extensions.DependencyInjection/) NuGet package

All of the package features will be available via `NATS.Client` namespace to ease working with the types without importing namespaces other than the `NATS.Client`'s namespace


## Usage

The code below demonstrates how to inject NATS Client services in `IServiceCollection`

```csharp
//using NATS.Client

public void ConfigureServices(IServiceCollection services)
{
    /*Inject NATS client connections as Transient services using the default NATS.Client.Options,
      And inject INatsClientConnectionFactory (NOTE: Always Singleton)*/
    services.AddNatsClient();

    //Change the services' registry lifetime via the connectionServiceLifeTime parameter (default is Transient)
    services.AddNatsClient(connectionServiceLifeTime: ServiceLifetime.Singleton);
            
    //Configure NATS.Client.Options via configureOptions parameter (default is NATS.Client.ConnectionFactory.GetDefaultOptions())
    services.AddNatsClient(configureOptions: options =>
    {
        options.Servers = new[]
        {
            "nats://localhost:4222",
            "nats://localhost:4223"
        };

        options.MaxReconnect = 2;
        options.ReconnectWait = 1000;
    });
}
```
Then you can use it as follows.

```csharp
/*Assuming BackgroundServiceA was registered in IServiceCollection & resolved via IServiceProvider,
      IConnection will be injected via the constructor */
    public class BackgroundServiceA
    {
        private readonly IConnection _connection;

        public BackgroundServiceA(IConnection connection)
        {
            _connection = connection;
        }
    }

    /*Assuming BackgroundServiceC was registered in IServiceCollection & resolved via IServiceProvider,
      IEncodedConnection will be injected via the constructor */
    public class BackgroundServiceB
    {
        private readonly IEncodedConnection _encodedConnection;

        public BackgroundServiceB(IEncodedConnection encodedConnection)
        {
            _encodedConnection = encodedConnection;
        }
    }

    /*Assuming BackgroundServiceC is registered in IServiceCollection & resolved via IServiceProvider,
      INatsClientConnectionFactory will be injected via the constructor */
    public class BackgroundServiceC
    {
        private readonly INatsClientConnectionFactory _connectionFactory;

        public BackgroundServiceC(INatsClientConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task Execute()
        {
            /* You can use INatsClientConnectionFactory if you want more control of the creation & disposal of connections outside of the DI's scope.
               The factory will create a new instance of either IConnection or IEncoded connection everytime */

            //This will create IConnection using the default options (i.e., NATS.Client.ConnectionFactory.GetDefaultOptions())
            _connectionFactory.CreateConnection();

            //You can configure the options via a delegate function
            _connectionFactory.CreateConnection(options =>
            {
                options.Servers = new[] {"nats://localhost:4223"};
            });

            //Or you can pass NATS.Client.Options as an argument
            var options = ConnectionFactory.GetDefaultOptions();
            options.Servers = new[] {"nats://localhost:4223"};
            _connectionFactory.CreateConnection(options);

            //Same options available for creating IEncodedConnection
            _connectionFactory.CreateEncodedConnection();
            _connectionFactory.CreateEncodedConnection(options =>
            {
                options.Servers = new[] {"nats://localhost:4223"};
            });
            _connectionFactory.CreateEncodedConnection(options);
            
            return Task.CompletedTask;
        }
    }
```
