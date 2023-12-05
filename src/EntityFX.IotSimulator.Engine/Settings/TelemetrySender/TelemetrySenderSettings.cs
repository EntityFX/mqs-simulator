namespace EntityFX.IotSimulator.Engine
{


    public class TelemetrySenderSettings
    {
        public TelemetrySenderType Type { get; set; }

        public AzureSignalRSetings AzureSignalR { get; set; }

        public SignalRSettings SignalR { get; set; }

        public AzureIotCenterSettings AzureIotCenter { get; set; }

        public AzureIotHubSettings AzureIotHub { get; set; }

        public HttpSettings Http { get; set; }

        public MqttSettings Mqtt { get; set; }

        public PluginSettings Plugin { get; set; }

    }
}