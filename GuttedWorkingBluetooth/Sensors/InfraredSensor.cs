using RoboticNavigation.VectorMath;

namespace RoboticNavigation.Sensors
{
    public class InfraredSensor : RangeSensor
    {
        public InfraredSensor(Vector2d<double> relPos, double sensorFalloff) : base(relPos, sensorFalloff) { }
    }
}
