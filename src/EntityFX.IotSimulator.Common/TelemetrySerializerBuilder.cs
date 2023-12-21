using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using EntityFX.IotSimulator.Common.TelemetrySerializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Runtime;
using EntityFX.IotSimulator.Engine.Settings.TelemetrySerializer;
using EntityFX.IotSimulator.Engine.Builder;

namespace EntityFX.IotSimulator.Common
{

    public class TelemetrySerializerBuilder : BuilderBase<ITelemetrySerializer>, IBuilder<ITelemetrySerializer>
    {
        private readonly TelemetrySerializerSettings settings;

        public TelemetrySerializerBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings)
            : base(logger, configuration, extraSettings)
        {
            settings = configuration.GetSection("telemetrySerializer").Get<TelemetrySerializerSettings>();
        }

        public override ITelemetrySerializer Build()
        {
            if (settings.Type == MessageType.Json)
            {
                return new JsonTelemetrySerializer(settings?.Indented ?? true,
                    settings?.CamelCase == true ? JsonNamingPolicy.CamelCase : null
                    );
            }

            if (settings.Type == MessageType.Bypass)
            {
                return new BypassSerializer();
            }

            return null;
        }

    }
}