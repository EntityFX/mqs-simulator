using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{
    public class NumberGeneratorBuilder : GeneratorBuilderBase<NumberType, decimal?, NumberGenerator, NumberGeneratorBuilder>, IBuilder<NumberGenerator>
    {
        private bool? withNull;
        private NumberSequence sequence;
        private EnumValues<decimal?> enumValues;
        private RandomRange randomRange;
        private bool? isTwoWay;
        private bool? isRandom;
        private int? roundDecimals;

        public NumberGeneratorBuilder WithNull(bool withNull)
        {
            this.withNull = withNull;

            if (this.enumValues != null)
            {
                this.enumValues.WithNull = withNull;
            }

            return this;
        }

        public NumberGeneratorBuilder WithNumberSequence(NumberSequence sequence)
        {
            this.sequence = sequence;
            WithType(NumberType.Sequece);

            return this;
        }

        public NumberGeneratorBuilder WithEnumValue(EnumValues<decimal?> enumValues, bool? isRandom = false)
        {
            this.enumValues = enumValues;
            this.isRandom = isRandom;

            if (isRandom.HasValue)
            {
                this.enumValues.IsRandom = isRandom.Value;
            }

            WithType(NumberType.Enum);

            return this;
        }

        public NumberGeneratorBuilder WithRandomRange(RandomRange randomRange)
        {
            this.randomRange = randomRange;
            WithType(NumberType.Random);

            return this;
        }

        public NumberGeneratorBuilder WithTwoWay(bool isTwoWay)
        {
            this.isTwoWay = isTwoWay;
            WithType(NumberType.Sequece);

            if (this.enumValues != null)
            {
                this.enumValues.IsTwoWay = isTwoWay;
            }

            return this;
        }

        public NumberGeneratorBuilder WithRoundDecimals(int? roundDecimals)
        {
            this.roundDecimals = roundDecimals;

            return this;
        }

        protected override NumberGenerator BuildInstance()
        {
            if (constantValue != null)
            {
                WithType(NumberType.Constant);
            }

            return new NumberGenerator(name, type, variables)
            {
                RoundDecimals = roundDecimals,
                ConstantValue = constantValue,
                Sequence = sequence,
                Random = randomRange,
                Enum = enumValues
            };
        }
    }
}
