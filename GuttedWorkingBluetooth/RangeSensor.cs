using System;

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
        public override void SetRecentReading(SensorReading reading)
        {
            var pos = (reading as RangeReading).ReadingPosition;
            var sensorPos = (reading as RangeReading).SensorPosition;
            if (pos[0] != sensorPos[0] && pos[1] != sensorPos[1])
            {

                var oldVal = RecentReading is null ? (reading as RangeReading).Distance : (RecentReading as RangeReading).Distance;
                this.RecentReading = reading;
                var newVal = (RecentReading as RangeReading).Distance;
                if (Math.Abs(newVal - oldVal) < 5)
                {


                    foreach (var subscriber in subscribers)
                    {
                        subscriber.ReciveSensorReading(this);
                    }
                }
            }
        }
    }
}
