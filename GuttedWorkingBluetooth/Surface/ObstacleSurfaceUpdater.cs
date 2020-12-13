using RoboticNavigation.Robots;
using RoboticNavigation.Sensors;
using RoboticNavigation.Sensors.SensorReadings;
using System;
using System.Collections.Generic;
using System.Timers;

namespace RoboticNavigation.Surface
{
    public class ObstacleSurfaceUpdater : ISensorReadingSubsriber, ISubscribesToRobotPostionChange
    {
        public List<RangeReading> RangeReadings = new List<RangeReading>();
        public List<PositionOccupiedByRobotMemory> PosOccupied = new List<PositionOccupiedByRobotMemory>();
        private Robot Robot;
        public AdjustibleObstacleSurface ObstacleSurface;
        private Random Random = new Random();

        public ObstacleSurfaceUpdater(Robot robot, AdjustibleObstacleSurface surface)
        {
            this.Robot = robot;
            foreach (var sensor in Robot.RangeSensors)
            {
                sensor.SubsribeToNewReadings(this);
            }
            robot.SubscribeToRobotPositionChange(this);
            this.ObstacleSurface = surface;
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
                ObstacleSurface.MatchToRangeReading(reading, 1 / (ApplicationConfig.RangeReadingResampleAdjustment + (reading.TimesSampled * reading.TimesSampled)), false);
                reading.TimesSampled++;
            }
        }
        private void RandomlyResamplePastPosition()
        {
            if (PosOccupied.Count > 0)
            {
                int index = Random.Next(PosOccupied.Count);
                ObstacleSurface.MatchToAreaEmptyReading(PosOccupied[index], ApplicationConfig.PositionResampleAdjustment, false);
            }
        }

        public void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem)
        {
            this.PosOccupied.Add(mem);
            ObstacleSurface.MatchToAreaEmptyReading(mem, ApplicationConfig.PositionStartingSampleAdjustment);
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
                ObstacleSurface.MatchToRangeReading(reading, ApplicationConfig.RangeReadingStartingSampleAdjustment);
                reading.TimesSampled++;
            }
        }
    }
}
