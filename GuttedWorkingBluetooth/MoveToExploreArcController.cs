using System.Collections.Generic;

namespace Capstone
{
    public class MoveToExploreArcController : MovementControl
    {

        public ArcSegment Arc { get; private set; }
        private List<double> AnglesToMoveTo;
        public MoveToExploreArcController(Robot robot, ObstacleSurface surface, ArcSegment arc) : base(robot, surface)
        {
            this.Arc = arc;
        }
        public override void Abort()
        {
            this.CompletedSuccessfully = CompletedSuccessfully || ArcExplored();
            if (CompletedSuccessfully)
                System.Diagnostics.Debug.WriteLine($"Move to explore arc completed successfully. Highest obstacle certinty is {Surface.GetHighestObstacleConfidenceInArcSegmant(Arc)}, lowest confidence is {Surface.GetLowestConfidneceInArcSegmant(Arc)}");
            base.Abort();
        }
        public override void Execute()
        {
            System.Diagnostics.Debug.WriteLine("Executing move to explore arc command");
            if (ArcExplored())
            {
                Abort();
            }
            else
            {
                this.AnglesToMoveTo = new List<double>();
                AnglesToMoveTo.Add(Arc.FinalAngleA);
                AnglesToMoveTo.Add(Arc.FinalAngleB);
                MoveToNextAngle();
            }
        }
        private bool ArcExplored()
        {
            var confidence = Surface.GetLowestConfidneceInArcSegmant(Arc);
            return confidence >= RequiredConfidenceThreshold;
        }
        private void MoveToNextAngle()
        {
            if (!ArcExplored())
            {
                var angle = AnglesToMoveTo[0];
                var angleMover = new MoveToAngleController(this.Robot, angle);
                angleMover.CallOnMovementFinished += MoveToAngleFinished;
                angleMover.Execute();
            }
            else
            {
                Abort();
            }
        }
        private void MoveToAngleFinished(MovementControl angleMover)
        {
            if (!Aborted)
            {
                if (angleMover.CompletedSuccessfully)
                {
                    AnglesToMoveTo.RemoveAt(0);
                    if (AnglesToMoveTo.Count > 0)
                    {
                        MoveToNextAngle();
                    }
                    else
                    {
                        CompletedSuccessfully = true;
                        Abort();
                    }
                }
                else
                {
                    Abort();
                }
            }
        }
    }
}
