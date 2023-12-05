using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public abstract class PropertyGenerator<T, TTypeEnum> : IPropertyGenerator<T, TTypeEnum> where TTypeEnum : Enum
    {
        public string Name { get; set; }

        private T lastValue;
        private int passedTicks;

        public abstract T Value { get; }


        public string Format { get; set; }

        object ITelemetryGenerator.Value
        {
            get
            {
                passedTicks++;
                if (passedTicks == PassTicks)
                {
                    lastValue = this.Value;
                    passedTicks = 0;
                }
                if (lastValue.Equals(default(T)))
                {
                    lastValue = this.Value;
                }
                return lastValue;
            }
        }


        public T ConstantValue { get; set; }

        public TTypeEnum Type { get; set; }

        public int PassTicks { get; set; } = 1;


        public PropertyGenerator(string name, TTypeEnum type)
        {
            Name = name;
            Type = type;
        }
    }
}