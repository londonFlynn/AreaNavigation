namespace Capstone
{
    class UltrasonicRangeReading : RangeReading
    {
        public UltrasonicRangeReading(double value, Vector2d<double> sensorPosition, double angle, double sensorFalloff) : base(value, sensorPosition, angle, sensorFalloff) { }
    }
}
