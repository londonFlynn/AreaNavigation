using System.Threading.Tasks;

namespace Capstone
{
    public class MoveToDestinationController : MovementControl
    {

        private Vector2d<double> EndDestination;
        private NetworkPath _currentPath;
        private NetworkPath LastPath;
        private int PathIndex = 0;
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
            DoCalculatePath();
        }
        private async void DoCalculatePath()
        {
            if (HasReachedDestination())
            {
                Abort();
            }
            else
            {
                var generator = new NetworkGenerator(this.Surface, this.Robot.RequiredClearWidth() / 2);
                var network = await Task.Factory.StartNew(() => generator.GeneratePathNetwork());
                var path = network.GeneratePath(this.Robot.Position, EndDestination);
                var trimer = new PathTrimer(path, this.Surface, this.Robot.RequiredClearWidth() / 2);
                this.CurrentPath = trimer.Trim();
                if (CurrentPath is null)
                {
                    Abort();
                }
                else
                {
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
            moveToPos.Execute();
        }
        public override void Abort()
        {
            this.CompletedSuccessfully = HasReachedDestination();
            if (CompletedSuccessfully)
                System.Diagnostics.Debug.WriteLine($"Move to destination {this.EndDestination} completed successfully");
            base.Abort();
        }
    }
}
