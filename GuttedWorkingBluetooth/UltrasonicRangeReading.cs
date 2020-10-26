namespace Capstone
{
    class UltrasonicRangeReading : RangeReading
    {
        public UltrasonicRangeReading(double value, Vector2d<double> sensorPosition, double angle) : base(value, sensorPosition, angle) { }
    }
}
