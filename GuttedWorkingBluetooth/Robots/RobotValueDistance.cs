namespace RoboticNavigation.Robots
{
    public abstract class RobotValueDistance
    {
        protected Robot Robot;
        public RobotValueDistance(Robot robot)
        {
            this.Robot = robot;
        }
        public abstract double GetValue();
    }
}
