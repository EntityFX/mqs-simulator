using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using System.Text.Json;

namespace EntityFX.IotSimulator.Common.TelemetrySerializer
{
    public class JsonTelemetrySerializer : ITelemetrySerializer
    {
        private JsonSerializerOptions jsonPolicy;

        public JsonTelemetrySerializer(bool indented = true, JsonNamingPolicy camelCase = null)
        {
            jsonPolicy = new JsonSerializerOptions()
            {
                WriteIndented = indented,
                DictionaryKeyPolicy = camelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true
            };
        }

        public object Serialize(object telemetry)
        {
            return JsonSerializer.Serialize(telemetry, jsonPolicy);
        }
    }
}