using System.Numerics;

namespace Capstone
{
    public class InfraredRangeReading : RangeReading
    {
        public InfraredRangeReading(double value, Vector<double> sensorPosition, double angle) : base(value * 3.3, sensorPosition, angle) { }
    }
}
