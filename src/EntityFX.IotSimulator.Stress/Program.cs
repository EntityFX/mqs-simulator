using EntityFX.IotSimulator.Common;
using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client.Options;
using MQTTnet;
using NATS.Client;
using NBomber;
using NBomber.Contracts.Internal.Cluster;
using NBomber.CSharp;
using System.Reflection.Emit;
using MQTTnet.Client;

static IConfiguration InitConfiguration(IHost host, string[] args)
{
    var rootConfig = host.Services.GetRequiredService<IConfiguration>();
    var profile = rootConfig.GetSection("profile");


    var configBuilder = new ConfigurationBuilder()
        .AddJsonFile(profile.Value);


    var config = configBuilder.Build();

    return config;
}

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddCommandLine(args);

builder.Logging.ClearProviders();

var host = builder
    .Build();

var configuration = InitConfiguration(host, args);
var settings = configuration.Get<SimulatorSettings>();

var clientPool = new ClientPool<(ITelemetryGenerator Generator, IMqttClient mqttClient)>();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

var builderFactory = BuilderExtensions.WithDefault(logger, configuration, settings);

var serializer = builderFactory.GetSerializerBuilder().Build();

var scenario = Scenario.Create("generate_serialize_send", async context =>
{
    var poolItem = clientPool.GetClient(context.ScenarioInfo);

    var obj = poolItem.Generator.Value;

    var serializeStep = await Step.Run("serializeStep", context, () =>
    {
        var serializedTelemetry = serializer.Serialize(obj).ToString();
        var length = serializedTelemetry!.Length;

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(settings!.TelemetrySender.Mqtt.Topic)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
            .WithPayload(serializedTelemetry)
            .Build();

        return Task.FromResult(Response.Ok(payload: applicationMessage, sizeBytes: length));
    });

    var sendStep = await Step.Run("send", context, async () =>
    {
        var applicationMessage = serializeStep.Payload.Value;
        await poolItem.mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        return Response.Ok();
    });

    return Response.Ok();
})
.WithoutWarmUp()
.WithInit(async context =>
{
    var mqttFactory = new MqttFactory();

    for (int i = 0; i < 15; i++)
    {
        var mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(settings!.TelemetrySender.Mqtt.Server, settings!.TelemetrySender.Mqtt.Port)
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var generator = builderFactory.GetGeneratorBuilder().Build();
        clientPool.AddClient((generator, mqttClient));
    }

    await Task.CompletedTask;
})
.WithLoadSimulations(
    // ramp up from 0 to 50 copies    
    // duration: 30 seconds (it executes from [00:00:00] to [00:00:30])
    Simulation.RampingConstant(copies: 20, during: TimeSpan.FromSeconds(30)),

    // it keeps 50 copies running
    // duration: 30 seconds (it executes from [00:00:30] to [00:01:00])
    Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(30)),

    // ramp down from 50 to 0 copies
    // duration: 30 seconds (it executes from [00:01:00] to [00:01:30])
    Simulation.RampingConstant(copies: 0, during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();