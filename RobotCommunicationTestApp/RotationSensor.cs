namespace Capstone
{
    public class RotationSensor : Sensor
    {
        public readonly bool LeftDrive;
        public readonly double DistancePerRotation;
        public double DistanceLastReading { get; private set; }
        public RotationSensor(bool IsLeftDrive, double distancePerRotation = 0)
        {
            this.LeftDrive = IsLeftDrive;
            this.DistancePerRotation = distancePerRotation;
        }
        public virtual void SetRecentReading(SensorReading reading)
        {
            var lastAngle = RecentReading is null ? 0 : ((RotationSensorReading)this.RecentReading).angleInRadians;
            this.RecentReading = reading;
            var newAngle = ((RotationSensorReading)this.RecentReading).angleInRadians;
            var angleDiference = newAngle - lastAngle;
            this.DistanceLastReading = (angleDiference) * DistancePerRotation;
            foreach (var subscriber in subscribers)
            {
                subscriber.ReciveSensorReading(this);
            }
        }
    }
}
