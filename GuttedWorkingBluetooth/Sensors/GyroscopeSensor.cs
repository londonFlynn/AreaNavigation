using RoboticNavigation.Sensors.SensorReadings;

namespace RoboticNavigation.Sensors
{
    public class GyroscopeSensor : Sensor
    {
        protected override bool ReadingChanged(SensorReading changedReading)
        {
            if (this.RecentReading is null)
                return true;
            var newReading = changedReading as GyroscopeReading;
            var oldReading = this.RecentReading as GyroscopeReading;
            return newReading.Radians != oldReading.Radians;
        }
    }
}
