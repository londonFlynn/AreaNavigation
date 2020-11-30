using RoboticNavigation.Sensors.SensorReadings;

namespace RoboticNavigation.Robots
{
    public interface ISubscribesToRobotPostionChange
    {
        void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem);
    }
}
