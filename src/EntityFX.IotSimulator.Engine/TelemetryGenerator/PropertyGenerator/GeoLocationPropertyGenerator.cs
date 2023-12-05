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


        public GeoLocationPropertyGenerator(string name, GeoLocationValue constant)
         : base(name, GeoLocationType.Constant, new IPropertyGenerator[]
            {
                new NumberGenerator("Lat", constant.Lat) ,
                new NumberGenerator("Lon", constant.Lon),
                new NumberGenerator("Alt", constant.Alt),
            })
        {
        }
        
        public GeoLocationPropertyGenerator(string name, EnumValues<decimal?> lat, EnumValues<decimal?> lon, EnumValues<decimal?> alt, bool isTwoWay = false)
            : base(name, GeoLocationType.Enum, new IPropertyGenerator[]
            {
                        new NumberGenerator("Lat", lat, false, false, isTwoWay) ,
                        new NumberGenerator("Lon", lon, false, false, isTwoWay),
                        new NumberGenerator("Alt", alt, false, false, isTwoWay)
            })
        {
            IsTwoWay = true;
        }


        public GeoLocationPropertyGenerator(string name, GeoLocationType type, RandomRange latRange, RandomRange lonRange, RandomRange altRange, int? roundDecimals = null)
            : base(name, type, new IPropertyGenerator[]
            {
                new NumberGenerator("Lat", latRange != null
                    ? latRange
                    : new RandomRange(true) { From = -90.0m, To = 90.0m}) { RoundDecimals = roundDecimals },
                new NumberGenerator("Lon", lonRange != null
                    ? lonRange
                    : new RandomRange(true) { From = -180.0m, To = 180.0m}) { RoundDecimals = roundDecimals },
                new NumberGenerator("Alt", altRange != null
                    ? altRange
                    : new RandomRange(true) { From = -50m, To = 50.0m}) { RoundDecimals = roundDecimals },
            })
        {

        }
    }
}