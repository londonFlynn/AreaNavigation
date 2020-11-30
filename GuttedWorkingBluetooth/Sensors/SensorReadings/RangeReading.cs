using RoboticNavigation.VectorMath;
using System;

namespace RoboticNavigation.Sensors.SensorReadings
{
    public class RangeReading : SensorReading
    {
        public Vector2d<double> SensorPosition;
        public readonly Vector2d<double> DistanceVector;
        public readonly double Distance;
        public int TimesSampled = 0;
        public readonly double SensorFalloffDistance;
        public Vector2d<double> ReadingPosition
        {
            get
            {
                return SensorPosition + DistanceVector;
            }

        }
        public RangeReading(double distance, Vector2d<double> sensorPosition, double angle, double sensorFalloff)
        {
            this.SensorFalloffDistance = sensorFalloff;
            this.SensorPosition = sensorPosition;
            this.Distance = distance;
            var v = new Vector2d<double>(new double[] { 0, distance, 0, 0 });
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

        public static double ConfidenceFromConfidenceChange(double startingConfidence, double confidenceChange)
        {
            return ((1 - startingConfidence) * confidenceChange) + startingConfidence;
        }
        public double DistanceFromOtherReading(RangeReading that)
        {
            var thisPoint = this.ReadingPosition;
            var thatPoint = that.ReadingPosition;
            return Math.Sqrt(Math.Pow(thisPoint[0] - thatPoint[0], 2) + Math.Pow(thisPoint[1] - thatPoint[1], 2));
        }
    }
}
