using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Capstone
{
    public abstract class Robot : IDisplayable
    {
        //represents the position of the axis of rotation
        public Vector<double> Position { get; protected set; }
        public double Orientation { get; protected set; }
        public RoboticCommunication RoboticCommunication { get; protected set; }
        public double CurrentSpeed { get; protected set; }
        public double CurrentRotationalSpeed { get; protected set; }
        //represents the edges of the robot relative to its axis of rotation
        protected Vector<double>[] Shape;
        protected Sensor[] sensors;

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
        public void Display(Panel panel, double scale, double horizontalOffset, double verticalOffset)
        {
            var pos = FullRobotPosition();
            var polygon = new Polygon();
            polygon.Fill = new SolidColorBrush(Windows.UI.Colors.LightBlue);

            var points = new PointCollection();
            foreach (var point in pos)
            {
                points.Add(new Windows.Foundation.Point((point[0] - horizontalOffset) * scale, (point[1] - verticalOffset) * scale));

            }
            polygon.Points = points;
            panel.Children.Add(polygon);
        }
        public double TopMostPosition()
        {
            double top = double.MaxValue;
            foreach (var v in FullRobotPosition())
            {
                if (v[1] < top)
                    top = v[1];
            }
            return top;
        }
        public double RightMostPosition()
        {
            double right = double.MinValue;
            foreach (var v in FullRobotPosition())
            {
                if (v[1] > right)
                    right = v[1];
            }
            return right;
        }
        public double LeftMostPosition()
        {
            double left = double.MaxValue;
            foreach (var v in FullRobotPosition())
            {
                if (v[1] < left)
                    left = v[1];
            }
            return left;
        }
        public double BottomMostPosition()
        {
            double bottom = double.MinValue;
            foreach (var v in FullRobotPosition())
            {
                if (v[1] > bottom)
                    bottom = v[1];
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
    }
}
