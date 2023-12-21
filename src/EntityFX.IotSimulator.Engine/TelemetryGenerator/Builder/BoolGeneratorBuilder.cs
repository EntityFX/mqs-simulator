using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{


    public class BoolGeneratorBuilder : GeneratorBuilderBase<BoolType, bool?, BoolGenerator, BoolGeneratorBuilder>, IBuilder<BoolGenerator>
    {
        private bool withNull;
        private RandomRange random;

        private NumberSequence sequence;

        public BoolGeneratorBuilder WithNull(bool withNull)
        {
            this.withNull = withNull;

            return this;
        }

        public override BoolGeneratorBuilder WithDefault()
        {
            random = new RandomRange() { To = 2 };
            sequence = new NumberSequence() { To = 2 };

            return this;
        }

        protected override BoolGenerator BuildInstance()
        {
            if (withNull)
            {
                random.withNull = withNull;
            }

            if (constantValue.HasValue)
            {
                WithType(BoolType.Constant);
            }

            return new BoolGenerator(name, type, variables)
            {
                random = random,
                sequence = sequence,
                ConstantValue = constantValue
            };
        }
    }
}