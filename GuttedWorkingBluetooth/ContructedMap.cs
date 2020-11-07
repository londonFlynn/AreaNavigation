using System;
using System.Collections.Generic;
using System.Timers;

namespace Capstone
{
    public class ContructedMap : Map, ISensorReadingSubsriber, ISubscribesToRobotPostionChange
    {
        public List<RangeReading> RangeReadings = new List<RangeReading>();
        public List<PositionOccupiedByRobotMemory> PosOccupied = new List<PositionOccupiedByRobotMemory>();
        private Robot Robot;
        public ObstacleSurface ObstacleSurface;
        private Random Random = new Random();

        public ContructedMap(Robot robot)
        {
            this.Robot = robot;
            robot.USSensor.SubsribeToNewReadings(this);
            robot.IRSensor.SubsribeToNewReadings(this);
            robot.SubscribeToRobotPositionChange(this);
            this.ObstacleSurface = new ObstacleSurface(3, 150, 150);
            StartRandomResamplings();
        }
        private void StartRandomResamplings()
        {
            var timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(DoRandomResampling);
            timer.Interval = 50;
            timer.Enabled = true;
        }
        private void DoRandomResampling(object source, ElapsedEventArgs e)
        {
            RandomlyResampleRangeReading();
            RandomlyResamplePastPosition();
        }
        private void RandomlyResampleRangeReading()
        {
            if (RangeReadings.Count > 0)
            {
                int index = Random.Next(RangeReadings.Count);
                ObstacleSurface.MatchToRangeReading(RangeReadings[index], 0.125, false);

            }
        }
        private void RandomlyResamplePastPosition()
        {
            if (PosOccupied.Count > 0)
            {
                int index = Random.Next(PosOccupied.Count);
                ObstacleSurface.MatchToAreaEmptyReading(PosOccupied[index], 0.25, false);
            }
        }

        public void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem)
        {
            this.PosOccupied.Add(mem);
            ObstacleSurface.MatchToAreaEmptyReading(mem, 1);
            this.NotifyDisplayChanged();
        }
        public void ReciveSensorReading(Sensor sensor)
        {
            if (!RangeReadings.Contains(sensor.GetCurrentReading() as RangeReading) /*&& Robot.MovementCommandState == MovementCommandState.NEUTRAL*/)
            {
                var reading = sensor.GetCurrentReading() as RangeReading;
                //reading.SetPanel(panel);
                //reading.SetScale(scale, horizontalOffset, verticalOffset);
                //reading.StartDisplay();
                //RangeReadings.Add(reading);
                ObstacleSurface.MatchToRangeReading(reading, 0.5);
                this.NotifyDisplayChanged();
            }
        }












        //Display stuffs
        public override void StartDisplay()
        {
            ObstacleSurface.StartDisplay();
            //Robot.StartDisplay();
            foreach (var reading in RangeReadings)
            {
                reading.StartDisplay();
            }
        }
        public override void UpdateDisplay()
        {
            ObstacleSurface.UpdateDisplay();
            Robot.UpdateDisplay();
            foreach (var reading in RangeReadings)
            {
                reading.UpdateDisplay();
            }
        }
        public override void SetPanel(System.Windows.Controls.Canvas panel)
        {
            this.panel = panel;
            ObstacleSurface.SetPanel(panel);
            Robot.SetPanel(panel);
            foreach (var reading in RangeReadings)
            {
                reading.SetPanel(panel);
            }

        }
        public override void SetScale(double scale, double horizontalOffset, double verticalOffset)
        {
            this.scale = scale;
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
            this.ObstacleSurface.SetScale(scale, horizontalOffset, verticalOffset);
            Robot.SetScale(scale, horizontalOffset, verticalOffset);
            foreach (var reading in RangeReadings)
            {
                reading.SetScale(scale, horizontalOffset, verticalOffset);
            }
        }
        public override double TopMostPosition()
        {
            double top = double.MaxValue;
            foreach (var reading in RangeReadings)
            {
                if (reading.TopMostPosition() < top)
                    top = reading.TopMostPosition();
            }
            top = top < ObstacleSurface.TopMostPosition() ? top : ObstacleSurface.TopMostPosition();
            top = top < Robot.TopMostPosition() ? top : Robot.TopMostPosition();
            return top;
        }
        public override double RightMostPosition()
        {
            double right = double.MinValue;
            foreach (var reading in RangeReadings)
            {
                if (reading.RightMostPosition() > right)
                    right = reading.RightMostPosition();
            }
            right = right > ObstacleSurface.RightMostPosition() ? right : ObstacleSurface.RightMostPosition();
            right = right > Robot.RightMostPosition() ? right : Robot.RightMostPosition();
            return right;
        }
        public override double LeftMostPosition()
        {
            double left = double.MaxValue;
            foreach (var reading in RangeReadings)
            {
                if (reading.LeftMostPosition() < left)
                    left = reading.LeftMostPosition();
            }
            left = left < ObstacleSurface.LeftMostPosition() ? left : ObstacleSurface.LeftMostPosition();
            left = left < Robot.LeftMostPosition() ? left : Robot.LeftMostPosition();
            return left;
        }
        public override double BottomMostPosition()
        {
            double bottom = double.MinValue;
            foreach (var reading in RangeReadings)
            {
                if (reading.BottomMostPosition() > bottom)
                    bottom = reading.BottomMostPosition();
            }
            bottom = bottom > ObstacleSurface.BottomMostPosition() ? bottom : ObstacleSurface.BottomMostPosition();
            bottom = bottom > Robot.BottomMostPosition() ? bottom : Robot.BottomMostPosition();
            return bottom;
        }
        public override void StopDisplaying()
        {
            foreach (var reading in RangeReadings)
            {
                reading.StopDisplaying();
            }
            Robot.StopDisplaying();
            ObstacleSurface.StopDisplaying();
        }
    }
}
