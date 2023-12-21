using EntityFX.IotSimulator.Engine.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{
    public class RootTelemetryGeneratorBuilder : Builder<IValueGenerator, StubTelemetryGeneratorBuilder>, ITelemetryGeneratorBuilder
    {
        private readonly string assemblyName;
        private readonly string senderTypeName;
        private Dictionary<string, object> variables;

        public RootTelemetryGeneratorBuilder(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings,
            string assemblyName, string senderTypeName = nameof(StubTelemetryGeneratorBuilder))
            : base(logger, configuration, extraSettings, assemblyName, senderTypeName)
        {
            this.assemblyName = assemblyName;
            this.senderTypeName = senderTypeName;
        }

        protected override IBuilder<IValueGenerator> GetBuilder()
        {
            var builder = this.GetBuilder<ITelemetryGeneratorBuilder, IValueGenerator, StubTelemetryGeneratorBuilder>(
                assemblyName, senderTypeName, logger, configuration, extraSettings);
            builder.WithVariables(variables);
            return builder;
        }

        public ITelemetryGeneratorBuilder WithVariables(Dictionary<string, object> variables)
        {
            this.variables = variables;
            return this;
        }
    }
}