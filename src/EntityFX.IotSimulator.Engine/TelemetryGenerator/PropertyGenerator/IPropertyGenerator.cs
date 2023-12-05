using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public interface IPropertyGenerator<T, TTypeEnum> : IPropertyGenerator
        where TTypeEnum : Enum
    {
        TTypeEnum Type { get; set; }
    }

    public interface IPropertyGenerator : ITelemetryGenerator
    {
        string Name { get; set; }
        string Format { get; set; }

        int PassTicks { get; set; } 
    }
}