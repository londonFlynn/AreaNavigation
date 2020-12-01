using RoboticNavigation.ArcSegmants;
using RoboticNavigation.Display;
using RoboticNavigation.MovementControls;
using RoboticNavigation.Sensors;
using RoboticNavigation.Sensors.SensorReadings;
using RoboticNavigation.VectorMath;
using System;
using System.Collections.Generic;

namespace RoboticNavigation.Robots
{
    public abstract class Robot : ISensorReadingSubsriber, IDisplayablePositionedItem
    {
        protected readonly double TurnPorportional;
        protected readonly double TurnIntegral;
        protected readonly double TurnDerivative;
        protected readonly double TurnMarginOfError;
        protected readonly double DistancePorportional;
        protected readonly double DistanceIntegral;
        protected readonly double DistanceDerivative;
        protected readonly double DistanceMarginOfError;

        public Robot(double turnPercent, double turnIntegral, double turnDerivative, double turnError, double distancePercent, double distanceIntegral, double distanceDerivative, double distanceError)
        {
            this.TurnPorportional = turnPercent;
            this.TurnIntegral = turnIntegral;
            this.TurnDerivative = turnDerivative;
            this.TurnMarginOfError = turnError;
            this.DistancePorportional = distancePercent;
            this.DistanceIntegral = distanceIntegral;
            this.DistanceDerivative = distanceDerivative;
            this.DistanceMarginOfError = distanceError;
            this.Position = new Vector2d<double>(new double[] { 0, 0 });
        }

        //represents the position of the axis of rotation
        public Vector2d<double> Position { get; protected set; }
        private double _angle;
        public double Orientation
        {
            get { return _angle; }
            protected set
            {
                var angle = value;
                while (angle < 0)
                {
                    angle += 2 * Math.PI;
                }
                _angle = angle % (2 * Math.PI);

            }

        }
        public double Height
        {
            get
            {
                var bottom = double.MaxValue;
                foreach (var pos in this.Shape)
                {
                    if (pos.y < bottom)
                        bottom = pos.y;
                }
                var top = double.MinValue;
                foreach (var pos in this.Shape)
                {
                    if (pos.y > top)
                        top = pos.y;
                }
                return top - bottom;
            }
        }
        public double Width
        {
            get
            {
                var left = double.MaxValue;
                foreach (var pos in this.Shape)
                {
                    if (pos.x < left)
                        left = pos.x;
                }
                var right = double.MinValue;
                foreach (var pos in this.Shape)
                {
                    if (pos.x > right)
                        right = pos.x;
                }
                return right - left;
            }
        }

        private RobotDisplayer Displayer;
        public RoboticCommunication RoboticCommunication { get; protected set; }
        //represents the edges of the robot relative to its axis of rotation
        protected Vector2d<double>[] Shape;
        //protected Sensor[] Sensors;
        public GyroscopeSensor Gyro { get; protected set; }
        public RotationSensor LeftMotor { get; protected set; }
        public RotationSensor RightMotor { get; protected set; }
        public InfraredSensor IRSensor { get; protected set; }
        public UltrasonicSensor USSensor { get; protected set; }
        private MovementDirection _movementCommandState = MovementDirection.NEUTRAL;
        public MovementDirection MovementCommandState
        {
            get { return _movementCommandState; }
            set
            {
                _movementCommandState = value;
                RoboticCommunication.CommandMove(_movementCommandState, 1);
            }
        }
        public Vector2d<double>[] FullRobotPosition()
        {
            Vector2d<double>[] result = new Vector2d<double>[Shape.Length];
            double cos = Math.Cos(Orientation);
            double sin = Math.Sin(Orientation);
            for (int i = 0; i < Shape.Length; i++)
            {
                result[i] = new Vector2d<double>(new double[] {
                Shape[i][0] * cos - Shape[i][1] * sin,
                Shape[i][0] * sin + Shape[i][1] * cos,
                0,
                0,
                });
            }
            for (int i = 0; i < Shape.Length; i++)
            {
                result[i] = result[i] + Position;
            }
            return result;
        }
        public PIDMotorController GeneratePIDControllerForDirection(MovementDirection direction, double target, CallOnPIDFinished pIDFinished)
        {
            if (direction.Equals(MovementDirection.LEFT) || direction.Equals(MovementDirection.RIGHT))
            {
                return GenerateAnglePIDController(direction, target, pIDFinished);
            }
            else if (direction.Equals(MovementDirection.FORWARD) || direction.Equals(MovementDirection.REVERSE))
            {
                return GenerateDistancePIDController(direction, target, pIDFinished);
            }
            else
            {
                return null;
            }
        }
        private PIDMotorController GenerateAnglePIDController(MovementDirection direction, double target, CallOnPIDFinished pIDFinished)
        {
            var angle = Orientation.SignedAngleDifference(target);
            angle = direction.Equals(MovementDirection.RIGHT) ? -angle : angle;
            var angleGetter = new RobotAngleDistance(this, target, direction.Equals(MovementDirection.RIGHT));
            var pid = new PIDMotorController(this.TurnPorportional, this.TurnIntegral, this.TurnDerivative, target, angleGetter.GetValue, this.TurnMarginOfError, direction, pIDFinished, this.RoboticCommunication.CommandMove);
            return pid;
        }
        private PIDMotorController GenerateDistancePIDController(MovementDirection direction, double target, CallOnPIDFinished pIDFinished)
        {
            var distanceGetter = new RobotPositionalDistance(this, target);
            var pid = new PIDMotorController(this.DistancePorportional, this.DistanceIntegral, this.DistanceDerivative, target, distanceGetter.GetValue, this.DistanceMarginOfError, direction, pIDFinished, this.RoboticCommunication.CommandMove);
            return pid;
        }

