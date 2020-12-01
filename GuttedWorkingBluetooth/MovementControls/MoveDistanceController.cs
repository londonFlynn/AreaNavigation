using RoboticNavigation.ArcSegmants;
using RoboticNavigation.Robots;
using RoboticNavigation.Sensors.SensorReadings;
using RoboticNavigation.Surface;
using RoboticNavigation.VectorMath;

namespace RoboticNavigation.MovementControls
{
    public class MoveDistanceController : MovementControl
    {
        private readonly double TargetDistance;
        public bool ConfidenceFailure { get; private set; }
        private double CurrentDistance { get { return (LastPosition - StartingPosition).Magnitude(); } }
        private Vector2d<double> LastPosition;
        private Vector2d<double> StartingPosition;
        public const double AccecptibleMarginOfError = 1;
        public MoveDistanceController(Robot robot, ObstacleSurface surface, double distance) : base(robot, surface)
        {
            this.TargetDistance = distance;
        }
        public override void Execute()
        {
            System.Diagnostics.Debug.WriteLine($"Executing move distance {TargetDistance} command");
            LastPosition = Robot.Position;
            StartTimeoutTimer();
            this.StartingPosition = Robot.Position;
            if (!HasMovedDistance())
            {
                Robot.SubscribeToRobotPositionChange(this);
                Robot.MovementCommandState = MovementCommandState.FORWARD;
            }
            else
            {
                Abort();
            }

        }
        private bool HasMovedDistance()
        {
            return CurrentDistance + AccecptibleMarginOfError > TargetDistance;
        }
        public override void Abort()
        {
            CompletedSuccessfully = HasMovedDistance();
            Robot.UnsubscribeToRobotPositionChange(this);
            if (CompletedSuccessfully)
            {
                System.Diagnostics.Debug.WriteLine($"Move distance {this.TargetDistance} completed successfully");
            }
            else
            {

                System.Diagnostics.Debug.WriteLine($"Move distance {this.TargetDistance} failed. Only moved {this.CurrentDistance}");
            }
            base.Abort();
        }
        public override void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem)
        {
            if (LastPosition != mem.Position)
            {
                LastPosition = mem.Position;
                ResetTimeout();
            }
            var arc1 = Robot.GetArcSegmantToFitRobotInDirection(Robot.Orientation);
            var distance = System.Math.Min(arc1.RaySegmant.Magnitude(), this.TargetDistance);
            var arc = new ArcSegment(arc1.AngleInRadians, arc1.Position, (arc1.RaySegmant / arc1.RaySegmant.Magnitude()) * distance);
            if (Surface.GetLowestConfidneceInArcSegmant(arc) < RequiredConfidenceThreshold || Surface.GetHighestObstacleConfidenceInArcSegmant(arc) > MaximumObstacleConfidence || HasMovedDistance())
            {
                if (Surface.GetLowestConfidneceInArcSegmant(arc) < RequiredConfidenceThreshold && !HasMovedDistance())
                {
                    ConfidenceFailure = true;
                }
                Abort();
            }

        }
    }
}
