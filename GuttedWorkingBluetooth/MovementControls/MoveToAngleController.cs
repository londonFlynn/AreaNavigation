using RoboticNavigation.Robots;
using RoboticNavigation.Sensors.SensorReadings;
using System;

namespace RoboticNavigation.MovementControls
{
    public class MoveToAngleController : MovementControl
    {
        public readonly double AngleInRadians;
        private double LastAngle;
        public const double AccecptibleMarginOfError = Math.PI / 90;
        private bool TurningRight;
        public MoveToAngleController(Robot robot, double angleInRadians) : base(robot, null)
        {
            while (angleInRadians < 0)
            {
                angleInRadians += 2 * System.Math.PI;
            }
            this.AngleInRadians = angleInRadians % (2 * System.Math.PI);
        }
        public override void Execute()
        {
            System.Diagnostics.Debug.WriteLine($"Executing turn to angle {AngleInRadians} command");
            LastAngle = Robot.Orientation;
            LastTimePositionWasChanged = DateTime.Now;
            if (!AngleIsWithinMarginOfError())
            {
                Robot.SubscribeToRobotPositionChange(this);
                if (ShouldTurnRight(Robot.Orientation))
                {
                    Robot.MovementCommandState = MovementDirection.RIGHT;
                    TurningRight = true;
                }
                else
                {
                    Robot.MovementCommandState = MovementDirection.LEFT;
                }
            }
            else
            {
                Abort();
            }
        }
        private bool ShouldTurnRight(double robotAngle)
        {
            while (robotAngle > AngleInRadians)
            {
                robotAngle = robotAngle - 2 * Math.PI;
            }
            var rotationCount = 0d;
            while (robotAngle < AngleInRadians)
            {
                robotAngle += Math.PI;
                rotationCount += 0.5;
            }
            return rotationCount < 1;
        }
        private bool AngleIsWithinMarginOfError()
        {
            return LastAngle - AccecptibleMarginOfError < AngleInRadians && LastAngle + AccecptibleMarginOfError > AngleInRadians;
        }
        public override void ReciveRobotPositionMemory(PositionOccupiedByRobotMemory mem)
        {
            var shouldTurnRight = ShouldTurnRight(Robot.Orientation);
            if (shouldTurnRight && !TurningRight || TurningRight && !shouldTurnRight)
            {
                CompletedSuccessfully = true;
                Abort();
            }
            else
            {

                if (Robot.Orientation != LastAngle)
                {
                    LastAngle = Robot.Orientation;
                    LastTimePositionWasChanged = DateTime.Now;
                }
                if (DateTime.Now - LastTimePositionWasChanged > MovementTimeout)
                {
                    Abort();
                }
            }
        }
        public override void Abort()
        {
            CompletedSuccessfully = CompletedSuccessfully || AngleIsWithinMarginOfError();
            Robot.UnsubscribeToRobotPositionChange(this);
            if (CompletedSuccessfully)
                System.Diagnostics.Debug.WriteLine($"Turn to angle {this.AngleInRadians} completed successfully");
            base.Abort();
        }
    }
}
