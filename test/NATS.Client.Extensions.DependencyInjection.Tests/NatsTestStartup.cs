using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Xunit;

namespace NATS.Client.Extensions.DependencyInjection.Tests
{
    public class NatsTestStartup : IAsyncLifetime
    {
        private DockerClient _dockerClient;
        private string _containerId;

        public async Task InitializeAsync()
        {
            const string dockerImageName = "nats";
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();

            await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters()
            {
                FromImage = dockerImageName,
                Tag = "latest"
            }, null, new Progress<JSONMessage>());

            var createContainerResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                ExposedPorts = new Dictionary<string, EmptyStruct>() { { "4222", new EmptyStruct() } },
                Image = dockerImageName,
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>()
                    {
                        { "4222", new List<PortBinding>()
                        {
                            new PortBinding(){HostPort = "4222"}
                        }}
                    }
                }
            });

            _containerId = createContainerResponse.ID;
            var startSucceeded = await _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters());

            if (!startSucceeded)
                throw new InvalidOperationException("Failed to start NATS");
        }

        public async Task DisposeAsync()
        {
            if (!string.IsNullOrEmpty(_containerId))
            {
                await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters());
                await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters() { Force = true });
            }

            _dockerClient?.Dispose();
        }
    }
}