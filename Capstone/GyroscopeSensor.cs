using System;

namespace Capstone
{
    public class GyroscopeSensor : Sensor
    {
        public double Angle { get; private set; }
        public double CurrentRotationalSpeed { get; protected set; }
        public override void SetRecentReading(SensorReading reading)
        {
            DateTime lastTime = RecentReading.DateTime;
            this.RecentReading = reading;
            double interval = (RecentReading.DateTime - lastTime).TotalMilliseconds;
            CurrentRotationalSpeed = interval * ((AccelerometerReading)RecentReading).Acceleration;
            Angle = CurrentRotationalSpeed * interval;
            foreach (var subscriber in subscribers)
            {
                subscriber.ReciveSensorReading(this);
            }
        }
    }
}
