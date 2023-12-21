using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class ComplexPropertyGenerator : ComplexPropertyGenerator<object, Enum>
    {
        public ComplexPropertyGenerator(string name, IEnumerable<IPropertyGenerator> propertyGenerators, Dictionary<string, object> variables) 
            : base(name, default(Enum), propertyGenerators, variables)
        {
            PropertyGenerators = propertyGenerators;
        }

        public override object Value => PropertyGenerators.ToDictionary(p => p.Name, 
            p => p.Format != null ? string.Format(CultureInfo.InvariantCulture, p.Format, p.Value) : p.Value);
    }

    public abstract class ComplexPropertyGenerator<T, TTypeEnum> : PropertyGenerator<T, TTypeEnum>
        where TTypeEnum : Enum
    {
        public IEnumerable<IPropertyGenerator> PropertyGenerators { get; set; }

        public ComplexPropertyGenerator(string name, TTypeEnum type, IEnumerable<IPropertyGenerator> propertyGenerators, 
            Dictionary<string, object> variables) : base(name, type, variables)
        {
            PropertyGenerators = propertyGenerators;
        }

    }
}