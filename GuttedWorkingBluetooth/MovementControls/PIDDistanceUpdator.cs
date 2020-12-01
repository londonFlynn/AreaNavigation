using RoboticNavigation.Sensors.SensorReadings;

namespace RoboticNavigation.MovementControls
{
    public class PIDDistanceUpdator : PIDValueUpdator
    {
        private double TargetDistance;
        private VectorMath.Vector2d<double> StartingPosition;
        public PIDDistanceUpdator(PIDController pid, double targetDistance, VectorMath.Vector2d<double> startingPosition) : base(pid)
        {
            this.TargetDistance = targetDistance;
            this.StartingPosition = startingPosition;
        }

        public override void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem)
        {
            PID.RecieveCurrentValue(TargetDistance - (StartingPosition - mem.Position).Magnitude());
        }
    }
}
