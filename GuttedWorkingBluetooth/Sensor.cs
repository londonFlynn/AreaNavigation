using System.Collections.Generic;

namespace Capstone
{
    public abstract class Sensor
    {
        protected SensorReading RecentReading;
        public Vector2d<double> RelativePosition { get; protected set; }
        public SensorReading GetCurrentReading()
        {
            return RecentReading;
        }
        public virtual void SetRecentReading(SensorReading reading)
        {
            if (ReadingChanged(reading))
            {
                this.RecentReading = reading;
                foreach (var subscriber in subscribers)
                {
                    subscriber.ReciveSensorReading(this);
                }
            }
        }
        protected abstract bool ReadingChanged(SensorReading changedReading);
        protected List<ISensorReadingSubsriber> subscribers = new List<ISensorReadingSubsriber>();
        public void SubsribeToNewReadings(ISensorReadingSubsriber subsriber)
        {
            subscribers.Add(subsriber);
        }
        public void UnsubscribeToNewReadings(ISensorReadingSubsriber subsriber)
        {
            subscribers.Remove(subsriber);
        }
    }
}
