namespace Capstone
{
    public interface ISubscribesToRobotPostionChange
    {
        void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem);
    }
}
