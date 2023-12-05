using System.Collections.Generic;
using System.Text;

namespace EntityFX.IotSimulator.Engine.TelemetrySerializer
{
    public interface ITelemetrySerializer
    {
        object Serialize(object telemetry);
    }
}
