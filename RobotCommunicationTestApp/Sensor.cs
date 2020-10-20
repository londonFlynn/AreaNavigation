using System.Collections.Generic;
using System.Numerics;

namespace Capstone
{
    public abstract class Sensor
    {
        protected SensorReading RecentReading;
        public Vector<double> RelativePosition { get; protected set; }
        public SensorReading GetCurrentReading()
        {
            return RecentReading;
        }
        public virtual void SetRecentReading(SensorReading reading)
        {
            this.RecentReading = reading;
            foreach (var subscriber in subscribers)
            {
                subscriber.ReciveSensorReading(this);
            }
        }
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
