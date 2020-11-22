namespace Capstone
{
    public class PositionOccupiedByRobotMemory : SensorReading
    {
        public Vector2d<double>[] Shape;
        public int TimesSampled = 0;
        public double Angle { get; private set; }
        public Vector2d<double> Position { get; private set; }
        public PositionOccupiedByRobotMemory(Vector2d<double>[] shape, double angle, Vector2d<double> position)
        {
            this.Shape = shape;
            this.Angle = angle;
            this.Position = position;
        }
    }
}
