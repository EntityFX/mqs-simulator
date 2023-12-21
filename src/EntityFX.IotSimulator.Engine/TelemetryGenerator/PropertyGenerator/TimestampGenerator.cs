using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class TimestampGenerator : PropertyGenerator<long?, DateType>
    {
        public TimestampGenerator(string name, DateType type, Dictionary<string, object> variables) 
            : base(name, type, variables)
        {
        }

        public NumberSequence Sequence { get; set; }

        public EnumValues<long> Enum { get; set; }

        public RandomRange Random { get; set; }

        public override long? Value
        {
            get
            {
                switch (Type)
                {
                    case DateType.Constant:
                        return ConstantValue;
                    case DateType.Sequece:
                        return (long)Sequence.Value;
                    case DateType.Enum:
                        return Enum.Value;
                    case DateType.Random:
                        return (long)Random.Value;
                    case DateType.Now:
                        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    case DateType.UtcNow:
                        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    default:
                        break;
                }
                return DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        }

    }
}