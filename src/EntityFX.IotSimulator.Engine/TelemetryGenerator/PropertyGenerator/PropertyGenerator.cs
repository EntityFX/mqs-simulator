using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public abstract class PropertyGenerator<T, TTypeEnum> : IPropertyGenerator<T, TTypeEnum> where TTypeEnum : Enum
    {
        public string Name { get; set; }

        private T lastValue;
        private int passedTicks;

        public abstract T Value { get; }


        public string Format { get; set; }

        object IValueGenerator.Value
        {
            get
            {
                passedTicks++;
                if (passedTicks == PassTicks)
                {
                    lastValue = this.Value;
                    passedTicks = 0;
                }
                if (lastValue != null && lastValue.Equals(default(T)))
                {
                    lastValue = this.Value;
                }
                return lastValue;
            }
        }


        public T ConstantValue { get; set; }

        public TTypeEnum Type { get; set; }
        public Dictionary<string, object> Variables { get; }

        public int PassTicks { get; set; } = 1;


        public PropertyGenerator(string name, TTypeEnum type, Dictionary<string, object> variables)
        {
            Name = name;
            Type = type;
            Variables = variables;
        }
    }
}