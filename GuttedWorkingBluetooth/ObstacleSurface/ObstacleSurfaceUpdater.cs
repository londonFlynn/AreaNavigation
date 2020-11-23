using System;
using System.Collections.Generic;
using System.Timers;

namespace Capstone
{
    public class ObstacleSurfaceUpdater : ISensorReadingSubsriber, ISubscribesToRobotPostionChange
    {
        public List<RangeReading> RangeReadings = new List<RangeReading>();
        public List<PositionOccupiedByRobotMemory> PosOccupied = new List<PositionOccupiedByRobotMemory>();
        private Robot Robot;
        public ObstacleSurface ObstacleSurface;
        private Random Random = new Random();

        public ObstacleSurfaceUpdater(Robot robot)
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
                var reading = RangeReadings[index];
                ObstacleSurface.MatchToRangeReading(reading, 1 / (7 + (reading.TimesSampled * reading.TimesSampled)), false);
                reading.TimesSampled++;
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
            mem.TimesSampled++;
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
                ObstacleSurface.MatchToRangeReading(reading, 0.25);
                reading.TimesSampled++;
            }
        }
    }
}
