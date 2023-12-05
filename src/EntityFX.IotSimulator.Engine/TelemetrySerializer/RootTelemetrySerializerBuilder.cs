using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetrySerializer
{
    public class RootTelemetrySerializerBuilder : Builder<ITelemetrySerializer, StubTelemetrySenderBuilder>, IBuilder<ITelemetrySerializer>
    {
        public RootTelemetrySerializerBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> settings,
            string assemblyName, string senderTypeName = nameof(StubTelemetrySenderBuilder))
            : base(logger, configuration, settings, assemblyName, senderTypeName)
        {

        }

    }
}
