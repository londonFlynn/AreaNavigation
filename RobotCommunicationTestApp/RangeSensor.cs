using System.Numerics;

namespace Capstone
{
    public abstract class RangeSensor : Sensor
    {
        public RangeSensor(Vector<double> relativePosition)
        {
            this.RelativePosition = relativePosition;
        }
    }
}
