namespace Capstone
{
    public class PositionOccupiedByRobotMemory : SensorReading
    {
        public Vector2d<double>[] Shape;
        public int TimesSampled = 0;
        public PositionOccupiedByRobotMemory(Vector2d<double>[] shape)
        {
            this.Shape = shape;
        }
    }
}
