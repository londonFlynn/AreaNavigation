using System.Collections.Generic;

namespace Capstone
{
    public class ContructedMap : Map, ISensorReadingSubsriber
    {
        public List<RangeReading> RangeReadings = new List<RangeReading>();
        private Robot Robot;
        private const double ZeroConfidenceIncreaseThreshold = 20;
        public ContructedMap(Robot robot)
        {
            this.Robot = robot;
            robot.USSensor.SubsribeToNewReadings(this);
            robot.IRSensor.SubsribeToNewReadings(this);
        }
        public void ReciveSensorReading(Sensor sensor)
        {
            if (!RangeReadings.Contains(sensor.GetCurrentReading() as RangeReading) /*&& Robot.MovementCommandState == MovementCommandState.NEUTRAL*/)
            {
                var reading = sensor.GetCurrentReading() as RangeReading;
                reading.SetPanel(panel);
                reading.SetScale(scale, horizontalOffset, verticalOffset);
                reading.StartDisplay();
                UpdateReadingConfidence(reading);
                RangeReadings.Add(reading);
                if (RangeReadings.Count == 2)
                {
                    this.Walls.Add(new ContructedWall(new List<RangeReading>(new RangeReading[] { RangeReadings[0], RangeReadings[1] })));
                    Walls[0].SetPanel(panel);
                    Walls[0].SetScale(scale, horizontalOffset, verticalOffset);
                    Walls[0].StartDisplay();
                }
                else if (RangeReadings.Count > 2)
                {
                    (this.Walls[0] as ContructedWall).AddRangeReading(reading);
                }
                this.NotifyDisplayChanged();
            }
        }
        private void UpdateReadingConfidence(RangeReading reading)
        {
            foreach (RangeReading other in RangeReadings)
            {
                var distance = reading.DistanceFromOtherReading(other);
                distance = distance < ZeroConfidenceIncreaseThreshold ? distance : ZeroConfidenceIncreaseThreshold;
                if (distance < ZeroConfidenceIncreaseThreshold)
                {
                    var confidenceChange = (-(distance - (ZeroConfidenceIncreaseThreshold / 2)) + (ZeroConfidenceIncreaseThreshold / 2)) / ZeroConfidenceIncreaseThreshold;
                    confidenceChange = 0.5 * confidenceChange;
                    other.Confidence = RangeReading.ConfidenceFromConfidenceChange(other.Confidence, confidenceChange);
                    reading.Confidence = RangeReading.ConfidenceFromConfidenceChange(reading.Confidence, confidenceChange);
                }
            }
        }



        public override void SetPanel(System.Windows.Controls.Canvas panel)
        {
            base.SetPanel(panel);
            foreach (RangeReading reading in RangeReadings)
            {
                reading.SetPanel(panel);
            }
            this.panel = panel;
        }

        public override void SetScale(double scale, double horizontalOffset, double verticalOffset)
        {
            base.SetScale(scale, horizontalOffset, verticalOffset);
            foreach (RangeReading reading in RangeReadings)
            {
                reading.SetScale(scale, horizontalOffset, verticalOffset);
            }
            this.scale = scale;
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
        }
        private System.Windows.Controls.Canvas panel;
        private double scale;
        private double verticalOffset;
        private double horizontalOffset;

        public override void StartDisplay()
        {
            foreach (RangeReading reading in RangeReadings)
            {
                reading.StartDisplay();
            }
            base.StartDisplay();
        }
        public override void UpdateDisplay()
        {
            foreach (RangeReading reading in RangeReadings)
            {
                reading.UpdateDisplay();
            }
            base.UpdateDisplay();
        }
        public override double TopMostPosition()
        {
            double top = double.MaxValue;
            foreach (RangeReading reading in RangeReadings)
            {
                top = reading.SensorPosition[1] < top ? reading.SensorPosition[1] : top;
                top = reading.ReadingPosition[1] < top ? reading.ReadingPosition[1] : top;
            }
            return top < base.TopMostPosition() ? top : base.TopMostPosition();
        }
        public override double RightMostPosition()
        {
            double right = double.MinValue;
            foreach (RangeReading reading in RangeReadings)
            {
                right = reading.SensorPosition[0] > right ? reading.SensorPosition[0] : right;
                right = reading.ReadingPosition[0] > right ? reading.ReadingPosition[0] : right;
            }
            return right > base.RightMostPosition() ? right : base.RightMostPosition();
        }
        public override double LeftMostPosition()
        {
            double left = double.MaxValue;
            foreach (RangeReading reading in RangeReadings)
            {
                left = reading.SensorPosition[0] < left ? reading.SensorPosition[0] : left;
                left = reading.ReadingPosition[0] < left ? reading.ReadingPosition[0] : left;
            }
            return left < base.LeftMostPosition() ? left : base.LeftMostPosition();
        }
        public override double BottomMostPosition()
        {
            double bottom = double.MinValue;
            foreach (RangeReading reading in RangeReadings)
            {
                bottom = reading.SensorPosition[1] > bottom ? reading.SensorPosition[1] : bottom;
                bottom = reading.ReadingPosition[1] > bottom ? reading.ReadingPosition[1] : bottom;
            }
            return bottom > base.BottomMostPosition() ? bottom : base.BottomMostPosition();
        }
    }
}
