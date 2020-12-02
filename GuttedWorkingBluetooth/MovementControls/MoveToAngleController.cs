using RoboticNavigation.Robots;
using RoboticNavigation.Sensors.SensorReadings;
using System;

namespace RoboticNavigation.MovementControls
{
    public class MoveToAngleController : MovementControl
    {
        public readonly double AngleInRadians;
        private double LastAngle;
        public double AccecptibleMarginOfError = Math.PI / 90;
        private bool TurningRight;
        public PIDMotorController PIDController;
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
            StartTimeoutTimer();
            if (!AngleIsWithinMarginOfError())
            {
                Robot.SubscribeToRobotPositionChange(this);
                MovementDirection direction;
                if (ShouldTurnRight(Robot.Orientation))
                {
                    direction = MovementDirection.RIGHT;
                    TurningRight = true;
                }
                else
                {
                    direction = MovementDirection.LEFT;
                }
                this.PIDController = Robot.GeneratePIDControllerForDirection(direction, AngleInRadians, PIDCompleted);
                this.AccecptibleMarginOfError = PIDController.MarginOfError;
                this.PIDController.Execute();
            }
            else
            {
                Abort();
            }
        }
        private void PIDCompleted(PIDMotorController pid)
        {
            CompletedSuccessfully = true;
            Abort();
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
                    ResetTimeout();
                }
            }
        }
        public override void Abort()
        {
            if (!(PIDController is null))
            {
                PIDController.Abort();
            }
            CompletedSuccessfully = CompletedSuccessfully || AngleIsWithinMarginOfError();
            Robot.UnsubscribeToRobotPositionChange(this);
            if (CompletedSuccessfully)
                System.Diagnostics.Debug.WriteLine($"Turn to angle {this.AngleInRadians} completed successfully");
            base.Abort();
        }
    }
}
