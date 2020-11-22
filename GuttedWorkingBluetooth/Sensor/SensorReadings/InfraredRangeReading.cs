namespace Capstone
{
    public class InfraredRangeReading : RangeReading
    {
        public InfraredRangeReading(double value, Vector2d<double> sensorPosition, double angle, double sensorFalloff) : base(value, sensorPosition, angle, sensorFalloff) { }
    }
}
