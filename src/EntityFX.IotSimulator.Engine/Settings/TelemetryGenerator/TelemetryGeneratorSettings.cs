using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine
{
    public class TelemetryGeneratorSettings
    {
        public TelemetryGeneratorType Type { get; set; }

        public Dictionary<string, TelemetryPropertySetting> Properties { get; set; }

        public PluginSettings Plugin { get; set; }
    }
}