using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.Settings.TelemetrySender
{

    public class HttpSettings
    {
        public Uri Path { get; set; }

        public HttpMethod Method { get; set; }

        public string ConnectionStringName { get; set; }

        public Dictionary<string, object> RequestHeaders { get; set; }

        public string QueryParameter { get; set; } = "telemetry";
    }
}