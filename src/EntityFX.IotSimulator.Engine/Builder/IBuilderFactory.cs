using EntityFX.IotSimulator.Engine.Settings;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.Builder
{
    public interface IBuilderFactory
    {
        TBuilderType GetBuilder<TBuilderType, TType>((string AssemblyName, string TypeName) builderAssemblyAndType)
            where TBuilderType : class, IBuilder<TType>;

        ITelemetryGeneratorBuilder GetGeneratorBuilder();

        IBuilder<ITelemetrySerializer> GetSerializerBuilder();

        IBuilder<ITelemetrySender> GetSenderBuilder();

        IEnumerable<Simulator> BuildSimulators(InstanceSettings instanceSettings);

        Dictionary<string, object> Settings { get; }
    }
}
