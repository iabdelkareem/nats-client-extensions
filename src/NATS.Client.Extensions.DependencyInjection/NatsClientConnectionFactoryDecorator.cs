using System;

namespace NATS.Client
{
    internal class NatsClientConnectionFactoryDecorator : INatsClientConnectionFactory
    {
        private readonly ConnectionFactory _connectionFactory;

        public NatsClientConnectionFactoryDecorator(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IConnection CreateConnection(Action<Options>? configureOptions = null)
        {
            var options = ConnectionFactory.GetDefaultOptions();
            configureOptions?.Invoke(options);
            return CreateConnection(options);
        }

        public IConnection CreateConnection(Options options)
        {
            return _connectionFactory.CreateConnection(options);
        }

        public IEncodedConnection CreateEncodedConnection(Action<Options>? configureOptions = null)
        {
            var options = ConnectionFactory.GetDefaultOptions();
            configureOptions?.Invoke(options);
            return CreateEncodedConnection(options);
        }

        public IEncodedConnection CreateEncodedConnection(Options options)
        {
            return _connectionFactory.CreateEncodedConnection(options);
        }
    }
}