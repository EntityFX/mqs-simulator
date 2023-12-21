using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Engine.TelemetrySender
{
    public interface ITelemetrySender
    {
        Task SendAsync(Dictionary<string, object> telemetry, object serialized);
    }
}
