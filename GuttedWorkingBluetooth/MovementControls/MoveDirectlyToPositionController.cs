using RoboticNavigation.ArcSegmants;
using RoboticNavigation.Robots;
using RoboticNavigation.Surface;
using RoboticNavigation.VectorMath;

namespace RoboticNavigation.MovementControls
{
    public class MoveDirectlyToPositionController : MovementControl
    {
        private ArcSegment ArcDisplay;
        public ArcSegment Displayed
        {
            get { return ArcDisplay; }
            set
            {
                ArcDisplay = value;
            }
        }
        private Vector2d<double> Position;
        public MoveDirectlyToPositionController(Robot robot, ObstacleSurface surface, Vector2d<double> position) : base(robot, surface)
        {
            this.Position = position;
            double angle = AngleFromRobotToPosition();
            var arc = Robot.GetArcSegmantToFitRobotInDirection(angle);
            Displayed = arc;
        }
        public override void Abort()
        {
            CompletedSuccessfully = HasReachedDestination();
            if (CompletedSuccessfully)
                System.Diagnostics.Debug.WriteLine($"Move directly to position {Position} completed successfully");
            base.Abort();
        }
        private bool HasReachedDestination()
        {
            return (Robot.Position - this.Position).Magnitude() < Robot.MaxiumDistanceFromCenter / 2;
        }
        public override void Execute()
        {
            System.Diagnostics.Debug.WriteLine($"Executing move directly to position {Position} command");
            if (HasReachedDestination())
            {
                this.Abort();
            }
            else
            {
                DoGatherConfidence();
            }
        }
        private void DoGatherConfidence()
        {
            double angle = AngleFromRobotToPosition();
            var arc = Robot.GetArcSegmantToFitRobotInDirection(angle);
            var arcMove = new MoveToExploreArcController(this.Robot, this.Surface, arc);
            arcMove.CallOnMovementFinished += MoveToExploreArcHasFinished;
            arcMove.Execute();
        }
        private void MoveToExploreArcHasFinished(MovementControl arcMovement)
        {
            if (!Aborted)
            {
                if (arcMovement.CompletedSuccessfully)
                {
                    //var arc = (arcMovement as MoveToExploreArcController).Arc;
                    //var distance = System.Math.Min(arc.RaySegmant.Magnitude(), (this.Position - Robot.Position).Magnitude());
                    //var newArc = new ArcSegment(arc.AngleInRadians, arc.Position, (arc.RaySegmant / arc.RaySegmant.Magnitude()) * distance);
                    //if (Surface.GetHighestObstacleConfidenceInArcSegmant(newArc) > MaximumObstacleConfidence)
                    //{

                    //    System.Diagnostics.Debug.WriteLine($"Obstacle in the the arc has caused failure. Confidence was {Surface.GetHighestObstacleConfidenceInArcSegmant(newArc)}");
                    //    Abort();
                    //}
                    //else
                    //{
                    var angleMove = new MoveToAngleController(this.Robot, this.AngleFromRobotToPosition());
                    angleMove.CallOnMovementFinished += MoveToAngleHasFinished;
                    angleMove.Execute();
                    //}
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Moving to explore the arc has caused failure");
                    Abort();
                }
            }
        }
        private double AngleFromRobotToPosition()
        {
            return (this.Position - Robot.Position).Angle();
        }
        private void MoveToAngleHasFinished(MovementControl angleMovmenet)
        {
            if (!Aborted)
            {
                if (angleMovmenet.CompletedSuccessfully)
                {
                    var moveDistance = new MoveDistanceController(this.Robot, this.Surface, (this.Position - this.Robot.Position).Magnitude());
                    moveDistance.CallOnMovementFinished += MoveDistanceHasFinished;
                    moveDistance.Execute();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Moving to angle has caused failure");
                    Abort();
                }
            }
        }
        private void MoveDistanceHasFinished(MovementControl disntaceMovement)
        {
            if (!Aborted)
            {
                var dist = disntaceMovement as MoveDistanceController;
                if (dist.ConfidenceFailure && !this.Aborted)
                {
                    DoGatherConfidence();
                }
                else
                {
                    Abort();
                }
            }
        }
    }
}
