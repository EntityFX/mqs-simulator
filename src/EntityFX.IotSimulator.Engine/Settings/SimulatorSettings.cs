using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine
{
    public class SimulatorSettings
    {
        public TelemetrySenderSettings TelemetrySender { get; set; }
        public TelemetryGeneratorSettings TelemetryGenerator { get; set; }
        public TelemetrySerializerSettings TelemetrySerializer { get; set; }



        public TimeSpan SendPeriod { get; set; } = TimeSpan.FromSeconds(1);

    }
}