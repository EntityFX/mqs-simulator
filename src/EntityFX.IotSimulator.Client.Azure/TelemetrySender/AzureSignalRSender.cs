using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Common.TelemetrySender
{

    public class AzureSignalRSender : ITelemetrySender
    {
        private readonly ILogger logger;
        private readonly IServiceHubContext hubContext;

        private readonly AzureSignalRSetings azureSignalRSetings;

        public AzureSignalRSender(ILogger logger, AzureSignalRSetings azureSignalRSetings, string connectionString)
        {
            this.logger = logger;
            this.azureSignalRSetings = azureSignalRSetings;

            var sm = new ServiceManagerBuilder()
            .WithOptions(option =>
            {
                option.ConnectionString = connectionString;
                option.ServiceTransportType = ServiceTransportType.Persistent;
            })
            .Build();
            hubContext = sm.CreateHubContextAsync(azureSignalRSetings.Hub).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task SendAsync(object telemetry)
        {
            var telemetryJson = JsonSerializer.Serialize(telemetry);

            await hubContext.Clients.All.SendCoreAsync("Send", new object[] { telemetryJson });
        }
    }
}
