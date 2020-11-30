using System;

namespace RoboticNavigation.Sensors.SensorReadings
{
    public abstract class SensorReading
    {
        public readonly DateTime DateTime = DateTime.Now;
    }
}
