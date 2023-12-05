using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine
{
    public class TelemetryPropertySetting
    {
        public TelemetryPropertyType Type { get; set; }

        public object Constant { get; set; }

        public GeoLocationValue GeoConstant { get; set; }

        public TelemetryPropertySequenceSetting Sequence { get; set; }

        public TelemetryPropertySequenceSetting LatRandomSequence { get; set; }

        public TelemetryPropertySequenceSetting LonRandomSequence { get; set; }

        public TelemetryPropertySequenceSetting AltRandomSequence { get; set; }

        public string[] Enum { get; set; }

        public GeoLocationValue[] GeoEnum { get; set; }

        public bool? Guid { get; set; }

        public bool? Random { get; set; }

        public bool? UseNull { get; set; }

        public bool? IsTwoWay { get; set; }

        public string Format { get; set; }
        public int? RoundDecimals { get; set; }
        public int? Pass { get; set; }

        public string TemplatePath { get; set; }

        private string _dateType;
        public string DateType
        {
            get => _dateType; set
            {
                if (value != "now" && value != "utcNow")
                {
                    _dateType = "now";
                }
                else
                {
                    _dateType = value;
                }
            }
        }

        public Dictionary<string, TelemetryPropertySetting> Properties { get; set; }

        public double? Radius { get; set; }
        public int? Points { get; set; }
    }
}