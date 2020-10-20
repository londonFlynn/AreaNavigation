using System.Numerics;

namespace Capstone
{
    class UltrasonicRangeReading : RangeReading
    {
        public UltrasonicRangeReading(double value, Vector<double> sensorPosition, double angle) : base(value, sensorPosition, angle) { }
    }
}
