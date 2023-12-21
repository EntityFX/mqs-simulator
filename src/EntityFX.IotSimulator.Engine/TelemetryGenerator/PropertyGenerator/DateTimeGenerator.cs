using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{

    public class DateTimeGenerator : PropertyGenerator<DateTimeOffset?, DateType>
    {
        public DateTimeOffsetSequence Sequence { get; set; }

        public EnumValues<DateTimeOffset> Enum { get; set; }

        public DateTimeOffsetRandomSequence Random { get; set; }

        public DateTimeGenerator(string name, DateType type, Dictionary<string, object> variables) : base(name, type, variables)
        {
        }

        public override DateTimeOffset? Value
        {
            get
            {
                switch (Type)
                {
                    case DateType.Constant:
                        return ConstantValue;
                    case DateType.Sequece:
                        return Sequence.Value;
                    case DateType.Enum:
                        return Enum.Value;
                    case DateType.Random:
                        return Random.Value;
                    case DateType.Now:
                        return DateTimeOffset.Now;
                    case DateType.UtcNow:
                        return DateTimeOffset.UtcNow;

                    default:
                        break;
                }
                return DateTimeOffset.Now;
            }
        }

    }
}