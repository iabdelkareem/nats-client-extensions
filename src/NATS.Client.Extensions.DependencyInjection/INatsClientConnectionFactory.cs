using System;

namespace NATS.Client
{
    /// <summary>
    /// An abstract factory to create Nats client connections
    /// </summary>
    public interface INatsClientConnectionFactory
    {
        /// <summary>
        /// Create new connection
        /// </summary>
        /// <param name="configureOptions">Optionally, configure the options to be used when creating the connection.</param>
        /// <returns>New instance of <see cref="IConnection"/></returns>
        IConnection CreateConnection(Action<Options>? configureOptions = null);

        /// <summary>
        /// Create new connection
        /// </summary>
        /// <param name="options">The options to be used when creating the connection.</param>
        /// <returns>New instance of <see cref="IConnection"/></returns>
        IConnection CreateConnection(Options options);

        /// <summary>
        /// Create new encoded connection
        /// </summary>
        /// <param name="configureOptions">Optionally, configure the options to be used when creating the connection.</param>
        /// <returns>New instance of <see cref="IEncodedConnection"/></returns>
        IEncodedConnection CreateEncodedConnection(Action<Options>? configureOptions  = null);

        /// <summary>
        /// Create new encoded connection
        /// </summary>
        /// <param name="options">The options to be used when creating the connection.</param>
        /// <returns>New instance of <see cref="IEncodedConnection"/></returns>
        IEncodedConnection CreateEncodedConnection(Options options);
    }
}