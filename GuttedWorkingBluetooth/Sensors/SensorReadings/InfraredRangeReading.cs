using RoboticNavigation.VectorMath;

namespace RoboticNavigation.Sensors.SensorReadings
{
    public class InfraredRangeReading : RangeReading
    {
        public InfraredRangeReading(double value, Vector2d<double> sensorPosition, double angle, double angleAdjust, double sensorFalloff) : base(value, sensorPosition, angle, angleAdjust, sensorFalloff) { }
    }
}
