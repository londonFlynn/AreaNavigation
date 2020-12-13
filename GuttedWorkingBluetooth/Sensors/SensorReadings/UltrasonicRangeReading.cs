using RoboticNavigation.VectorMath;

namespace RoboticNavigation.Sensors.SensorReadings
{
    class UltrasonicRangeReading : RangeReading
    {
        public UltrasonicRangeReading(double value, Vector2d<double> sensorPosition, double angle, double angleAdjust, double sensorFalloff) : base(value, sensorPosition, angle, angleAdjust, sensorFalloff) { }
    }
}
