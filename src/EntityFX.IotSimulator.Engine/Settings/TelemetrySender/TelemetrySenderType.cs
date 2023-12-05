namespace EntityFX.IotSimulator.Engine
{
    public enum TelemetrySenderType
    {
        Http, Mqtt, AzureSignalR, AzureIotHub, AzureIotCenter, SignalR,
        FileSystem, Logger
    }
}