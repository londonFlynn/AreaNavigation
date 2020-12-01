using RoboticNavigation.Robots;
using RoboticNavigation.Sensors.SensorReadings;
using RoboticNavigation.Surface;
using System;
using System.Timers;

namespace RoboticNavigation.MovementControls
{
    public abstract class MovementControl : ISubscribesToRobotPostionChange
    {
        //protected static readonly TimeSpan MovementTimeout = TimeSpan.FromMilliseconds(1000);
        private static System.Timers.Timer TimeoutTimer;
        protected const double RequiredConfidenceThreshold = 0.75;
        protected const double MaximumObstacleConfidence = 0.5;
        protected const double PositionAcceptibleMarginOfError = 1;
        protected bool Aborted = false;
        protected Robot Robot;
        protected ObstacleSurface Surface;
        protected int TimeoutTime = 1000;
        //protected DateTime LastTimePositionWasChanged;
        public bool CompletedSuccessfully { get; protected set; }
        public abstract void Execute();
        public virtual void Abort()
        {
            if (!Aborted)
            {
                StopTimeout();
                this.Aborted = true;
                Robot.MovementCommandState = MovementDirection.NEUTRAL;
                CallOnMovementFinished.DynamicInvoke(this);
            }
        }
        public virtual void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem) { }
        public MovementControl(Robot robot, ObstacleSurface surface)
        {
            this.Robot = robot;
            this.Surface = surface;
        }
        protected void StartTimeoutTimer()
        {
            TimeoutTimer = new System.Timers.Timer(TimeoutTime);
            TimeoutTimer.Elapsed += OnTimeout;
            TimeoutTimer.AutoReset = true;
            TimeoutTimer.Enabled = true;
        }
        protected void ResetTimeout()
        {
            if (!(TimeoutTimer is null))
            {
                TimeoutTimer.Stop();
                TimeoutTimer.Start();
            }
        }
        protected void StopTimeout()
        {
            if (!(TimeoutTimer is null))
            {
                TimeoutTimer.Stop();
            }
        }
        protected virtual void OnTimeout(Object source, ElapsedEventArgs e)
        {
            Abort();
        }
        public CallOnMovementFinished CallOnMovementFinished;
    }
    public delegate void CallOnMovementFinished(MovementControl caller);
}
