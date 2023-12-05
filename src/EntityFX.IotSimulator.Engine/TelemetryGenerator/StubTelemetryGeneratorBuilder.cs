using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator
{
    public class StubTelemetryGeneratorBuilder : BuilderBase<ITelemetryGenerator>, IBuilder<ITelemetryGenerator>
    {
        public StubTelemetryGeneratorBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> settings) 
            : base(logger, configuration, settings)
        {
        }

        public override ITelemetryGenerator Build()
        {
            return new StubTelemetryGenerator();
        }


        public class StubTelemetryGenerator : ITelemetryGenerator
        {
            public object Value
            {
                get
                {
                    Console.WriteLine("StubTelemetryGenerator Ge");
                    return nameof(StubTelemetryGenerator);
                }
            }
        }
    }
}