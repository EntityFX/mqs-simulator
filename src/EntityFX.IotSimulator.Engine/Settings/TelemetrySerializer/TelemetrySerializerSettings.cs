namespace EntityFX.IotSimulator.Engine
{
    public class TelemetrySerializerSettings
    {
        public MessageType Type { get; set; }

        public bool? Indented { get; set; } = true;


        public bool? CamelCase { get; set; } = true;

        public PluginSettings Plugin { get; set; }
    }
}