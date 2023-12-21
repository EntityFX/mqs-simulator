using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{
    public class TimestampGeneratorBuilder : GeneratorBuilderBase<DateType, long?, TimestampGenerator, TimestampGeneratorBuilder>, IBuilder<TimestampGenerator>
    {
        private bool? isRandom;
        private NumberSequence sequence;
        private EnumValues<long> enumValues;
        private RandomRange randomRange;
        private bool isTwoWay;
        private bool withNull;

        public TimestampGeneratorBuilder WithSequence(NumberSequence sequence)
        {
            this.sequence = sequence;
            WithType(DateType.Sequece);

            return this;
        }

        public TimestampGeneratorBuilder WithRandomRange(RandomRange randomRange)
        {
            this.randomRange = randomRange;
            WithType(DateType.Random);

            return this;
        }

        public TimestampGeneratorBuilder WithEnumValue(EnumValues<long> enumValues, bool? isRandom = false)
        {
            this.enumValues = enumValues;
            this.isRandom = isRandom;

            if (isRandom.HasValue)
            {
                enumValues.IsRandom = isRandom.Value;
            }

            WithType(DateType.Enum);

            return this;
        }

        public TimestampGeneratorBuilder WithTwoWay(bool isTwoWay)
        {
            this.isTwoWay = isTwoWay;
            WithType(DateType.Sequece);

            if (this.enumValues != null)
            {
                this.enumValues.IsTwoWay = isTwoWay;
            }

            return this;
        }

        protected override TimestampGenerator BuildInstance()
        {
            if (constantValue != null)
            {
                WithType(DateType.Constant);
            }

            return new TimestampGenerator(name, type, variables)
            {
                ConstantValue = constantValue,
                Sequence = sequence,
                Random = randomRange,
                Enum = enumValues
            };
        }
    }
}
