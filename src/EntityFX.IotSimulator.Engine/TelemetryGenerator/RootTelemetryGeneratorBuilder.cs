using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator
{
    public class RootTelemetryGeneratorBuilder : Builder<ITelemetryGenerator, StubTelemetryGeneratorBuilder>, IBuilder<ITelemetryGenerator>
    {
        public RootTelemetryGeneratorBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings,
            string assemblyName, string senderTypeName = nameof(StubTelemetryGeneratorBuilder))
            : base(logger, configuration, extraSettings, assemblyName, senderTypeName)
        {

        }


    }
}