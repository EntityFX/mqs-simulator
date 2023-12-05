using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Engine.TelemetrySender
{
    public class StubTelemetrySenderBuilder : BuilderBase<ITelemetrySender>, IBuilder<ITelemetrySender>
    {
        public StubTelemetrySenderBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings)
            : base(logger, configuration, extraSettings)
        {
        }

        public class StubTelemetrySender : ITelemetrySender
        {
            public Task SendAsync(object telemetry)
            {
                Console.WriteLine("StubTelemetrySender Send");
                return Task.FromResult(nameof(StubTelemetrySender));
            }
        }

        public override ITelemetrySender Build()
        {
            return new StubTelemetrySender();
        }

    }
}
