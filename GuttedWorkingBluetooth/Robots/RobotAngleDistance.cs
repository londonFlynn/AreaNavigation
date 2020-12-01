using RoboticNavigation.VectorMath;
namespace RoboticNavigation.Robots
{
    public class RobotAngleDistance : RobotValueDistance
    {
        private double TargetAngle;
        private bool Clockwise;
        public RobotAngleDistance(Robot robot, double targetAngle, bool clockwise) : base(robot)
        {
            this.TargetAngle = targetAngle;
            this.Clockwise = clockwise;
        }

        public override double GetValue()
        {
            var angle = Robot.Orientation.SignedAngleDifference(TargetAngle);
            return Clockwise ? -angle : angle;
        }
    }
}
