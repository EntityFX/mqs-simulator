using System.Globalization;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class NumberGenerator : PropertyGenerator<decimal?, NumberType>
    {
        public override decimal? Value
        {
            get
            {
                decimal? result = null;
                switch (Type)
                {
                    case NumberType.Constant:
                        result = ConstantValue; break;
                    case NumberType.Sequece:
                        result = Sequence.Value; break;
                    case NumberType.Enum:
                        result = Enum.Value; break;
                    case NumberType.Random:
                        result = Random.Value; break;
                    default:
                        break;
                }

                if (RoundDecimals != null && RoundDecimals > 0 && result.HasValue)
                {
                    result = decimal.Round(result.Value, RoundDecimals.Value);
                }
                return result;
            }
        }

        public NumberSequence Sequence { get; set; }

        public EnumValues<decimal?> Enum { get; set; }

        public RandomRange Random { get; set; }

        public int? RoundDecimals { get; set; }

        private INullable nullable;

        public bool WithNull
        {
            get => nullable?.WithNull ?? false; set => nullable.WithNull = value;
        }

        public bool IsRandom { get => Enum.IsRandom; set => Enum.IsRandom = value; }

        public bool IsTwoWay { get => Enum.IsTwoWay; set => Enum.IsTwoWay = value; }



        public NumberGenerator(string name, NumberType numberType, bool withNull = false)
            : base(name, numberType)
        {
            WithNull = withNull;
        }

        public NumberGenerator(string name, decimal? constant)
            : base(name, NumberType.Constant)
        {
            ConstantValue = constant;
        }

        public NumberGenerator(string name, NumberSequence sequence, bool withNull = false)
            : base(name, NumberType.Sequece)
        {
            Sequence = sequence;
            nullable = Sequence;
            WithNull = withNull;
        }

        public NumberGenerator(string name, EnumValues<decimal?> @enum, bool isRandom = false,
            bool withNull = false,
            bool isTwoWay = false)
            : base(name, NumberType.Enum)
        {
            Enum = @enum;
            nullable = Enum;
            IsRandom = isRandom;
            WithNull = withNull;
            IsTwoWay = isTwoWay;
        }

        public NumberGenerator(string name, RandomRange random, bool withNull = false)
            : base(name, NumberType.Random)
        {
            Random = random;
            nullable = Random;
            WithNull = withNull;
        }
    }
}