        public ArcSegment GetArcSegmantToFitRobotInDirection(double anlgeInRadians)
        {
            var left = LeftMostPoint().Rotate(anlgeInRadians).Unit() * this.MaxiumDistanceFromCenter;
            var right = RightMostPoint().Rotate(anlgeInRadians).Unit() * this.MaxiumDistanceFromCenter;
            ArcSegment result;
            if (left.Angle() > right.Angle())
            {
                var angleBetween = Math.Abs(2 * Math.PI - Math.Abs(left.Angle() - right.Angle()));
                result = new ArcSegment(angleBetween, this.Position, left);
            }
            else
            {
                var angleBetween = Math.Abs(right.Angle() - left.Angle());
                result = new ArcSegment(angleBetween, this.Position, left);
            }
            return result;
        }
        public double RequiredClearWidth()
        {
            var left = LeftMostPoint();
            var Right = RightMostPoint();
            return (left - Right).Magnitude();
        }
        public void ReciveSensorReading(Sensor sensor)
        {
            if (sensor is GyroscopeSensor)
                ReciveGyroReading((GyroscopeSensor)sensor);
            if (sensor == this.LeftMotor)
                reciveLeftMotorReading(sensor as RotationSensor);
            if (sensor == this.RightMotor)
                reciveRightMotorReading(sensor as RotationSensor);
        }
        private void reciveRightMotorReading(RotationSensor rotationSensor)
        {
            if (!(this.Gyro.GetCurrentReading() is null))
            {
                var movement = this.RightMotor.DistanceLastReading / 2;
                var moveVector = new Vector2d<double>(new double[] { 0, movement, 0, 0 });
                var angle = ((GyroscopeReading)this.Gyro.GetCurrentReading()).Radians;
                var cos = Math.Cos(angle);
                var sin = Math.Sin(angle);
                moveVector = new Vector2d<double>(new double[] { (moveVector[0] * cos) - (moveVector[1] * sin), (moveVector[0] * sin) + (moveVector[1] * cos), 0, 0 });
                this.Position = this.Position + moveVector;
                //this.Orientation = angle;
                this.UpdatePosition();
            }
        }
        private void reciveLeftMotorReading(RotationSensor rotationSensor)
        {
            if (!(this.Gyro.GetCurrentReading() is null))
            {

                var movement = this.LeftMotor.DistanceLastReading / 2;
                var moveVector = new Vector2d<double>(new double[] { 0, movement, 0, 0 });
                var angle = ((GyroscopeReading)this.Gyro.GetCurrentReading()).Radians;
                var cos = Math.Cos(angle);
                var sin = Math.Sin(angle);
                moveVector = new Vector2d<double>(new double[] { (moveVector[0] * cos) - (moveVector[1] * sin), (moveVector[0] * sin) + (moveVector[1] * cos), 0, 0 });
                this.Position = this.Position + moveVector;
                //this.Orientation = angle;
                this.UpdatePosition();
            }
        }
        public double MaxiumDistanceFromCenter
        {
            get
            {
                var max = 0d;
                foreach (var point in this.Shape)
                {
                    var magnitude = point.Magnitude();
                    max = magnitude > max ? magnitude : max;
                }
                return max;
            }
        }
        private void ReciveGyroReading(GyroscopeSensor sensor)
        {

            this.Orientation = ((GyroscopeReading)this.Gyro.GetCurrentReading()).Radians;
            this.UpdatePosition();
        }
        private DateTime LastPositionReadingTime = DateTime.Now;
        private bool startPositionCheck = false;
        protected virtual void UpdatePosition()
        {
            if (startPositionCheck)
            {
                if (DateTime.Now - LastPositionReadingTime >= TimeSpan.FromMilliseconds(1))
                {
                    this.DisplayableValueChanged();
                    for (int i = 0; i < subscribesToRobotPostionChange.Count; i++)
                    {
                        var subscriber = subscribesToRobotPostionChange[i];
                        LastPositionReadingTime = DateTime.Now;
                        subscriber.ReciveRobotPositionMemory(new PositionOccupiedByRobotMemory(this.FullRobotPosition(), this.Orientation, this.Position));
                    }
                }
            }
            else
            {
                startPositionCheck = true;
            }
        }
        List<ISubscribesToRobotPostionChange> subscribesToRobotPostionChange = new List<ISubscribesToRobotPostionChange>();
        public void SubscribeToRobotPositionChange(ISubscribesToRobotPostionChange subscribes)
        {
            subscribesToRobotPostionChange.Add(subscribes);
        }
        public void UnsubscribeToRobotPositionChange(ISubscribesToRobotPostionChange subscribes)
        {
            subscribesToRobotPostionChange.Remove(subscribes);
        }
        private Vector2d<double> LeftMostPoint()
        {
            Vector2d<double> left = null;
            foreach (var point in this.Shape)
            {
                if (left is null || point.x > left.x || (point.x == left.x && point.y > left.y))
                {
                    left = point;
                }
            }
            return left;
        }
        private Vector2d<double> RightMostPoint()
        {
            Vector2d<double> right = null;
            foreach (var point in this.Shape)
            {
                if (right is null || point.x < right.x || (point.x == right.x && point.y > right.y))
                {
                    right = point;
                }
            }
            return right;
        }
        public PositionedItemDisplayer GetDisplayer()
        {
            if (Displayer is null)
            {
                Displayer = new RobotDisplayer(this);
            }
            return Displayer;
        }

