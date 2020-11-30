using RoboticNavigation.Robots;
using RoboticNavigation.Sensors.SensorReadings;
using RoboticNavigation.Surface;
using System;

namespace RoboticNavigation.MovementControls
{
    public abstract class MovementControl : ISubscribesToRobotPostionChange
    {
        protected static readonly TimeSpan MovementTimeout = TimeSpan.FromMilliseconds(1000);
        protected const double RequiredConfidenceThreshold = 0.75;
        protected const double MaximumObstacleConfidence = 0.5;
        protected const double PositionAcceptibleMarginOfError = 1;
        protected bool Aborted = false;
        protected Robot Robot;
        protected ObstacleSurface Surface;
        protected DateTime LastTimePositionWasChanged;
        public bool CompletedSuccessfully { get; protected set; }
        public abstract void Execute();
        public virtual void Abort()
        {
            if (!Aborted)
            {
                this.Aborted = true;
                Robot.MovementCommandState = MovementCommandState.NEUTRAL;
                CallOnMovementFinished.DynamicInvoke(this);
            }
        }
        public virtual void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem) { }
        public MovementControl(Robot robot, ObstacleSurface surface)
        {
            this.Robot = robot;
            this.Surface = surface;
        }

        public CallOnMovementFinished CallOnMovementFinished;
    }
    public delegate void CallOnMovementFinished(MovementControl caller);
}
