using System;

namespace RoboticNavigation.Sensors.SensorReadings
{
    public class RotationSensorReading : SensorReading
    {
        public double angleInRadians;
        public readonly double AngleChange = 0;
        public RotationSensorReading(double angleInDegees)
        {
            this.angleInRadians = angleInDegees * (Math.PI / 180d);
        }
    }
}
