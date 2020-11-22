using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capstone
{
    public class MoveToDestinationController : MovementControl, IDisplayable, ListenToDispalyChanged
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
                LastPath = _currentPath;
                if (!(LastPath is null))
                {
                    if (startedDisplaying)
                        LastPath.StopDisplaying();
                    LastPath.UnsubsricbeDisplayChanged(this);
                }
                _currentPath = value;
                if (_currentPath != null)
                {
                    SetupPath(_currentPath);
                }
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












        //Display stuffs
        private bool startedDisplaying;
        protected System.Windows.Controls.Canvas panel;
        protected double scale;
        protected double verticalOffset;
        protected double horizontalOffset;
        public void StartDisplay()
        {
            startedDisplaying = true;
            if (!(CurrentPath is null))
            {
                CurrentPath.StartDisplay();
            }
        }
        public void UpdateDisplay()
        {
            if (!(CurrentPath is null))
            {
                CurrentPath.UpdateDisplay();
            }
        }
        public virtual void SetPanel(System.Windows.Controls.Canvas panel)
        {
            this.panel = panel;
            if (!(CurrentPath is null))
            {
                CurrentPath.SetPanel(panel);
            }
        }
        public virtual void SetScale(double scale, double horizontalOffset, double verticalOffset)
        {
            this.scale = scale;
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
            if (!(CurrentPath is null))
            {
                CurrentPath.SetScale(scale, horizontalOffset, verticalOffset);
            }
        }
        public double MaxWidth()
        {
            if (!(CurrentPath is null))
            {
                return CurrentPath.MaxWidth();
            }
            return 0;
        }
        public double MaxHeight()
        {
            if (!(CurrentPath is null))
            {
                return CurrentPath.MaxHeight();
            }
            return 0;
        }
        public double LeftMostPosition()
        {
            if (!(CurrentPath is null))
            {
                return CurrentPath.LeftMostPosition();
            }
            return double.MaxValue;
        }
        public double RightMostPosition()
        {
            if (!(CurrentPath is null))
            {
                return CurrentPath.RightMostPosition();
            }
            return double.MinValue;
        }
        public double TopMostPosition()
        {
            if (!(CurrentPath is null))
            {
                return CurrentPath.TopMostPosition();
            }
            return double.MaxValue;
        }
        public double BottomMostPosition()
        {
            if (!(CurrentPath is null))
            {
                return CurrentPath.BottomMostPosition();
            }
            return double.MinValue;
        }
        public void NotifyDisplayChanged()
        {
            foreach (var listener in listeners)
            {
                listener.HearDisplayChanged();
            }
        }
        public void StopDisplaying()
        {
            startedDisplaying = false;
            if (!(CurrentPath is null))
            {
                CurrentPath.StopDisplaying();
            }
        }
        private List<ListenToDispalyChanged> listeners = new List<ListenToDispalyChanged>();
        public void SubsricbeDisplayChanged(ListenToDispalyChanged listener)
        {
            listeners.Add(listener);
        }
        public void UnsubsricbeDisplayChanged(ListenToDispalyChanged listener)
        {
            listeners.Remove(listener);
        }
        private void SetupPath(NetworkPath path)
        {
            path.SetPanel(panel);
            path.SetScale(scale, horizontalOffset, verticalOffset);
            path.SubsricbeDisplayChanged(this);
            if (startedDisplaying)
            {
                path.StartDisplay();
            }
        }
        public void HearDisplayChanged()
        {
            NotifyDisplayChanged();
        }
    }
}
