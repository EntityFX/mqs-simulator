using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace EntityFX.IotSimulator.Common
{
    public class TelemetryGeneratorBuilder : BuilderBase<ITelemetryGenerator>, IBuilder<ITelemetryGenerator>
    {
        private readonly TelemetryGeneratorSettings settings;

        string UseDeviceId { get => extraSettings.ContainsKey("deviceId") ? extraSettings["deviceId"].ToString() : null; }
        string UseDeviceIdPropertyName { get;  }

        public TelemetryGeneratorBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings)
            : base(logger, configuration, extraSettings)
        {
            settings = configuration.GetSection("telemetryGenerator").Get<TelemetryGeneratorSettings>();
        }

        public override ITelemetryGenerator Build()
        {
            var propertyGeneratorBuilder = new PropertyGeneratorBuilder();
            var properties = settings.Properties.Select(propertyGeneratorBuilder.BuildPropertyGenerator).ToList();

            if (UseDeviceId != null)
            {
                properties.Add(new StringGenerator(UseDeviceIdPropertyName ?? "deviceId", UseDeviceId));
            }

            return new ComplexPropertyGenerator(string.Empty, properties);
        }

    }
}