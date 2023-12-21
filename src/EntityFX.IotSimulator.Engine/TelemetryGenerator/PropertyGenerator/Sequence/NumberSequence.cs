namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence
{


    public class NumberSequence : INullable
    {
        public decimal From { get => WithNull ? fromWithNull : from; set { from = value; fromWithNull = value - 1; } }
        public decimal? To { get; set; }

        public decimal Step { get; set; } = 1;

        private decimal _curent = decimal.MinValue;

        public bool IsTwoWay { get; set; }

        public bool WithNull { get; set; }

        private decimal from;

        private decimal fromWithNull;

        private bool direction = true;

        public decimal? Value
        {
            get
            {
                if (_curent == decimal.MinValue)
                {
                    _curent = From;
                }

                var result = _curent;

                _curent = direction ? _curent + Step : _curent - Step;

                if (!IsTwoWay)
                {
                    if (To != null && _curent >= To.Value)
                    {
                        _curent = From;
                    }
                }
                else
                {
                    if (To != null && _curent >= To.Value)
                    {
                        direction = !direction;
                    }

                    if (_curent <= From)
                    {
                        direction = !direction;
                    }
                }


                if (WithNull && result == From)
                {
                    return null;
                }

                return result;
            }
        }

        public NumberSequence(bool withNull = false)
        {
            WithNull = withNull;
        }

    }
}