using EntityFX.IotSimulator.Common.TelemetrySender;
using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using EntityFX.IotSimulator.TelemetrySender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Common
{

    public class TelemetrySenderBuilder : BuilderBase<ITelemetrySender>, IBuilder<ITelemetrySender>
    {
        private readonly TelemetrySenderSettings settings;

        public TelemetrySenderBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings)
            : base(logger, configuration, extraSettings)
        {
            settings = configuration.GetSection("telemetrySender").Get<TelemetrySenderSettings>();
        }

        public override ITelemetrySender Build()
        {
            if (settings.Type == TelemetrySenderType.Logger)
            {
                return new LoggerSender(logger);
            }

            if (settings.Type == TelemetrySenderType.SignalR)
            {
                return new SignalRSender(logger, settings.SignalR);
            }

            if (settings.Type == TelemetrySenderType.Http)
            {
                return new HttpSender(logger, settings.Http);
            }

            // if (settings.Type == TelemetrySenderType.AzureIotCenter)
            // {
            //     return new AzureIotCenterSender(logger, settings.AzureIotCenter);
            // }

            // if (settings.Type == TelemetrySenderType.AzureSignalR)
            // {
            //     var connectionString = configuration.GetConnectionString(settings.AzureSignalR.ConnectionStringName);
            //     return new AzureSignalRSender(logger, settings.AzureSignalR, connectionString);
            // }

            // if (settings.Type == TelemetrySenderType.AzureIotHub)
            // {
            //     return new AzureIotHubSender(logger, settings.AzureIotHub);
            // }            
            
            if (settings.Type == TelemetrySenderType.Mqtt)
            {
                return new MqttSender(logger, new MqttFactory(), settings.Mqtt);
            }

            if (settings.Type == TelemetrySenderType.FileSystem)
            {
                return new FileSystemSender(logger, "");
            }

            return null;
        }

    }
}