using System.Numerics;

namespace Capstone
{
    public abstract class RangeSensor : Sensor
    {
        public RangeSensor(Vector<double> relativePosition)
        {
            this.RelativePosition = relativePosition;
        }
        protected override bool ReadingChanged(SensorReading changedReading)
        {
            return true;
        }
        public virtual void SetRecentReading(SensorReading reading)
        {
            var pos = (reading as RangeReading).ReadingPosition;
            var sensorPos = (reading as RangeReading).SensorPosition;
            if (pos[0] != sensorPos[0] && pos[1] != sensorPos[1])
            {
                this.RecentReading = reading;
                foreach (var subscriber in subscribers)
                {
                    subscriber.ReciveSensorReading(this);
                }
            }
        }
    }
}
