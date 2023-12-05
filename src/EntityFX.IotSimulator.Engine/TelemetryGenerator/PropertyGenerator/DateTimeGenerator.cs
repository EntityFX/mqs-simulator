using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class DateTimeGenerator : PropertyGenerator<DateTimeOffset?, DateType>
    {
        public DateTimeGenerator(string name, DateType type) : base(name, type)
        {
        }

        public DateTimeGenerator(string name, DateTimeOffset? constant)
            : base(name, DateType.Constant)
        {
            ConstantValue = constant;
        }

        public DateTimeGenerator(string name, DateTimeOffsetSequence sequence)
            : base(name, DateType.Sequece)
        {
            Sequence = sequence;
        }

        public DateTimeGenerator(string name, EnumValues<DateTimeOffset> @enum, bool isRandom = true)
            : base(name, DateType.Enum)
        {
            Enum = @enum;
            Enum.IsRandom = isRandom;
        }

        public DateTimeGenerator(string name, DateTimeOffsetRandomSequence random)
            : base(name, DateType.Random)
        {
            Random = random;
        }

        public DateTimeOffsetSequence Sequence { get; set; }

        public EnumValues<DateTimeOffset> Enum { get; set; }

        public DateTimeOffsetRandomSequence Random { get; set; }

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