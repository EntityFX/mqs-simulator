using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{
    public class StringGeneratorBuilder : GeneratorBuilderBase<StringType, string, StringGenerator, StringGeneratorBuilder>, IBuilder<StringGenerator>
    {
        private bool? withNull;
        private bool? isRandom;
        private EnumValues<string> enumValues;
        private string placeholder;

        public StringGeneratorBuilder WithNull(bool withNull)
        {
            this.withNull = withNull;

            if (this.enumValues != null)
            {
                this.enumValues.WithNull = withNull;
            }

            return this;
        }

        public StringGeneratorBuilder WithEnumValue(EnumValues<string> enumValues, bool? isRandom = false)
        {
            this.enumValues = enumValues;
            this.isRandom = isRandom;

            if (isRandom.HasValue)
            {
                this.enumValues.IsRandom = isRandom.Value;
            }

            WithType(StringType.Enum);

            return this;
        }

        public StringGeneratorBuilder WithPlaceholder(string placeholder)
        {
            this.placeholder = placeholder;
            WithType(StringType.Placeholder);

            return this;
        }

        protected override StringGenerator BuildInstance()
        {
            if (constantValue != null)
            {
                WithType(StringType.Constant);
            }

            return new StringGenerator(name, type, variables)
            {
                ConstantValue = constantValue,
                Enum = enumValues,
                Placeholder = placeholder
            };
        }
    }
}
