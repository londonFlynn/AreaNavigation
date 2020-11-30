using RoboticNavigation.MovementControls;
using RoboticNavigation.Robots;
using RoboticNavigation.Surface;
using RoboticNavigation.VectorMath;

namespace RoboticNavigation
{
    public class ProgramManager
    {
        public ObstacleSurfaceUpdater SurfaceUpdater;
        public ObstacleSurface Surface;
        public Robot Robot;
        protected Viewbox Page;
        private MovementControl ActiveControl;
        public ProgramManager(Viewbox page)
        {
            this.Page = page;
            this.Robot = new EV3Robot();
            Robot.GetDisplayer().StartDisplaying();
            this.Surface = new AdjustibleObstacleSurface(3, 100, 100);
            this.SurfaceUpdater = new ObstacleSurfaceUpdater(Robot, Surface as AdjustibleObstacleSurface);
            Surface.GetDisplayer().StartDisplaying();
        }
        public void SaveObstacleSurface()
        {
            this.Surface.Save();
        }
        public void LoadObstacleSurface(string filepath)
        {
            System.Diagnostics.Debug.WriteLine(filepath);
            var surface = ObstacleSurfaceLoader.Load(filepath);
            //surface.GetDisplayer().StartDisplaying();
            //surface.ChangeResolution(30).GetDisplayer().StartDisplaying();
        }
        public void MoveRobotToPoint(Vector2d<double> point)
        {
            var move = new MoveToDestinationController(this.Robot, this.Surface, point);
            move.CallOnMovementFinished += MovedToPoint;
            move.Execute();
        }
        private void MoveToExploreArcTest(Vector2d<double> point)
        {
            if (!(ActiveControl is null))
            {
                ActiveControl.Abort();

            }
            Page.HideMoveToDestinationPath();
            var arc = Robot.GetArcSegmantToFitRobotInDirection((point - Robot.Position).Angle());
            var move = new MoveToExploreArcController(this.Robot, this.Surface, arc);
            move.CallOnMovementFinished += MovedToPoint;
            ActiveControl = move;
            move.Execute();
        }
        private void MoveToAngleTest(Vector2d<double> point)
        {
            var move = new MoveToAngleController(this.Robot, (point - Robot.Position).Angle());
            move.CallOnMovementFinished += MovedToPoint;
            move.Execute();
        }
        private void MoveDistanceTest(Vector2d<double> point)
        {
            var move = new MoveDistanceController(this.Robot, this.Surface, (point - Robot.Position).Magnitude());
            move.CallOnMovementFinished += MovedToPoint;
            move.Execute();
        }
        private void MoveDirectlyToPointTest(Vector2d<double> point)
        {
            var move = new MoveDirectlyToPositionController(this.Robot, this.Surface, point);
            move.CallOnMovementFinished += MovedToPoint;
            move.Execute();
        }

        private void MovedToPoint(MovementControl mc)
        {
            Page.HideArcSegments();
            Page.HideMoveToDestinationPath();
        }
    }
}
