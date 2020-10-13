namespace Capstone
{
    public class GyroscopeReading : SensorReading
    {
        public readonly double RotationalAcceleration;
        public GyroscopeReading(double rotationalAcceleration)
        {
            this.RotationalAcceleration = rotationalAcceleration; ;
        }
    }
}
