using EntityFX.IotSimulator.Common;
using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using NBomber;
using NBomber.CSharp;
using MQTTnet.Client;
using NBomber.Contracts;

class MqttScenarioBuilder
{
    private readonly ClientPool<(ITelemetryGenerator Generator, IMqttClient mqttClient)> _clientPool;
    private readonly SimulatorSettings? _settings;
    private readonly BuilderFactory _builderFactory;
    private readonly ILogger<MqttScenarioBuilder> _logger;

    public MqttScenarioBuilder(
        ILogger<MqttScenarioBuilder> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _clientPool = new ClientPool<(ITelemetryGenerator Generator, IMqttClient mqttClient)>();
        _settings = configuration.Get<SimulatorSettings>();
        _builderFactory = BuilderExtensions.WithDefault(logger, configuration, _settings); 
    }

    public ScenarioProps Build(string name)
    {
        var serializer = _builderFactory.GetSerializerBuilder().Build();

        var scenario = Scenario.Create(name, async context =>
        {
            var poolItem = _clientPool.GetClient(context.ScenarioInfo);

            var obj = poolItem.Generator.Value;

            //context.ScenarioInfo.Cu

            var serializeStep = await Step.Run("serialize", context, () =>
            {
                var serializedTelemetry = serializer.Serialize(obj).ToString();

                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(_settings!.TelemetrySender.Mqtt.Topic)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                    .WithPayload(serializedTelemetry)
                    .Build();

                return Task.FromResult(Response.Ok(payload: applicationMessage));
            });

            var sendStep = await Step.Run("publish", context, async () =>
            {
                var applicationMessage = serializeStep.Payload.Value;
                var length = applicationMessage!.Payload.Length;
                await poolItem.mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
                return Response.Ok(sizeBytes: length);
            });

            return Response.Ok();
        })
        .WithoutWarmUp()
        .WithInit(Init);

        return scenario;
    }

    private Task Init(IScenarioInitContext arg)
    {
        var settings = arg.CustomSettings.Get<MqttScenarioSettings>()
            ?? new MqttScenarioSettings("test", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, 
            _settings!.TelemetrySender.Mqtt.Server, _settings.TelemetrySender.Mqtt.Port ?? 1883, 50
            );

        return ScenarioHelper.BuildMqttClientPool(_clientPool,
        _builderFactory, settings);
    }
}
