using System;

namespace RoboticNavigation.Sensors.SensorReadings
{
    public class GyroscopeReading : SensorReading
    {
        public readonly double Radians;
        public GyroscopeReading(double angleInDegrees)
        {
            this.Radians = angleInDegrees * (Math.PI / 180d);
        }
    }
}
