﻿namespace Capstone
{
    class UltrasonicRangeReading : RangeReading
    {
        public UltrasonicRangeReading(double value) : base(value * (16 / 26)) { }
    }
}
