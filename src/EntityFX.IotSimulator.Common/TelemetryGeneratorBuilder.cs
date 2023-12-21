using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.Settings.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace EntityFX.IotSimulator.Common
{
    public class TelemetryGeneratorBuilder : BuilderBase<IValueGenerator>, ITelemetryGeneratorBuilder
    {
        private readonly TelemetryGeneratorSettings settings;
        private Dictionary<string, object> variables;

        string UseDeviceId { get => extraSettings.ContainsKey("deviceId") ? extraSettings["deviceId"].ToString() : null; }
        string UseDeviceIdPropertyName { get; }

        public TelemetryGeneratorBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings)
            : base(logger, configuration, extraSettings)
        {
            settings = configuration.GetSection("telemetryGenerator").Get<TelemetryGeneratorSettings>();
        }

        public override IValueGenerator Build()
        {

            var propertyGeneratorBuilder = new PropertyGeneratorBuilder();
            var properties = settings.Properties.Select(p => propertyGeneratorBuilder.BuildPropertyGenerator(p, variables)).ToList();

            if (UseDeviceId != null)
            {
                properties.Add(new StringGeneratorBuilder()
                    .WithName(UseDeviceIdPropertyName ?? "deviceId")
                    .WithConstant(UseDeviceId)
                    .WithVariables(variables)
                    .Build());
            }

            return new ComplexPropertyGenerator(string.Empty, properties, variables);
        }

        public ITelemetryGeneratorBuilder WithVariables(Dictionary<string, object> variables)
        {
            this.variables = variables;

            return this;
        }
    }
}