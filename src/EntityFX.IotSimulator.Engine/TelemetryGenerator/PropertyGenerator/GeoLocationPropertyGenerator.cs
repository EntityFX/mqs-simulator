using EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{

    public class GeoLocationPropertyGenerator : ComplexPropertyGenerator<object, GeoLocationType>
    {
        public override object Value
        {
            get
            {
                object result = result = PropertyGenerators.ToDictionary(p => p.Name,
                            p => p.Format != null ? string.Format(CultureInfo.InvariantCulture, p.Format, p.Value) : p.Value);

                return result;
            }
        }

        public int? RoundDecimals { get; set; }

        public EnumValues<GeoLocationValue> Enum { get; set; }

        public bool IsTwoWay { get; set; }


        public GeoLocationPropertyGenerator(string name, GeoLocationValue constant, Dictionary<string, object> variables)
         : base(name, GeoLocationType.Constant, new IPropertyGenerator[]
            {
                new NumberGeneratorBuilder()
                    .WithName("Lat")
                    .WithConstant(constant.Lat)
                    .WithVariables(variables)
                    .Build(),
                new NumberGeneratorBuilder()
                    .WithName("Lon")
                    .WithConstant(constant.Lon)
                    .WithVariables(variables)
                    .Build(),
                new NumberGeneratorBuilder()
                    .WithName("Alt")
                    .WithConstant(constant.Alt)
                    .WithVariables(variables)
                    .Build()
            }, variables)
        {
        }

        public GeoLocationPropertyGenerator(string name, EnumValues<decimal?> lat, EnumValues<decimal?> lon,
            EnumValues<decimal?> alt, Dictionary<string, object> variables, bool isTwoWay = false)
            : base(name, GeoLocationType.Enum, new IPropertyGenerator[]
            {
                new NumberGeneratorBuilder()
                    .WithName("Lat")
                    .WithEnumValue(lat)
                    .WithTwoWay(isTwoWay)
                    .WithVariables(variables)
                    .Build() ,
                new NumberGeneratorBuilder()
                    .WithName("Lon")
                    .WithEnumValue(lon)
                    .WithTwoWay(isTwoWay)
                    .WithVariables(variables)
                    .Build() ,
                new NumberGeneratorBuilder()
                    .WithName("Alt")
                    .WithEnumValue(alt)
                    .WithTwoWay(isTwoWay)
                    .WithVariables(variables)
                    .Build()
            }, variables)
        {
            IsTwoWay = true;
        }


        public GeoLocationPropertyGenerator(string name, GeoLocationType type, RandomRange latRange, RandomRange lonRange,
            RandomRange altRange, Dictionary<string, object> variables, int? roundDecimals = null)
            : base(name, type, new IPropertyGenerator[]
            {
                new NumberGeneratorBuilder()
                    .WithName("Lat")
                    .WithRandomRange(latRange != null
                        ? latRange
                        : new RandomRange(true) { From = -90.0m, To = 90.0m})
                    .WithRoundDecimals(roundDecimals)
                    .WithVariables(variables)
                    .Build(),
                new NumberGeneratorBuilder()
                    .WithName("Lon")
                    .WithRandomRange(lonRange != null
                        ? lonRange
                        : new RandomRange(true) { From = -180.0m, To = 180.0m})
                    .WithRoundDecimals(roundDecimals)
                    .WithVariables(variables)
                    .Build(),
                new NumberGeneratorBuilder()
                    .WithName("Alt")
                    .WithRandomRange(altRange != null
                        ? altRange
                        : new RandomRange(true) { From = -50m, To = 50.0m})
                    .WithRoundDecimals(roundDecimals)
                    .WithVariables(variables)
                    .Build()
            }, variables)
        {

        }
    }
}