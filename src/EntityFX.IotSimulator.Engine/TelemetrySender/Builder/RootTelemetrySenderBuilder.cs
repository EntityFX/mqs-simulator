using EntityFX.IotSimulator.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityFX.IotSimulator.Engine.TelemetrySender.Builder
{
    public class RootTelemetrySenderBuilder : Builder<ITelemetrySender, StubTelemetrySenderBuilder>,
        IBuilder<ITelemetrySender>
    {
        public RootTelemetrySenderBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> settings,
            string assemblyName, string senderTypeName = nameof(StubTelemetrySenderBuilder))
            : base(logger, configuration, settings, assemblyName, senderTypeName)
        {

        }

    }
}
