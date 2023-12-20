using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using MQTTnet.Client.Options;
using MQTTnet;
using NBomber;
using MQTTnet.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive;

static class ScenarioHelper
{
    public static IConfiguration InitConfiguration(IHost host, string[] args)
    {
        var rootConfig = host.Services.GetRequiredService<IConfiguration>();
        var profile = rootConfig.GetSection("profile");


        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile(profile.Value);


        var config = configBuilder.Build();

        return config;
    }

    internal static async Task BuildMqttClientPool(ClientPool<(ITelemetryGenerator Generator, IMqttClient mqttClient)> clientPool, BuilderFactory builderFactory, MqttScenarioSettings settings)
    {
        var mqttFactory = new MqttFactory();

        for (int i = 0; i < settings.ClientsCount; i++)
        {
            var mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(settings.Server, settings.Port)
                .Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var generator = builderFactory.GetGeneratorBuilder().Build();
            clientPool.AddClient((generator, mqttClient));
        }
    }
}