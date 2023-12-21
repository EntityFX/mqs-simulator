using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class StringGenerator : PropertyGenerator<string, StringType>
    {
        public EnumValues<string> Enum { get; set; }

        public string Placeholder { get; set; }

        public bool WithNull { get => Enum.WithNull; set => Enum.WithNull = value; }

        public bool IsRandom { get => Enum.IsRandom; set => Enum.IsRandom = value; }

        public StringGenerator(string name, StringType type, Dictionary<string, object> variables) 
            : base(name, type, variables)
        {
        }

        public override string Value
        {
            get
            {
                switch (Type)
                {
                    case StringType.Constant:
                        return ConstantValue;
                    case StringType.Enum:
                        return Enum.Value;
                    case StringType.Guid:
                        return Guid.NewGuid().ToString();
                    case StringType.Placeholder:
                        return Replace();
                    default:
                        break;
                }
                return "";
            }
        }

        private string Replace()
        {
            var result = Placeholder;
            foreach (var variable in Variables)
            {
                result = result.Replace($"{{{variable.Key}}}", variable.Value.ToString());
            }
            return result;
        }
    }
}