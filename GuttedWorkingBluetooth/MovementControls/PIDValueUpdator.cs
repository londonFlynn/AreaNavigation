using RoboticNavigation.Robots;
using RoboticNavigation.Sensors.SensorReadings;

namespace RoboticNavigation.MovementControls
{
    public abstract class PIDValueUpdator : ISubscribesToRobotPostionChange
    {
        protected PIDController PID;
        public PIDValueUpdator(PIDController pid)
        {
            this.PID = pid;
        }

        public abstract void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem);
    }
}
