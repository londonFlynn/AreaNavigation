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
            ApplicationConfig.Load();
            this.Page = page;
            this.Robot = EV3Robot.Load();
            Robot.GetDisplayer().StartDisplaying();
            this.Surface = new AdjustibleObstacleSurface(ApplicationConfig.ObstacleSurfaceCMPerPixel, ApplicationConfig.ObstacleSurfaceWidth, ApplicationConfig.ObstacleSurfaceHeight);
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
            if (!(ActiveControl is null))
            {
                ActiveControl.Abort();
            }
            ActiveControl = new MoveToDestinationController(this.Robot, this.Surface, point);
            ActiveControl.CallOnMovementFinished += MovedToPoint;
            ActiveControl.Execute();
            //MoveDistanceTest(point);
            //MoveToAngleTest(point);
        }
        private void MoveToExploreArcTest(Vector2d<double> point)
        {
            var arc = Robot.GetArcSegmantToFitRobotInDirection((point - Robot.Position).Angle());
            ActiveControl = new MoveToExploreArcController(this.Robot, this.Surface, arc);
            ActiveControl.CallOnMovementFinished += MovedToPoint;
            ActiveControl.Execute();
        }
        private void MoveToAngleTest(Vector2d<double> point)
        {
            ActiveControl = new MoveToAngleController(this.Robot, (point - Robot.Position).Angle());
            ActiveControl.CallOnMovementFinished += MovedToPoint;
            ActiveControl.Execute();
        }
        private void MoveDistanceTest(Vector2d<double> point)
        {
            ActiveControl = new MoveDistanceController(this.Robot, this.Surface, (point - Robot.Position).Magnitude(), true);
            ActiveControl.CallOnMovementFinished += MovedToPoint;
            ActiveControl.Execute();
        }
        private void MoveDirectlyToPointTest(Vector2d<double> point)
        {
            ActiveControl = new MoveDirectlyToPositionController(this.Robot, this.Surface, point);
            ActiveControl.CallOnMovementFinished += MovedToPoint;
            ActiveControl.Execute();
        }

        private void MovedToPoint(MovementControl mc)
        {
            Page.HideArcSegments();
            Page.HideMoveToDestinationPath();
        }
    }
}
