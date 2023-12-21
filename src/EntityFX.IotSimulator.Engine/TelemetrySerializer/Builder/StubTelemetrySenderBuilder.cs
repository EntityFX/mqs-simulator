using EntityFX.IotSimulator.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Engine.TelemetrySerializer.Builder
{

    public class StubTelemetrySenderBuilder : BuilderBase<ITelemetrySerializer>, IBuilder<ITelemetrySerializer>
    {
        public StubTelemetrySenderBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> settings) 
            : base(logger, configuration, settings)
        {
        }

        public class StubTelemetrySerializer : ITelemetrySerializer
        {
            public object Serialize(object telemetry)
            {
                Console.WriteLine($"{nameof(StubTelemetrySerializer)} Serialize");
                return telemetry;
            }
        }

        public override ITelemetrySerializer Build()
        {
            return new StubTelemetrySerializer();
        }
    }
}
