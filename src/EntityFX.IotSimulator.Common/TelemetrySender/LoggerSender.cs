﻿using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.TelemetrySender
{
    public class LoggerSender : ITelemetrySender
    {
        private readonly ILogger logger;

        public JsonSerializerOptions JsonSerializerOptions { get; }

        public LoggerSender(ILogger logger)
        {
            this.logger = logger;
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task SendAsync(object telemetry)
        {
            if (telemetry is string stringTelemetry)
            {
                logger.LogInformation(stringTelemetry);
            }
            else
            {
                logger.LogInformation(JsonSerializer.Serialize(telemetry, JsonSerializerOptions));
            }


            await Task.CompletedTask;
        }
    }
}
