using EntityFX.IotSimulator.Engine.Settings;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySender.Builder;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using EntityFX.IotSimulator.Engine.TelemetrySerializer.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityFX.IotSimulator.Engine.Builder
{

    public class BuilderFactory : IBuilderFactory
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public BuilderFactory(ILogger logger, IConfiguration configuration, Dictionary<string, object> settings)
        {
            this.logger = logger;
            this.configuration = configuration;
            Settings = settings;
        }

        public Dictionary<string, object> Settings { get; }


        public TBuilderType GetBuilder<TBuilderType, TType>((string AssemblyName, string TypeName) builderAssemblyAndType)
            where TBuilderType : class, IBuilder<TType>
        {
            return Activator.CreateInstance(typeof(TBuilderType),
                new object[] { logger, configuration, Settings, builderAssemblyAndType.AssemblyName, builderAssemblyAndType.TypeName }) as TBuilderType;
        }

        public ITelemetryGeneratorBuilder GetGeneratorBuilder()
        {
            var generatorAsmType = ((string, string))Settings["generatorAsmType"];
            var builder = GetBuilder<RootTelemetryGeneratorBuilder, IValueGenerator>(generatorAsmType);
            return builder as ITelemetryGeneratorBuilder;
        }

        public IEnumerable<Simulator> BuildSimulators(InstanceSettings instanceSettings)
        {
            var count = instanceSettings?.Count ?? 1;

            return Enumerable.Range(1, count).Select(instance =>
            {
                var context = new Dictionary<string, object>();

                var simulatorId = instanceSettings.Name.Replace("{simulatorId}", instance.ToString());
                context["simulatorId"] = simulatorId;

                var generator = GetGeneratorBuilder()
                                .WithVariables(context)
                                .Build();

                var telemetrySender = GetSenderBuilder().Build();

                var serializer = GetSerializerBuilder().Build();

                return new Simulator(logger, context, generator, serializer, telemetrySender);
            }).ToArray();
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
