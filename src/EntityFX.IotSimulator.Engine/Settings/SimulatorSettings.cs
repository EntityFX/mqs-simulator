using EntityFX.IotSimulator.Engine.Settings.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.Settings.TelemetrySender;
using EntityFX.IotSimulator.Engine.Settings.TelemetrySerializer;
using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.Settings
{
    public class SimulatorSettings
    {
        public InstanceSettings Instance { get; set; }

        public TelemetrySenderSettings TelemetrySender { get; set; }
        public TelemetryGeneratorSettings TelemetryGenerator { get; set; }
        public TelemetrySerializerSettings TelemetrySerializer { get; set; }



        public TimeSpan SendPeriod { get; set; } = TimeSpan.FromSeconds(1);

    }
}