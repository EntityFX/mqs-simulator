using EntityFX.IotSimulator.Engine.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System;
using System.Collections.Generic;

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

        internal RandomRange random;

        internal NumberSequence sequence;

        public bool WithNull
        {
            get => random.WithNull; private set
            {
                random.WithNull = value;
                sequence.WithNull = value;
            }
        }

        public BoolGenerator(string name, BoolType type, Dictionary<string, object> variables) 
            : base(name, type, variables)
        {

        }

    }
}