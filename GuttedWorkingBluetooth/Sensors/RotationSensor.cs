using RoboticNavigation.Sensors.SensorReadings;

namespace RoboticNavigation.Sensors
{
    public class RotationSensor : Sensor
    {
        public readonly bool LeftDrive;
        public readonly double DistancePerRotation;
        public double ReadingValueOffset;
        public double DistanceLastReading { get; private set; }
        public RotationSensor(bool IsLeftDrive, double readingValueOffset = 0, double distancePerRotation = 0)
        {
            this.LeftDrive = IsLeftDrive;
            this.DistancePerRotation = distancePerRotation;
            this.ReadingValueOffset = readingValueOffset;
        }
        protected override bool ReadingChanged(SensorReading changedReading)
        {
            if (this.RecentReading is null)
                return true;
            var newReading = changedReading as RotationSensorReading;
            var oldReading = this.RecentReading as RotationSensorReading;
            return newReading.angleInRadians != oldReading.angleInRadians;
        }
        public override void SetRecentReading(SensorReading reading)
        {
            if (ReadingChanged(reading))
            {
                ((RotationSensorReading)reading).angleInRadians -= ReadingValueOffset;
                var lastAngle = RecentReading is null ? 0 : ((RotationSensorReading)this.RecentReading).angleInRadians;
                var newAngle = ((RotationSensorReading)reading).angleInRadians;
                var angleDiference = newAngle - lastAngle;
                this.DistanceLastReading = (angleDiference) * DistancePerRotation;
                base.SetRecentReading(reading);
            }
        }
    }
}
