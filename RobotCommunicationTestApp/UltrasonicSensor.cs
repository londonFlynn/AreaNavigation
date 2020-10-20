using System.Numerics;

namespace Capstone
{
    public class UltrasonicSensor : RangeSensor
    {
        public UltrasonicSensor(Vector<double> relPos) : base(relPos) { }
    }
}
