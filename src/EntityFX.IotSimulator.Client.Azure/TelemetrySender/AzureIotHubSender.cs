using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Common.TelemetrySender
{
    public class AzureIotHubSender : ITelemetrySender
    {
        private readonly ILogger logger;
        private readonly AzureIotHubSettings azureIotHubSettings;

        private readonly ConcurrentDictionary<string, DeviceClient> deviceClients = new ConcurrentDictionary<string, DeviceClient>();
        private object _stdLock = new { };

        public AzureIotHubSender(ILogger logger, AzureIotHubSettings azureIotHubSettings)
        {
            this.logger = logger;
            this.azureIotHubSettings = azureIotHubSettings;
        }

        public async Task SendAsync(object telemetry)
        {
            foreach (var kv in azureIotHubSettings.SymmetricKeys)
            {
                await SendTelemetryAsync(kv.Key, telemetry);
            }
        }

        private async Task SendTelemetryAsync(string deviceId, object telemetryModel)
        {

            var telemetryJsonMessage = JsonSerializer.Serialize(telemetryModel,
                new JsonSerializerOptions() { IgnoreNullValues = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


            var iotClient = await GetDeviceClient(deviceId);
            if (iotClient == null)
            {  
                return;
            }

            logger.LogDebug($"Sending a telemetry message for DeviceId: {deviceId}, Message: {telemetryJsonMessage}");

            using (var message = new Message(Encoding.UTF8.GetBytes(telemetryJsonMessage)))
            {
                await iotClient.SendEventAsync(message);
            }
        }

        private async Task<DeviceClient> GetDeviceClient(string deviceId)
        {
            if (deviceClients.ContainsKey(deviceId))
            {
                return deviceClients[deviceId];
            }

            try
            {

                lock (_stdLock)
                {

                    IAuthenticationMethod auth = new DeviceAuthenticationWithRegistrySymmetricKey(
                        deviceId,
                        azureIotHubSettings.SymmetricKeys[deviceId]);

                    DeviceClient iotClient = DeviceClient.Create(azureIotHubSettings.HostName.OriginalString, auth, TransportType.Mqtt);

                    deviceClients.TryAdd(deviceId, iotClient);
                    return iotClient;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Device with Id {deviceId} not registered");
                return null;
            }
        }
    }
}
