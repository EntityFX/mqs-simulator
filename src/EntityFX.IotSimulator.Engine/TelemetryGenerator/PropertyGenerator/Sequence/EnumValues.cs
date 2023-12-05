using System;
using System.Diagnostics;
using System.Threading;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class EnumValues<T> : INullable, IRandom
    {
        public T[] Values { get; set; }

        public bool IsRandom { get; set; }

        public bool IsTwoWay { get; set; }

        public bool WithNull { get; set; }

        private int StartIndex { get => WithNull ? -1 : 0; }

        private readonly Random _random = new Random();

        private int _index = 0;

        private bool direction = true;

        public EnumValues(T[] values, bool isRandom = true, bool withNull = false, bool isTwoWay = false)
        {
            Values = values;
            IsRandom = isRandom;
            WithNull = withNull;
            IsTwoWay = isTwoWay;
            _index = StartIndex;
        }

        public T Value
        {
            get
            {
                int current;
                if (IsRandom)
                {
                    _index = _random.Next(WithNull ? -1 : 0, Values.Length);
                    current = _index;
                }
                else
                {
                    current = _index;

                    _index = direction ? _index + 1 : _index - 1;

                    if (!IsTwoWay && _index > Values.Length)
                    {
                        _index = StartIndex;
                        current = _index;
                    }

                    if (IsTwoWay)
                    {
                        if(_index >= Values.Length)
                        {
                            direction = !direction;
                            _index = _index - 2;
                        }

                        if (_index < StartIndex)
                        {
                            direction = !direction;
                            _index = StartIndex + 1;
                        }
                    }
                }

                if (current == -1)
                {
                    _index = 0;
                    return default(T);
                }
                return Values[current];
            }
        }
    }
}