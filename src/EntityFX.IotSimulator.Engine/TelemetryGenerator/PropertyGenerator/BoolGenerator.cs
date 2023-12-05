using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class BoolGenerator : PropertyGenerator<bool?, BoolType>
    {
        private bool _current = true;

        public override bool? Value
        {
            get
            {
                switch (Type)
                {
                    case BoolType.Constant:
                        return ConstantValue;
                    case BoolType.Sequece:
                        var sequenceValue = sequence.Value;
                        if (sequenceValue == null)
                        {
                            return null;
                        }
                        return Convert.ToBoolean(sequenceValue);
                    case BoolType.Random:
                        var random = this.random.Value;
                        if (random == null)
                        {
                            return null;
                        }
                        return Convert.ToBoolean(random);
                    default:
                        break;
                }
                return false;
            }
        }

        private readonly RandomRange random;

        private readonly NumberSequence sequence;

        public bool WithNull
        {
            get => random.WithNull; private set
            {
                random.WithNull = value;
                sequence.WithNull = value;
            }
        }

        public BoolGenerator(string name, BoolType type) : base(name, type)
        {
            random = new RandomRange() { To = 2 };
            sequence = new NumberSequence() { To = 2 };
        }

        public BoolGenerator(string name, bool? constant)
            : base(name, BoolType.Constant)
        {
            ConstantValue = constant;
        }

        public BoolGenerator(string name, BoolType type, bool withNull)
            : this(name, type)
        {
            random.WithNull = withNull;
        }

    }
}