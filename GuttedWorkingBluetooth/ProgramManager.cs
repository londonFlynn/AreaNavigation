﻿using System.Threading.Tasks;

namespace Capstone
{
    public class ProgramManager
    {
        public ObstacleSurfaceUpdater ContructedMap;
        public Robot Robot;
        protected MainWindow Page;
        private MovementControl ActiveControl;
        public ProgramManager(MainWindow page)
        {
            this.Page = page;
            this.Robot = new EV3Robot();
            this.ContructedMap = new ObstacleSurfaceUpdater(Robot);
        }
        public async Task<NetworkPath> PathFromRobotToPoint(Vector2d<double> point)
        {
            var generator = new NetworkGenerator(this.ContructedMap.ObstacleSurface, this.Robot.RequiredClearWidth() / 2);
            var network = await Task.Factory.StartNew(() => generator.GeneratePathNetwork());
            var path = network.GeneratePath(this.Robot.Position, point);
            var trimer = new PathTrimer(path, this.ContructedMap.ObstacleSurface, this.Robot.RequiredClearWidth() / 2);
            return trimer.Trim();
        }
        public void MoveRobotToPoint(Vector2d<double> point)
        {
            var move = new MoveToDestinationController(this.Robot, this.ContructedMap.ObstacleSurface, point);
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
            var move = new MoveToExploreArcController(this.Robot, this.ContructedMap.ObstacleSurface, arc);
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
            var move = new MoveDistanceController(this.Robot, this.ContructedMap.ObstacleSurface, (point - Robot.Position).Magnitude());
            move.CallOnMovementFinished += MovedToPoint;
            move.Execute();
        }
        private void MoveDirectlyToPointTest(Vector2d<double> point)
        {
            var move = new MoveDirectlyToPositionController(this.Robot, this.ContructedMap.ObstacleSurface, point);
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
