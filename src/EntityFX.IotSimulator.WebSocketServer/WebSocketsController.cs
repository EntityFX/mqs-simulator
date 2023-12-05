using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.WebSocketServer
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketsController : ControllerBase
    {
        private readonly ILogger<WebSocketsController> logger;
        private readonly SimulatorSettings simulatorSettings;
        private readonly IBuilderFactory builderFactory;
        private readonly ITelemetrySerializer telemetrySerializer;

        public WebSocketsController(ILogger<WebSocketsController> logger, SimulatorSettings simulatorSettings,
            IBuilderFactory builderFactory)
        {
            this.logger = logger;
            this.simulatorSettings = simulatorSettings;
            this.builderFactory = builderFactory;
        }


        [HttpGet("/ws")]
        public async Task GetNoRequest([FromQuery]string deviceId, [FromQuery]string token)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                if (token != "xyz")
                {
                    HttpContext.Response.StatusCode = 401;
                    return;
                }

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                logger.Log(LogLevel.Information, "WebSocket connection established");
                await NoRequestLongReply(webSocket, deviceId);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task NoRequestLongReply(WebSocket webSocket, string deviceId)
        {
            if (!string.IsNullOrEmpty(deviceId))
            {
                builderFactory.Settings["deviceId"] = deviceId;
            }

            var telemetryGenerator = builderFactory
                .GetGeneratorBuilder().Build();
            var telemetrySerializer = builderFactory
                .GetSerializerBuilder().Build();


            while (!webSocket.CloseStatus.HasValue)
            {

                var telemetry = telemetryGenerator.Value;
                var serializedTelemetry = telemetrySerializer.Serialize(telemetry);

                var serverMsg = Encoding.UTF8.GetBytes(serializedTelemetry.ToString());
                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                logger.Log(LogLevel.Information, "Message sent to Client");

                await Task.Delay(simulatorSettings.SendPeriod);
            }

            await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
            logger.Log(LogLevel.Information, "WebSocket connection closed");
        }
    }
}
