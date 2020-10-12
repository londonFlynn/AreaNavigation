using System;
using System.Numerics;

namespace Capstone
{
    public class RangeReading : SensorReading
    {
        public Vector<double> SensorPosition;
        public Vector<double> DistanceVector = new Vector<double>(new double[2]);
        public Vector<double> ReadingPosition
        {
            get
            {
                return SensorPosition + DistanceVector;
            }
        }
        public DateTime DateTime = DateTime.Now;
    }
}
