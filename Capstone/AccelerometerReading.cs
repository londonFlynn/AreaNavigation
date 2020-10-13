namespace Capstone
{
    public class AccelerometerReading : SensorReading
    {
        public readonly double Acceleration;
        public AccelerometerReading(double acceleration)
        {
            this.Acceleration = acceleration;
        }
    }
}
