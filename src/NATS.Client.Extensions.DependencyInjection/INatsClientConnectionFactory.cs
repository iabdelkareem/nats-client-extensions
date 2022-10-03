using NATS.Client.JetStream;

namespace NATS.Client
{
    /// <summary>
    /// An abstract factory to create Nats client connections
    /// </summary>
    public interface INatsClientConnectionFactory
    {
        /// <summary>
        /// Create new JetStream context
        /// </summary>
        /// <param name="options"></param>
        /// <param name="streamNames"></param>
        /// <returns></returns>
        IJetStream CreateJetStreamContext(Options options, params string[] streamNames);
    }
}