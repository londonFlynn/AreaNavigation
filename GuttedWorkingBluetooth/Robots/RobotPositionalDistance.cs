using RoboticNavigation.VectorMath;

namespace RoboticNavigation.Robots
{
    public class RobotPositionalDistance : RobotValueDistance
    {
        private Vector2d<double> startPosition;
        private double targetDistance;
        public RobotPositionalDistance(Robot robot, double targetDistance) : base(robot)
        {
            this.startPosition = robot.Position;
            this.targetDistance = targetDistance;
        }

        public override double GetValue()
        {
            return targetDistance - (Robot.Position - startPosition).Magnitude();
        }
    }
}
