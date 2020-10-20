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
        public RangeReading(double distance, Vector<double> sensorPosition, double angle)
        {
            this.SensorPosition = sensorPosition;
            var v = new Vector<double>(new double[] { 0, distance, 0, 0 });
            this.DistanceVector = v.Rotate(angle);
            //this.DistanceVector = new Vector<double>(new double[] { 0, distance, 0, 0 });
        }
        public override bool Equals(object obj)
        {
            if (obj is RangeReading)
            {
                RangeReading that = obj as RangeReading;
                return that.SensorPosition[0] == this.SensorPosition[0]
                    && that.SensorPosition[1] == this.SensorPosition[1]
                    && that.ReadingPosition[0] == this.ReadingPosition[0]
                    && that.ReadingPosition[1] == this.ReadingPosition[1];
            }
            else
            {
                return base.Equals(obj);
            }
        }
    }
}
