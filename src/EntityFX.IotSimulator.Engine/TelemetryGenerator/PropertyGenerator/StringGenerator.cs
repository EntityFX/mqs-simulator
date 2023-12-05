using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class StringGenerator : PropertyGenerator<string, StringType>
    {
        public EnumValues<string> Enum { get; set; }

        public bool WithNull { get => Enum.WithNull; set => Enum.WithNull = value; }

        public bool IsRandom { get => Enum.IsRandom; set => Enum.IsRandom = value; }

        public StringGenerator(string name, StringType type) : base(name, type)
        {
        }

        public StringGenerator(string name, string constant)
            : base(name, StringType.Constant)
        {
            ConstantValue = constant;
        }

        public StringGenerator(string name, EnumValues<string> @enum, bool isRandom = true, bool withNull = false)
            : base(name, StringType.Enum)
        {
            Enum = @enum;
            IsRandom = isRandom;
            WithNull = withNull;
        }

        public override string Value
        {
            get
            {
                switch (Type)
                {
                    case StringType.Constant:
                        return ConstantValue;
                    case StringType.Enum:
                        return Enum.Value;
                    case StringType.Guid:
                        return Guid.NewGuid().ToString();
                    default:
                        break;
                }
                return "";
            }
        }
    }
}