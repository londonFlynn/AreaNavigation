using RoboticNavigation.Sensors.SensorReadings;
using RoboticNavigation.VectorMath;

namespace RoboticNavigation.MovementControls
{
    public class PIDAngleUpdator : PIDValueUpdator
    {
        private double StartingAngle;
        private double TargetAngle;
        private bool Clockwise;
        public PIDAngleUpdator(PIDController pid, double startAngle, double targetAngle, bool clockwise) : base(pid)
        {
            this.StartingAngle = startAngle;
            this.TargetAngle = targetAngle;
            this.Clockwise = clockwise;
        }
        public override void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem)
        {
            var angle = StartingAngle.SignedAngleDifference(TargetAngle);
            angle = Clockwise ? -angle : angle;
            PID.RecieveCurrentValue(angle);
        }
    }
}
