using EntityFX.IotSimulator.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{
    public class StubTelemetryGeneratorBuilder : BuilderBase<IValueGenerator>, IBuilder<IValueGenerator>
    {
        public StubTelemetryGeneratorBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> settings)
            : base(logger, configuration, settings)
        {
        }

        public override IValueGenerator Build()
        {
            return new StubTelemetryGenerator();
        }


        public class StubTelemetryGenerator : IValueGenerator
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