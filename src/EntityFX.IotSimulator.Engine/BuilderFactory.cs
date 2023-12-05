using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine
{
    public class BuilderFactory : IBuilderFactory
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public BuilderFactory(ILogger logger, IConfiguration configuration, Dictionary<string, object> settings)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.Settings = settings;
        }

        public Dictionary<string, object> Settings { get; }


        public TBuilderType GetBuilder<TBuilderType, TType>((string AssemblyName, string TypeName) builderAssemblyAndType)
            where TBuilderType : class, IBuilder<TType>
        {
            return Activator.CreateInstance(typeof(TBuilderType),
                new object[] { logger, configuration, Settings, builderAssemblyAndType.AssemblyName, builderAssemblyAndType.TypeName }) as TBuilderType;
        }

        public IBuilder<ITelemetryGenerator> GetGeneratorBuilder()
        {
            var generatorAsmType = ((string, string))Settings["generatorAsmType"];
            return GetBuilder<RootTelemetryGeneratorBuilder, ITelemetryGenerator>(generatorAsmType);
        }

        public IBuilder<ITelemetrySender> GetSenderBuilder()
        {
            var generatorAsmType = ((string, string))Settings["senderAsmType"];
            return GetBuilder<RootTelemetrySenderBuilder, ITelemetrySender>(generatorAsmType);
        }

        public IBuilder<ITelemetrySerializer> GetSerializerBuilder()
        {
            var serializerAsmType = ((string, string))Settings["serializerAsmType"];
            return GetBuilder<RootTelemetrySerializerBuilder, ITelemetrySerializer>(serializerAsmType);
        }
    }
}
