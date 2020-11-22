using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Capstone
{
    public abstract class Robot : ISensorReadingSubsriber, IDisplayable
    {
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
        public RoboticCommunication RoboticCommunication { get; protected set; }
        //represents the edges of the robot relative to its axis of rotation
        protected Vector2d<double>[] Shape;
        //protected Sensor[] Sensors;
        public GyroscopeSensor Gyro { get; protected set; }
        public RotationSensor LeftMotor { get; protected set; }
        public RotationSensor RightMotor { get; protected set; }
        public InfraredSensor IRSensor { get; protected set; }
        public UltrasonicSensor USSensor { get; protected set; }
        private MovementCommandState _movementCommandState = MovementCommandState.NEUTRAL;
        public MovementCommandState MovementCommandState
        {
            get { return _movementCommandState; }
            set
            {
                _movementCommandState = value;
                RoboticCommunication.CommandMove(_movementCommandState, 1);
            }
        }
        public Robot()
        {
            this.Position = new Vector2d<double>(new double[] { 0, 0, 0, 0 });
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
            this.UpdateDisplay();
            if (startPositionCheck)
            {
                if (DateTime.Now - LastPositionReadingTime >= TimeSpan.FromMilliseconds(1))
                    for (int i = 0; i < subscribesToRobotPostionChange.Count; i++)
                    {
                        var subscriber = subscribesToRobotPostionChange[i];
                        LastPositionReadingTime = DateTime.Now;
                        subscriber.ReciveRobotPositionMemory(new PositionOccupiedByRobotMemory(this.FullRobotPosition(), this.Orientation, this.Position));
                    }
            }
            else
            {
                startPositionCheck = true;
            }
            //Debug.WriteLine($"updating position: X={this.Position[0]}, Y={this.Position[1]}, Angle in Radians = {this.Orientation}");
            this.NotifyDisplayChanged();
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





        private Polygon DisplayedRobot;
        private Canvas panel;
        private double scale;
        private double verticalOffset;
        private double horizontalOffset;

        public virtual void StartDisplay()
        {
            DisplayedRobot = new Polygon();
            DisplayedRobot.Fill = new SolidColorBrush(Color.FromArgb(200, 200, 200, 255));
            UpdateDisplay();
            panel.Children.Add(DisplayedRobot);

            Debug.WriteLine("Starting to display robot");
            Canvas.SetZIndex(DisplayedRobot, 1);
        }
        public virtual void UpdateDisplay()
        {
            var points = new PointCollection();
            foreach (var point in this.FullRobotPosition())
            {
                points.Add(new Point((point[0] - horizontalOffset) * scale, (point[1] - verticalOffset) * scale));
            }
            DisplayedRobot.Points = points;
        }
        public double TopMostPosition()
        {
            double top = double.MaxValue;
            foreach (var point in this.FullRobotPosition())
            {
                if (point[1] < top)
                    top = point[1];
            }
            return top;
        }
        public double RightMostPosition()
        {
            double right = double.MinValue;
            foreach (var point in this.FullRobotPosition())
            {
                if (point[0] > right)
                    right = point[0];
            }
            return right;
        }
        public double LeftMostPosition()
        {
            double left = double.MaxValue;
            foreach (var point in this.FullRobotPosition())
            {
                if (point[0] < left)
                    left = point[0];
            }
            return left;
        }
        public double BottomMostPosition()
        {
            double bottom = double.MinValue;
            foreach (var point in this.FullRobotPosition())
            {
                if (point[1] > bottom)
                    bottom = point[1];
            }
            return bottom;
        }
        public double MaxHeight()
        {
            return BottomMostPosition() - TopMostPosition();
        }
        public double MaxWidth()
        {
            return RightMostPosition() - LeftMostPosition();
        }
        public void NotifyDisplayChanged()
        {
            foreach (var listener in listeners)
            {
                listener.HearDisplayChanged();
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
        public void SetPanel(System.Windows.Controls.Canvas panel)
        {
            this.panel = panel;
        }
        public void SetScale(double scale, double horizontalOffset, double verticalOffset)
        {
            this.scale = scale;
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
        }
        public void StopDisplaying()
        {
            panel.Children.Remove(DisplayedRobot);
        }
    }
}
