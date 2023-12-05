using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFX.IotPlatform.Simulator.TestPlugin
{
    public class CustomTelemetrySenderBuilder : BuilderBase<ITelemetrySender>, IBuilder<ITelemetrySender>
    {
        public CustomTelemetrySenderBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> settings) 
            : base(logger, configuration, settings)
        {
        }

        public class CustomTelemetrySender : ITelemetrySender
        {
            public Task SendAsync(object telemetry)
            {
                Console.WriteLine("Fake Send");
                return Task.FromResult(nameof(CustomTelemetrySender));
            }
        }

        public override ITelemetrySender Build()
        {
            return new CustomTelemetrySender();
        }

    }
}
