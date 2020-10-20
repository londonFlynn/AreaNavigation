using System.Numerics;

namespace Capstone
{
    public class RangeReading : SensorReading
    {
        public Vector<double> SensorPosition;
        public readonly Vector<double> DistanceVector;
        public Vector<double> ReadingPosition
        {
            get
            {
                return SensorPosition + DistanceVector;
            }
        }
        public RangeReading(double distance)
        {
            this.DistanceVector = new Vector<double>(new double[] { distance, 0, 0, 0 });
        }
    }
}
