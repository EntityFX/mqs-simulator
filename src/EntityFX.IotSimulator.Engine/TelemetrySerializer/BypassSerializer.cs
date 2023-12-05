namespace EntityFX.IotSimulator.Engine.TelemetrySerializer
{
    public class BypassSerializer : ITelemetrySerializer
    {
        public object Serialize(object telemetry)
        {
            return telemetry;
        }
    }
}
