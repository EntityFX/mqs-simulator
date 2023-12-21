using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{


    public class DateTimeGeneratorBuilder : GeneratorBuilderBase<DateType, DateTimeOffset?, DateTimeGenerator, DateTimeGeneratorBuilder>, IBuilder<DateTimeGenerator>
    {
        private bool? isRandom;
        private DateTimeOffsetSequence sequence;
        private EnumValues<DateTimeOffset> enumValues;
        private DateTimeOffsetRandomSequence randomRange;

        public DateTimeGeneratorBuilder WithSequence(DateTimeOffsetSequence sequence)
        {
            this.sequence = sequence;
            WithType(DateType.Sequece);

            return this;
        }

        public DateTimeGeneratorBuilder WithEnumValue(EnumValues<DateTimeOffset> enumValues, bool? isRandom = false)
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

        public DateTimeGeneratorBuilder WithRandomRange(DateTimeOffsetRandomSequence randomRange)
        {
            this.randomRange = randomRange;
            WithType(DateType.Random);

            return this;
        }


        protected override DateTimeGenerator BuildInstance()
        {
            if (constantValue != null)
            {
                WithType(DateType.Constant);
            }

            return new DateTimeGenerator(name, type, variables)
            {
                ConstantValue = constantValue,
                Sequence = sequence,
                Random = randomRange,
                Enum = enumValues
            };
        }
    }
}