        public void DisplayableValueChanged()
        {
            if (!(Displayer is null))
            {
                Displayer.PosistionedItemValueChanged();
            }
        }

        public double LowestX()
        {
            var value = double.MaxValue;
            foreach (var pos in this.FullRobotPosition())
            {
                if (pos.x < value)
                    value = pos.x;
            }
            return value;
        }

        public double HighestX()
        {
            var value = double.MinValue;
            foreach (var pos in this.FullRobotPosition())
            {
                if (pos.x > value)
                    value = pos.x;
            }
            return value;
        }

        public double LowestY()
        {
            var value = double.MaxValue;
            foreach (var pos in this.FullRobotPosition())
            {
                if (pos.y < value)
                    value = pos.y;
            }
            return value;
        }

        public double HighestY()
        {
            var value = double.MinValue;
            foreach (var pos in this.FullRobotPosition())
            {
                if (pos.y > value)
                    value = pos.y;
            }
            return value;
        }
        public double HoroizontalRotationCenter()
        {
            var value = 0d;
            foreach (var pos in this.Shape)
            {
                if (pos.x > value)
                    value = pos.x;
            }
            return value;
        }
        public double VerticalRotationCenter()
        {
            var value = 0d;
            foreach (var pos in this.Shape)
            {
                if (pos.y > value)
                    value = pos.y;
            }
            return value;
        }
    }
}
