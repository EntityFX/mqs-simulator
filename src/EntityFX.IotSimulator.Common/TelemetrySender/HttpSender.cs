using EntityFX.IotSimulator.Engine.Settings.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Common.TelemetrySender
{
    public class HttpSender : ITelemetrySender
    {
        private readonly ILogger logger;
        private readonly HttpSettings httpSetings;

        private readonly Uri baseAddress;
        private readonly string path;
        private readonly HttpClient httpClient;

        public HttpSender(ILogger logger, HttpSettings httpSetings)
        {
            this.logger = logger;
            this.httpSetings = httpSetings;
            string requested = httpSetings.Path.Scheme + Uri.SchemeDelimiter + httpSetings.Path.Host + ":" + httpSetings.Path.Port;
            baseAddress = new Uri(requested);
            path = httpSetings.Path.PathAndQuery;

            httpClient = new HttpClient() { BaseAddress = baseAddress };

            foreach (var header in httpSetings.RequestHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
            }

        }

        public async Task SendAsync(Dictionary<string, object> telemetry, object serialized)
        {
            var content = new StringContent(serialized.ToString(), Encoding.UTF8, "application/json");

            if (httpSetings.Method == Engine.Settings.TelemetrySender.HttpMethod.Post)
            {
                var response = await httpClient.PostAsync(path, content);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogWarning($"Http respnse: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
                }
                return;
            }

            if (httpSetings.Method == Engine.Settings.TelemetrySender.HttpMethod.Put)
            {
                var response = await httpClient.PutAsync(path, content);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogWarning($"Http respnse: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}");
                }
                return;
            }
        }
    }
}
