using EntityFX.IotSimulator.Engine.TelemetrySender;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EntityFX.IotSimulator.Common.TelemetrySender
{
    public class FileSystemSender : ITelemetrySender
    {
        private readonly ILogger logger;

        public FileSystemSender(ILogger logger, string path)
        {
            this.logger = logger;
        }

        public async Task SendAsync(object telemetry)
        {
            File.WriteAllText(string.Format("{0:yyyyMMddTHHmmss}-{1}.json", DateTime.Now, Guid.NewGuid()), JsonSerializer.Serialize(telemetry));
            await Task.CompletedTask;
        }
    }
}
