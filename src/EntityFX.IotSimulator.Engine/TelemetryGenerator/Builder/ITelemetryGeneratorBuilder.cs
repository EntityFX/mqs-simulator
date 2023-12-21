using EntityFX.IotSimulator.Engine.Builder;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{
    public interface ITelemetryGeneratorBuilder : IBuilder<IValueGenerator>
    {
        ITelemetryGeneratorBuilder WithVariables(Dictionary<string, object> variables);
    }
}