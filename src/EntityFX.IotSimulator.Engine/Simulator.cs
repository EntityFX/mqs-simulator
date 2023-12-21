using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Engine
{
    public class Simulator
    {

        private readonly ILogger logger;

        public Simulator(ILogger logger, Dictionary<string, object> context, IValueGenerator generator,
            ITelemetrySerializer serializer, ITelemetrySender telemetrySender)
        {
            this.logger = logger;
            TelemetryGenerator = generator;
            TelemetrySerializer = serializer;
            TelemetrySender = telemetrySender;
            Context = context;
        }

        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();

        public IValueGenerator TelemetryGenerator { get; }

        public ITelemetrySerializer TelemetrySerializer { get; }

        public ITelemetrySender TelemetrySender { get; }

        public async Task  SimulateAsync()
        {
            var obj = TelemetryGenerator.Value as Dictionary<string, object>;

            var serializedTelemetry = TelemetrySerializer.Serialize(obj);

            try
            {
                await TelemetrySender.SendAsync(obj, serializedTelemetry);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Send error");
            }
        }
    }
}
