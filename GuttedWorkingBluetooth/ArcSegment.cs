using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Capstone
{
    public class ArcSegment : IDisplayable
    {
        public double AngleInRadians { get; set; }
        public Vector2d<double> Position { get; set; }
        public Vector2d<double> RaySegmant { get; set; }
        public ArcSegment(double angleInRadians, Vector2d<double> position, Vector2d<double> ray)
        {
            this.AngleInRadians = angleInRadians;
            this.Position = position;
            this.RaySegmant = ray;
        }






        //Display functionality
        protected System.Windows.Shapes.Path DisplayPath;
        protected System.Windows.Shapes.Polygon DisplayTriangle;
        public virtual void StartDisplay()
        {
            var displayArc = new System.Windows.Media.ArcSegment();
            var figure = new System.Windows.Media.PathFigure();
            figure.Segments.Add(displayArc);
            var geometry = new System.Windows.Media.PathGeometry();
            geometry.Figures.Add(figure);
            DisplayPath = new System.Windows.Shapes.Path();
            DisplayPath.Data = geometry;
            DisplayPath.Fill = new SolidColorBrush(Color.FromArgb(155, 0, 0, 255));
            DisplayTriangle = new System.Windows.Shapes.Polygon();
            DisplayTriangle.Fill = DisplayPath.Fill;
            Canvas.SetZIndex(DisplayTriangle, 0);
            panel.Children.Add(DisplayPath);
            Canvas.SetZIndex(DisplayTriangle, 0);
            panel.Children.Add(DisplayTriangle);
            UpdateDisplay();
        }
        public virtual void UpdateDisplay()
        {
            var startPoint = Position + RaySegmant;
            var endPoint = Position + (RaySegmant.Rotate(AngleInRadians));
            var width = Math.Abs(startPoint.x - endPoint.x);
            var height = Math.Abs(startPoint.y - endPoint.y);
            var geometry = DisplayPath.Data;
            var figure = (geometry as PathGeometry).Figures[0];
            var arcSegmant = (figure as PathFigure).Segments[0] as System.Windows.Media.ArcSegment;
            figure.StartPoint = new System.Windows.Point((startPoint.x - horizontalOffset) * scale, (startPoint.y - verticalOffset) * scale);
            arcSegmant.IsLargeArc = Math.Abs(AngleInRadians) > System.Math.PI;
            arcSegmant.SweepDirection = AngleInRadians >= 0 ? System.Windows.Media.SweepDirection.Clockwise : System.Windows.Media.SweepDirection.Counterclockwise;
            arcSegmant.Point = new System.Windows.Point((endPoint.x - horizontalOffset) * scale, (endPoint.y - verticalOffset) * scale);
            arcSegmant.Size = new System.Windows.Size(RaySegmant.Magnitude() * scale, RaySegmant.Magnitude() * scale);
            //arcSegmant.Size = new System.Windows.Size(1, 1);
            arcSegmant.RotationAngle = AngleInRadians * (180 / System.Math.PI);

            var points = new PointCollection();
            //points.Add(figure.StartPoint);
            //points.Add(arcSegmant.Point);
            //points.Add(new Point(0, 0));
            points.Add(new Point((Position.x - horizontalOffset) * scale, (Position.y - verticalOffset) * scale));
            points.Add(figure.StartPoint);
            points.Add(arcSegmant.Point);
            DisplayTriangle.Points = points;
            if (AngleInRadians > Math.PI)
            {
                if (panel.Children.Contains(DisplayTriangle))
                {
                    panel.Children.Remove(DisplayTriangle);
                }
            }
            else
            {
                if (!panel.Children.Contains(DisplayTriangle))
                {
                    panel.Children.Add(DisplayTriangle);
                }
            }
        }
        protected Canvas panel;
        protected double scale;
        protected double verticalOffset;
        protected double horizontalOffset;
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
        public virtual double LeftMostPosition()
        {
            return double.MaxValue;
        }
        public virtual double RightMostPosition()
        {
            return double.MinValue;
        }
        public virtual double TopMostPosition()
        {
            return double.MaxValue;
        }
        public virtual double BottomMostPosition()
        {
            return double.MinValue;
        }
        public double MaxHeight()
        {
            return 0;
        }
        public double MaxWidth()
        {
            return 0;
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
        public void UnsubsribeAll()
        {
            listeners = new List<ListenToDispalyChanged>();
        }
        public void StopDisplaying()
        {
            panel.Children.Remove(DisplayPath);
            panel.Children.Remove(DisplayTriangle);
        }

    }

}
