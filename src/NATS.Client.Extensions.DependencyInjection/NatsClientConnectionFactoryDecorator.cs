using NATS.Client.JetStream;

namespace NATS.Client
{
    internal class NatsClientConnectionFactoryDecorator : INatsClientConnectionFactory
    {
        private readonly ConnectionFactory _connectionFactory;

        public NatsClientConnectionFactoryDecorator(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IJetStream CreateJetStreamContext(Options options)
        {
            var connection = _connectionFactory.CreateConnection(options);
            return connection.CreateJetStreamContext();
        }
    }
}