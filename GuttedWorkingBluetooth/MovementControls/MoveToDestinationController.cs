using RoboticNavigation.NavNetworks;
using RoboticNavigation.Robots;
using RoboticNavigation.Sensors.SensorReadings;
using RoboticNavigation.Surface;
using RoboticNavigation.VectorMath;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace RoboticNavigation.MovementControls
{
    public class MoveToDestinationController : MovementControl
    {

        private Vector2d<double> EndDestination;
        private NetworkPath _currentPath;
        private NetworkPath LastPath;
        private MovementControl ActiveMovement;
        private int PathIndex = 0;
        protected override int TimeoutTime => base.TimeoutTime * 2;

        private NetworkPath CurrentPath
        {
            get { return _currentPath; }
            set
            {
                _currentPath = value;
            }
        }
        public MoveToDestinationController(Robot robot, ObstacleSurface surface, Vector2d<double> destination) : base(robot, surface)
        {
            this.EndDestination = destination;
        }
        private bool HasReachedDestination()
        {
            return (Robot.Position - this.EndDestination).Magnitude() < Robot.MaxiumDistanceFromCenter / 2;
        }
        public override void Execute()
        {
            System.Diagnostics.Debug.WriteLine($"Executing move to destination {this.EndDestination} command");
            StopTimeout();
            Robot.SubscribeToRobotPositionChange(this);
            DoCalculatePath();
        }
        private async void DoCalculatePath()
        {
            if (!(CurrentPath is null))
            {
                CurrentPath.GetDisplayer().StopDisplaying();
            }
            if (HasReachedDestination())
            {
                Abort();
            }
            else
            {
                var generator = new NetworkGenerator(this.Surface, this.Robot.RequiredClearWidth());
                var network = await Task.Factory.StartNew(() => generator.GeneratePathNetwork());
                var path = network.GeneratePath(this.Robot.Position, EndDestination);
                var trimer = new PathTrimer(path, this.Surface, this.Robot.RequiredClearWidth());
                this.CurrentPath = trimer.Trim();
                if (CurrentPath is null)
                {
                    Abort();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        CurrentPath.GetDisplayer().StartDisplaying();
                    });
                    this.PathIndex = 0;
                    GoToNextPosition();
                }
            }
        }
        private void GoToPositionFinished(MovementControl moveToPosisiton)
        {
            if (!Aborted)
            {
                if (moveToPosisiton.CompletedSuccessfully)
                {
                    if (HasReachedDestination())
                    {
                        Abort();
                    }
                    else
                    {
                        GoToNextPosition();
                    }
                }
                else
                {
                    DoCalculatePath();
                }
            }
        }
        private void GoToNextPosition()
        {
            PathIndex++;
            var moveToPos = new MoveDirectlyToPositionController(this.Robot, this.Surface, this.CurrentPath.Route[PathIndex].Position);
            moveToPos.CallOnMovementFinished += GoToPositionFinished;
            ActiveMovement = moveToPos;
            moveToPos.Execute();
        }
        public override void Abort()
        {
            Robot.UnsubscribeToRobotPositionChange(this);
            if (!(CurrentPath is null))
            {
                CurrentPath.GetDisplayer().StopDisplaying();
            }
            this.CompletedSuccessfully = HasReachedDestination();
            if (CompletedSuccessfully)
                System.Diagnostics.Debug.WriteLine($"Move to destination {this.EndDestination} completed successfully");
            base.Abort();
        }
        public override void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem)
        {
            ResetTimeout();
        }
        protected override void OnTimeout(Object source, ElapsedEventArgs e)
        {
            if (!Surface.PathIsClear(CurrentPath.ToArray(), Robot.RequiredClearWidth() / 2, MaximumObstacleConfidence) && !(ActiveMovement is null))
            {
                ActiveMovement.Abort();
            }
        }
    }
}
