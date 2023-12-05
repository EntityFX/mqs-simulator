using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class DateTimeOffsetSequence
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset? To { get; set; }

        public TimeSpan Step { get; set; } = TimeSpan.FromMinutes(1);

        private DateTimeOffset _curent = DateTimeOffset.MinValue;

        public DateTimeOffset Value
        {
            get
            {
                if (_curent == DateTimeOffset.MinValue)
                {
                    _curent = From;
                }

                var result = _curent;

                _curent += Step;

                if (To != null && _curent >= To.Value)
                {
                    _curent = From;
                }

                return result;
            }
        }

    }
}