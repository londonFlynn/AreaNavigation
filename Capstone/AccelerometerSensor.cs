using System;

namespace Capstone
{
    public class AccelerometerSensor : Sensor
    {
        public double Position { get; private set; }
        public double CurrentSpeed { get; protected set; }
        public override void SetRecentReading(SensorReading reading)
        {
            DateTime lastTime = RecentReading.DateTime;
            this.RecentReading = reading;
            double interval = (RecentReading.DateTime - lastTime).TotalMilliseconds;
            CurrentSpeed = interval * ((AccelerometerReading)RecentReading).Acceleration;
            Position = CurrentSpeed * interval;
            foreach (var subscriber in subscribers)
            {
                subscriber.ReciveSensorReading(this);
            }
        }
    }
}
