using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System.Collections.Generic;
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

        public NumberGenerator(string name, NumberType type, Dictionary<string, object> variables)
            : base(name, type, variables)
        {
            nullable = Enum;
        }

        public bool WithNull
        {
            get => nullable?.WithNull ?? false; set => nullable.WithNull = value;
        }

        public bool IsRandom { get => Enum.IsRandom; set => Enum.IsRandom = value; }

        public bool IsTwoWay { get => Enum.IsTwoWay; set => Enum.IsTwoWay = value; }
    }
}