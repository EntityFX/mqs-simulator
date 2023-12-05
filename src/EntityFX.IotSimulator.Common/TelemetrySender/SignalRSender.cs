using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Common.TelemetrySender
{
    public class SignalRSender : ITelemetrySender
    {
        private readonly ILogger logger;

        private static HubConnection connection;

        private readonly SignalRSettings signalRSettings;

        public SignalRSender(ILogger logger, SignalRSettings signalRSettings)
        {
            this.logger = logger;
            this.signalRSettings = signalRSettings;

            connection = new HubConnectionBuilder()
                .WithUrl(signalRSettings.Url)
                .Build();

            connection.Reconnecting += Connection_Reconnecting;
            connection.Closed += Connection_Closed;

            connection.StartAsync().Wait();
        }

        private Task Connection_Closed(System.Exception arg)
        {
            logger.LogInformation("Closed");
            return System.Threading.Tasks.Task.CompletedTask;
        }

        private Task Connection_Reconnecting(System.Exception arg)
        {
            logger.LogInformation("Reconnecting");
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public async Task SendAsync(object telemetry)
        {
            await connection.InvokeAsync(signalRSettings.Method,
                telemetry);
        }
    }
}
