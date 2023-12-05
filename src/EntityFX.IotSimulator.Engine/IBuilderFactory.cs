using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine
{
    public interface IBuilderFactory
    {
        TBuilderType GetBuilder<TBuilderType, TType>((string AssemblyName, string TypeName) builderAssemblyAndType)
            where TBuilderType : class, IBuilder<TType>;

        IBuilder<ITelemetryGenerator> GetGeneratorBuilder();

        IBuilder<ITelemetrySerializer> GetSerializerBuilder();

        IBuilder<ITelemetrySender> GetSenderBuilder();

        Dictionary<string, object> Settings { get;  }
    }
}
