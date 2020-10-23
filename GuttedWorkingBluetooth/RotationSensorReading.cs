using System;

namespace Capstone
{
    public class RotationSensorReading : SensorReading
    {
        public readonly double angleInRadians;
        public readonly double AngleChange = 0;
        public RotationSensorReading(double angleInDegees)
        {
            this.angleInRadians = angleInDegees * (Math.PI / 180d);
        }
    }
}
