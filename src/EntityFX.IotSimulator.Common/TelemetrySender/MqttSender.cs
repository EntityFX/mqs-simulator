﻿using EntityFX.IotSimulator.Engine.Settings.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Common.TelemetrySender
{
    public class MqttSender : ITelemetrySender
    {
        private readonly ILogger logger;
        private IManagedMqttClient managedMqttClient;
        private readonly MqttSettings mqttSettings;

        public JsonSerializerOptions JsonSerializerOptions { get; }

        private readonly IMqttFactory mqttFactory;

        private const string TopicRegexString = @"[^{\}]+(?=})";
        private readonly Regex TopicRegex = new Regex(TopicRegexString);

        public MqttSender(ILogger logger, IMqttFactory mqttFactory, MqttSettings mqttSettings)
        {
            this.logger = logger;
            this.mqttFactory = mqttFactory;
            this.mqttSettings = mqttSettings;
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        private async Task<IManagedMqttClient> BuildMqttClientAndConnectAsync()
        {
            var managedMqttClient = mqttFactory.CreateManagedMqttClient();

            var mqttClientOptionsBuilder = new MqttClientOptionsBuilder();

            mqttClientOptionsBuilder.WithTcpServer(mqttSettings.Server ?? "localhost", mqttSettings?.Port)
                .WithCleanSession();

            if (mqttSettings.SslProtocol != null)
            {
                SslProtocols sslProtocol;
                if (Enum.TryParse<SslProtocols>(mqttSettings.SslProtocol, out sslProtocol))
                {
                    MqttClientOptionsBuilderTlsParameters tlsOptions = new MqttClientOptionsBuilderTlsParameters
                    {
                        SslProtocol = sslProtocol,
                        UseTls = true
                    };
                    X509Certificate2 rootCrt = new X509Certificate2("rootCA.crt");

                    tlsOptions.CertificateValidationHandler = (cert) =>
                    {
                        try
                        {
                            if (cert.SslPolicyErrors == SslPolicyErrors.None)
                            {
                                return true;
                            }

                            if (cert.SslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                            {
                                cert.Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                                cert.Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                                cert.Chain.ChainPolicy.ExtraStore.Add(rootCrt);

                                cert.Chain.Build((X509Certificate2)rootCrt);
                                var res = cert.Chain.ChainElements.Cast<X509ChainElement>().Any(a => a.Certificate.Thumbprint == rootCrt.Thumbprint);
                                return res;
                            }
                        }
                        catch { }

                        return false;
                    };
                    mqttClientOptionsBuilder.WithTls(tlsOptions);
                }

            }



            if (mqttSettings.UserName != null)
            {
                mqttClientOptionsBuilder.WithCredentials(mqttSettings.UserName, mqttSettings.Password);
            }

            var managedMqttClientOptions = (new ManagedMqttClientOptionsBuilder())
            .WithClientOptions(mqttClientOptionsBuilder.Build())
            .Build();

            managedMqttClient.UseConnectedHandler(e =>
            {
                logger.LogInformation($"{nameof(MqttSender)}: Connection Result: {e.ConnectResult.ResultCode}");
            });

            managedMqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                logger.LogDebug($"{nameof(MqttSender)}: Message from {e.ClientId}: {e.ApplicationMessage.Payload.Length} bytes.");
            });

            managedMqttClient.UseDisconnectedHandler(async e =>
            {
                logger.LogInformation($"{nameof(MqttSender)}: Reason={e.Reason}, DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
            });

            await managedMqttClient.StartAsync(managedMqttClientOptions);

            return managedMqttClient;
        }

        private bool CertificateValidationCallback(X509Certificate arg1, X509Chain arg2, SslPolicyErrors arg3, IMqttClientOptions arg4)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(Dictionary<string, object> telemetry, object serialized)
        {
            var topic = BuildTopic(telemetry, mqttSettings.Topic);

            var payload = Encoding.UTF8.GetBytes(serialized.ToString());
            var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(mqttSettings.QOS)
            .WithRetainFlag()
            .Build();

            if (managedMqttClient == null)
            {
                managedMqttClient = await BuildMqttClientAndConnectAsync();
            }

            await managedMqttClient.PublishAsync(mqttMessage, CancellationToken.None); // Since 3.0.5 with CancellationToken

            if (serialized is string stringTelemetry)
            {
                logger.LogInformation(stringTelemetry);
            }
            else
            {
                logger.LogInformation(JsonSerializer.Serialize(telemetry, JsonSerializerOptions));
            }


            await Task.CompletedTask;
        }

        private string BuildTopic(Dictionary<string, object> telemetry, string topic)
        {
            var matches = TopicRegex.Matches(topic);
            foreach (var match in matches.OfType<Match>())
            {
                if (!telemetry.ContainsKey(match.Value))
                {
                    continue;
                }
                topic = topic.Replace($"{{{match.Value}}}", telemetry[match.Value].ToString());
            }
            return topic;
        }
    }
}
