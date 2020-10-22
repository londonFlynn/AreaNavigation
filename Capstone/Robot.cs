using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Capstone
{
    public abstract class Robot : ISensorReadingSubsriber, IDisplayable
    {
        //represents the position of the axis of rotation
        public Vector<double> Position { get; protected set; }
        public double Orientation { get; protected set; }
        public RoboticCommunication RoboticCommunication { get; protected set; }
        public double CurrentSpeed { get; protected set; }
        public double CurrentRotationalSpeed { get; protected set; }
        //represents the edges of the robot relative to its axis of rotation
        protected Vector<double>[] Shape;
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
            this.Position = new Vector<double>(new double[] { 0, 0, 0, 0 });
        }
        public Vector<double>[] FullRobotPosition()
        {
            Vector<double>[] result = new Vector<double>[Shape.Length];
            double cos = Math.Cos(Orientation);
            double sin = Math.Sin(Orientation);
            for (int i = 0; i < Shape.Length; i++)
            {
                result[i] = new Vector<double>(new double[] {
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
            var movement = this.RightMotor.DistanceLastReading / 2;
            var moveVector = new Vector<double>(new double[] { 0, movement, 0, 0 });
            var angle = ((GyroscopeReading)this.Gyro.GetCurrentReading()).Radians;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            moveVector = new Vector<double>(new double[] { (moveVector[0] * cos) - (moveVector[1] * sin), (moveVector[0] * sin) + (moveVector[1] * cos), 0, 0 });
            this.Position = this.Position + moveVector;
            this.Orientation = angle;
            this.UpdatePosition();
        }

        private void reciveLeftMotorReading(RotationSensor rotationSensor)
        {
            var movement = this.LeftMotor.DistanceLastReading / 2;
            var moveVector = new Vector<double>(new double[] { 0, movement, 0, 0 });
            var angle = ((GyroscopeReading)this.Gyro.GetCurrentReading()).Radians;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            moveVector = new Vector<double>(new double[] { (moveVector[0] * cos) - (moveVector[1] * sin), (moveVector[0] * sin) + (moveVector[1] * cos), 0, 0 });
            this.Position = this.Position + moveVector;
            this.Orientation = angle;
            this.UpdatePosition();
        }

        private void ReciveGyroReading(GyroscopeSensor sensor)
        {
            this.Orientation = ((GyroscopeReading)this.Gyro.GetCurrentReading()).Radians;
            this.UpdatePosition();
        }
        protected abstract void UpdatePosition();






        private Polygon DisplayedRobot;
        public virtual void StartDisplay()
        {
            DisplayedRobot = new Polygon();
            DisplayedRobot.Fill = new SolidColorBrush(Windows.UI.Colors.LightBlue);
            UpdateDisplay();
            panel.Children.Add(DisplayedRobot);
        }
        public virtual void UpdateDisplay()
        {
            var points = new PointCollection();
            foreach (var point in this.FullRobotPosition())
            {
                points.Add(new Windows.Foundation.Point((point[0] - horizontalOffset) * scale, (point[1] - verticalOffset) * scale));
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

        private Panel panel;
        private double scale;
        private double verticalOffset;
        private double horizontalOffset;
        public void SetPanel(Panel panel)
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
