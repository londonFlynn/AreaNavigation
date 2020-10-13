namespace Capstone
{
    public class RotationSensorReading : SensorReading
    {
        public readonly double readingValue;
        public readonly double AngleChange = 0;
        public RotationSensorReading(double value)
        {
            this.readingValue = value;
        }
    }
}
