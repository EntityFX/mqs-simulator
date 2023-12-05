using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.TelemetrySender
{
    public class AzureIotCenterSender : ITelemetrySender
    {

        private readonly ILogger _logger;
        private readonly AzureIotCenterSettings _settings;

        private readonly ConcurrentDictionary<string, DeviceClient> deviceClients = new ConcurrentDictionary<string, DeviceClient>();

        private static object _stdLock = new { };

        public AzureIotCenterSender(ILogger logger, AzureIotCenterSettings settings)
        {
            _logger = logger;
            _settings = settings;
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

            _logger.LogDebug($"Sending a telemetry message for DeviceId: {deviceId}, Message: {telemetryJsonMessage}");

            using (var message = new Message(Encoding.UTF8.GetBytes(telemetryJsonMessage)))
            {
                await iotClient.SendEventAsync(message);
            }
        }

        public async Task SendAsync(object telemetry)
        {
            foreach (var kv in _settings.SymmetricKeys)
            {
                await SendTelemetryAsync(kv.Key, telemetry);
            }
        }

        private async Task<DeviceClient> GetDeviceClient(string deviceId)
        {
            if (deviceClients.ContainsKey(deviceId))
            {
                return deviceClients[deviceId];
            }

            var security = new SecurityProviderSymmetricKey(
                deviceId,
                _settings.SymmetricKeys?.ContainsKey(deviceId) == true ? _settings.SymmetricKeys[deviceId] : String.Empty,
                null);

            var transportHandler = new ProvisioningTransportHandlerHttp();

            ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(
                _settings.ProvisioningHost, _settings.IdScope, security, transportHandler);

            DeviceRegistrationResult result = await provClient.RegisterAsync();

            if (result.Status != ProvisioningRegistrationStatusType.Assigned)
            {
                _logger.LogWarning($"ProvisioningClient AssignedHub: {result.AssignedHub}; DeviceId: {result.DeviceId}");
                return null;
            }

            try
            {

                lock (_stdLock)
                {


                    IAuthenticationMethod auth = new DeviceAuthenticationWithRegistrySymmetricKey(
                        result.DeviceId,
                        security.GetPrimaryKey());

                    _logger.LogDebug($"Testing the provisioned device with IoT Hub...");
                    DeviceClient iotClient = DeviceClient.Create(result.AssignedHub, auth, TransportType.Mqtt);
                    deviceClients.TryAdd(deviceId, iotClient);
                    return iotClient;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Device with Id {deviceId} not registered");
                return null;
            }
        }
    }
}
