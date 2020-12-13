using RoboticNavigation.VectorMath;

namespace RoboticNavigation.Sensors
{
    public class InfraredSensor : RangeSensor
    {
        public InfraredSensor(Vector2d<double> relPos, double sensorFalloff, double angleAdjustment) : base(relPos, sensorFalloff, angleAdjustment) { }
    }
}
