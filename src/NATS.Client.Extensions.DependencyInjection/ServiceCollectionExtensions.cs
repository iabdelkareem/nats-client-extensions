using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NATS.Client.JetStream;

namespace NATS.Client
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add NatsClient <see cref="IConnection"/> and <see cref="IEncodedConnection"/> to <see cref="IServiceCollection"/>,
        /// Also adds <see cref="INatsClientConnectionFactory"/> abstract factory to ease creating new connection.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> instance</param>
        /// <param name="configureOptions">Configure the default options to be used when resolving NATS client connection via the DI. When null the default configurations will be used.</param>
        /// <param name="connectionServiceLifeTime">Configure the default lifetime for the connections resolved via the DI. Default is Transient.</param>
        /// <param name="streamNames"></param>
        /// <returns><see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/></returns>
        public static IServiceCollection AddNatsClient(this IServiceCollection services, Action<Options>? configureOptions = null, ServiceLifetime connectionServiceLifeTime = ServiceLifetime.Transient, params string[] streamNames)
        {
            var defaultOptions = ConnectionFactory.GetDefaultOptions();
            configureOptions?.Invoke(defaultOptions);
            services.AddSingleton(defaultOptions);

            services.AddSingleton<ConnectionFactory>();
            services.AddSingleton<INatsClientConnectionFactory, NatsClientConnectionFactoryDecorator>();

            services.TryAdd(new ServiceDescriptor(typeof(IJetStream), sp =>
            {
                var options = sp.GetRequiredService<Options>();
                var connectionFactory = sp.GetRequiredService<INatsClientConnectionFactory>();
                return connectionFactory.CreateJetStreamContext(options, streamNames);
            }, connectionServiceLifeTime));

            return services;
        }
    }
}
