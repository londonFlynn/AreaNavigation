namespace Capstone
{
    public class InfraredRangeReading : RangeReading
    {
        public InfraredRangeReading(double value, Vector2d<double> sensorPosition, double angle) : base(value, sensorPosition, angle) { }
    }
}
