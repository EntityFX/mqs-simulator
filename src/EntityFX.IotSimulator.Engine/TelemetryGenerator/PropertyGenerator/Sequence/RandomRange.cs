using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence
{

    public class RandomRange : INullable
    {

        public decimal From { get; set; }

        public decimal? To { get; set; }

        public bool IsDouble { get; set; } = false;

        public bool withNull;
        public bool WithNull
        {
            get; set;
        }

        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        public RandomRange(bool isDouble = false, bool withNull = false)
        {
            IsDouble = isDouble;
            WithNull = withNull;
        }

        public decimal? Value
        {
            get
            {
                var from = WithNull ? From - 1 : From;
                if (IsDouble)
                {
                    var tof = To == null ? long.MaxValue : (double)To.Value;

                    var rand = _random.NextDouble();
                    var valuef = Convert.ToDecimal((tof - (double)from) * rand + (double)from);

                    if (WithNull && valuef - from <= 0.001m)
                    {
                        return null;
                    }

                    return valuef;
                }
                var to = To == null ? long.MaxValue : (long)To.Value;

                var value = _random.NextLong((long)from, to);

                if (WithNull && value == from)
                {
                    return null;
                }

                return value;
            }
        }
    }
}