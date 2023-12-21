using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class RadiusGeoLocationGenerator : ComplexPropertyGenerator<object, Enum>
    {
        public const double R = 6371000.0; // metres

        private readonly int points;
        private readonly double radius;
        private readonly GeoLocationValue center;

        private int angle = 0;

        public RadiusGeoLocationGenerator(string name, int points, double radius, GeoLocationValue center, Dictionary<string, object> variables)
            : base(name, default(Enum), new IPropertyGenerator[] { }, variables)
        {
            this.points = points;
            this.radius = radius;
            this.center = center;
        }

        public override object Value => GetPoint();

        public object GetPoint()
        {
            var point = CalculateEndPoint(center, angle, radius);

            if (angle > 360)
            {
                angle = 0;
            }
            {
                angle += 360 / points;
            }

            return new {lat = point.Lat, lon = point.Lon, alt = point.Alt};
        }


        static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        static double ToAngle(double radians)
        {
            return radians * 180 / Math.PI;
        }

        static double СomputeDelta(double degrees)
        {
            return Math.Cos(ToRadians(degrees)) * R;
        }

        public static GeoLocationValue CalculateEndPoint(GeoLocationValue center, double angle, double distance)
        {
            var phi1 = ToRadians((double)center.Lat);
            var lambda1 = ToRadians((double)center.Lon);
            var deltaDistance = distance / R;
            var theta = ToRadians(angle);
            var sinPhi2 = Math.Sin(phi1) * Math.Cos(deltaDistance)
                          + Math.Cos(phi1) * Math.Sin(deltaDistance) * Math.Cos(theta);
            var phi2 = Math.Asin(sinPhi2);
            var x = Math.Cos(deltaDistance) - Math.Sin(phi1) * sinPhi2;
            var y = Math.Sin(theta) * Math.Sin(deltaDistance) * Math.Cos(phi1);
            var lambda2 = lambda1 + Math.Atan2(y, x);
            var lat2 = ToAngle(phi2);
            var lon2 = ToAngle(lambda2);

            return new GeoLocationValue()
            {
                Lat = (decimal)lat2,
                Lon = (decimal)lon2,
            };
        }
    }
}

