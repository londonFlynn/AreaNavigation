using RoboticNavigation.VectorMath;

namespace RoboticNavigation.Sensors
{
    public class UltrasonicSensor : RangeSensor
    {
        public UltrasonicSensor(Vector2d<double> relPos, double sensorFalloff) : base(relPos, sensorFalloff) { }
    }
}
