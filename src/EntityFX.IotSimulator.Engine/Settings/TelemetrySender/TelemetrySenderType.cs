namespace EntityFX.IotSimulator.Engine.Settings.TelemetrySender
{
    public enum TelemetrySenderType
    {
        Http, Mqtt, AzureSignalR, AzureIotHub, AzureIotCenter, SignalR,
        FileSystem, Logger
    }
}