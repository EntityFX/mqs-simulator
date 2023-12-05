using System;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class DateTimeOffsetRandomSequence
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset? To { get; set; }

        private readonly Random _random = new Random((int)DateTimeOffset.Now.ToUnixTimeSeconds());

        public DateTimeOffset Value
        {
            get
            {
                long to;
                long from = From.ToUnixTimeSeconds();
                if (To == null)
                {
                    to = long.MaxValue;
                }
                else
                {
                    to = To.Value.ToUnixTimeSeconds();
                }

                byte[] buf = new byte[8];
                _random.NextBytes(buf);
                long longRand = BitConverter.ToInt64(buf, 0);

                var unixSeconds = (Math.Abs(longRand % (from - to)) + from);
                return DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
            }
        }
    }
}