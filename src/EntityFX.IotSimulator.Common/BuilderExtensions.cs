using EntityFX.IotSimulator.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Common
{
    public static class BuilderExtensions
    {
        public static BuilderFactory WithDefault(ILogger logger, IConfiguration configuration, SimulatorSettings settings)
        {
            var generatorAsmType = BuilderHelper.GetDefaultAssemblyAndTypeName<TelemetryGeneratorBuilder>(settings.TelemetryGenerator?.Plugin);
            var senderAsmType = BuilderHelper.GetDefaultAssemblyAndTypeName<TelemetrySenderBuilder>(settings.TelemetryGenerator?.Plugin);
            var serializerAsmType = BuilderHelper.GetDefaultAssemblyAndTypeName<TelemetrySerializerBuilder>(settings.TelemetryGenerator?.Plugin);


            var dictionarySettings = new Dictionary<string, object>()
            {
                ["settings"] = settings,
                ["generatorAsmType"] = generatorAsmType,
                ["serializerAsmType"] = serializerAsmType,
                ["senderAsmType"] = senderAsmType,
            };

            return new BuilderFactory(logger, configuration, dictionarySettings);
        }
    }
}