using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class TimestampGenerator : PropertyGenerator<long?, DateType>
    {
        public TimestampGenerator(string name, DateType type) : base(name, type)
        {
        }

        public TimestampGenerator(string name, long? constant)
            : base(name, DateType.Constant)
        {
            ConstantValue = constant;
        }

        public TimestampGenerator(string name, NumberSequence sequence)
            : base(name, DateType.Sequece)
        {
            Sequence = sequence;
        }

        public TimestampGenerator(string name, EnumValues<long> @enum, bool isRandom = true)
            : base(name, DateType.Enum)
        {
            Enum = @enum;
            Enum.IsRandom = isRandom;
        }

        public TimestampGenerator(string name, RandomRange random)
            : base(name, DateType.Random)
        {
            Random = random;
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