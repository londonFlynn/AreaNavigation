using RoboticNavigation.ArcSegmants;
using RoboticNavigation.VectorMath;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RoboticNavigation.Display
{
    public class ArcConfidenceDisplayer : PositionedItemDisplayer
    {
        public ArcConfidenceDisplayer(ArcSegmentConfidence item) : base(item) { }

        public override void ElementOnScreenPositionChanged()
        {
            var canvas = this.RootElement as Canvas;
            var arc = this.DisplayedItem as ArcSegmants.ArcSegmentConfidence;
            var DisplayPath = canvas.Children[0] as System.Windows.Shapes.Path;
            DisplayPath.Fill = new SolidColorBrush(arc.Confidence < ArcSegmentConfidence.ConfidenceTreshold ? Colors.LightSalmon : Colors.LightSkyBlue);
            DisplayTriangle.Fill = DisplayPath.Fill;


            var startPoint = arc.Position + arc.RaySegmant;
            var endPoint = arc.Position + (arc.RaySegmant.Rotate(arc.AngleInRadians));
            var width = Math.Abs(startPoint.x - endPoint.x);
            var height = Math.Abs(startPoint.y - endPoint.y);
            var geometry = DisplayPath.Data;
            var figure = (geometry as PathGeometry).Figures[0];
            var arcSegmant = (figure as PathFigure).Segments[0] as System.Windows.Media.ArcSegment;
            figure.StartPoint = new System.Windows.Point((startPoint.x - Offset.HorizontalOffset) * Scale, (startPoint.y - Offset.VerticalOffset) * Scale);
            arcSegmant.IsLargeArc = Math.Abs(arc.AngleInRadians) > System.Math.PI;
            arcSegmant.SweepDirection = arc.AngleInRadians >= 0 ? System.Windows.Media.SweepDirection.Clockwise : System.Windows.Media.SweepDirection.Counterclockwise;
            arcSegmant.Point = new System.Windows.Point((endPoint.x - Offset.HorizontalOffset) * Scale, (endPoint.y - Offset.VerticalOffset) * Scale);
            arcSegmant.Size = new System.Windows.Size(arc.RaySegmant.Magnitude() * Scale, arc.RaySegmant.Magnitude() * Scale);
            //arcSegmant.Size = new System.Windows.Size(1, 1);
            arcSegmant.RotationAngle = arc.AngleInRadians * (180 / System.Math.PI);



            var points = new PointCollection();
            //points.Add(figure.StartPoint);
            //points.Add(arcSegmant.Point);
            //points.Add(new Point(0, 0));
            points.Add(new Point((arc.Position.x - Offset.HorizontalOffset) * Scale, (arc.Position.y - Offset.VerticalOffset) * Scale));
            points.Add(figure.StartPoint);
            points.Add(arcSegmant.Point);
            DisplayTriangle.Points = points;
            if (arc.AngleInRadians > Math.PI)
            {
                if (canvas.Children.Contains(DisplayTriangle))
                {
                    canvas.Children.Remove(DisplayTriangle);
                }
            }
            else
            {
                if (!canvas.Children.Contains(DisplayTriangle))
                {
                    canvas.Children.Add(DisplayTriangle);
                }
            }
        }
        System.Windows.Shapes.Polygon DisplayTriangle;
        public override void StartDisplaying()
        {
            var canvas = new Canvas();


            var displayArc = new System.Windows.Media.ArcSegment();
            var figure = new System.Windows.Media.PathFigure();
            figure.Segments.Add(displayArc);
            var geometry = new System.Windows.Media.PathGeometry();
            geometry.Figures.Add(figure);
            var displayPath = new System.Windows.Shapes.Path();
            displayPath.Data = geometry;
            displayPath.Fill = new SolidColorBrush(Color.FromArgb(155, 0, 0, 255));
            DisplayTriangle = new System.Windows.Shapes.Polygon();
            DisplayTriangle.Fill = displayPath.Fill;
            canvas.Children.Add(displayPath);
            //canvas.Children.Add(DisplayTriangle);

            this.RootElement = canvas;
            base.StartDisplaying();
        }



        ////Display functionality
        //protected System.Windows.Shapes.Path DisplayPath;
        //protected System.Windows.Shapes.Polygon DisplayTriangle;
        //public virtual void StartDisplay()
        //{
        //    if (!(this.panel is null))
        //    {
        //        var displayArc = new System.Windows.Media.ArcSegment();
        //        var figure = new System.Windows.Media.PathFigure();
        //        figure.Segments.Add(displayArc);
        //        var geometry = new System.Windows.Media.PathGeometry();
        //        geometry.Figures.Add(figure);
        //        DisplayPath = new System.Windows.Shapes.Path();
        //        DisplayPath.Data = geometry;
        //        DisplayPath.Fill = new SolidColorBrush(Color.FromArgb(155, 0, 0, 255));
        //        DisplayTriangle = new System.Windows.Shapes.Polygon();
        //        DisplayTriangle.Fill = DisplayPath.Fill;
        //        Canvas.SetZIndex(DisplayTriangle, 0);
        //        panel.Children.Add(DisplayPath);
        //        Canvas.SetZIndex(DisplayTriangle, 0);
        //        panel.Children.Add(DisplayTriangle);
        //        UpdateDisplay();
        //    }
        //}
        //public virtual void UpdateDisplay()
        //{
        //    var startPoint = Position + RaySegmant;
        //    var endPoint = Position + (RaySegmant.Rotate(AngleInRadians));
        //    var width = Math.Abs(startPoint.x - endPoint.x);
        //    var height = Math.Abs(startPoint.y - endPoint.y);
        //    var geometry = DisplayPath.Data;
        //    var figure = (geometry as PathGeometry).Figures[0];
        //    var arcSegmant = (figure as PathFigure).Segments[0] as System.Windows.Media.ArcSegment;
        //    figure.StartPoint = new System.Windows.Point((startPoint.x - horizontalOffset) * scale, (startPoint.y - verticalOffset) * scale);
        //    arcSegmant.IsLargeArc = Math.Abs(AngleInRadians) > System.Math.PI;
        //    arcSegmant.SweepDirection = AngleInRadians >= 0 ? System.Windows.Media.SweepDirection.Clockwise : System.Windows.Media.SweepDirection.Counterclockwise;
        //    arcSegmant.Point = new System.Windows.Point((endPoint.x - horizontalOffset) * scale, (endPoint.y - verticalOffset) * scale);
        //    arcSegmant.Size = new System.Windows.Size(RaySegmant.Magnitude() * scale, RaySegmant.Magnitude() * scale);
        //    //arcSegmant.Size = new System.Windows.Size(1, 1);
        //    arcSegmant.RotationAngle = AngleInRadians * (180 / System.Math.PI);

        //    var points = new PointCollection();
        //    //points.Add(figure.StartPoint);
        //    //points.Add(arcSegmant.Point);
        //    //points.Add(new Point(0, 0));
        //    points.Add(new Point((Position.x - horizontalOffset) * scale, (Position.y - verticalOffset) * scale));
        //    points.Add(figure.StartPoint);
        //    points.Add(arcSegmant.Point);
        //    DisplayTriangle.Points = points;
        //    if (AngleInRadians > Math.PI)
        //    {
        //        if (panel.Children.Contains(DisplayTriangle))
        //        {
        //            panel.Children.Remove(DisplayTriangle);
        //        }
        //    }
        //    else
        //    {
        //        if (!panel.Children.Contains(DisplayTriangle))
        //        {
        //            panel.Children.Add(DisplayTriangle);
        //        }
        //    }
        //}
        //public Canvas panel;
        //public double scale;
        //public double verticalOffset;
        //public double horizontalOffset;
        //public void SetPanel(System.Windows.Controls.Canvas panel)
        //{
        //    this.panel = panel;
        //}
        //public void SetScale(double scale, double horizontalOffset, double verticalOffset)
        //{
        //    this.scale = scale;
        //    this.horizontalOffset = horizontalOffset;
        //    this.verticalOffset = verticalOffset;
        //}
        //public virtual double LeftMostPosition()
        //{
        //    return double.MaxValue;
        //}
        //public virtual double RightMostPosition()
        //{
        //    return double.MinValue;
        //}
        //public virtual double TopMostPosition()
        //{
        //    return double.MaxValue;
        //}
        //public virtual double BottomMostPosition()
        //{
        //    return double.MinValue;
        //}
        //public double MaxHeight()
        //{
        //    return 0;
        //}
        //public double MaxWidth()
        //{
        //    return 0;
        //}
        //public void NotifyDisplayChanged()
        //{
        //    foreach (var listener in listeners)
        //    {
        //        listener.HearDisplayChanged();
        //    }
        //}
        //public List<ListenToDispalyChanged> listeners = new List<ListenToDispalyChanged>();
        //public void SubsricbeDisplayChanged(ListenToDispalyChanged listener)
        //{
        //    listeners.Add(listener);
        //}
        //public void UnsubsricbeDisplayChanged(ListenToDispalyChanged listener)
        //{
        //    listeners.Remove(listener);
        //}
        //public void UnsubsribeAll()
        //{
        //    listeners = new List<ListenToDispalyChanged>();
        //}
        //public void StopDisplaying()
        //{
        //    if (!(panel is null))
        //    {
        //        if (panel.Children.Contains(DisplayPath))
        //        {
        //            panel.Children.Remove(DisplayPath);
        //        }
        //        if (panel.Children.Contains(DisplayTriangle))
        //        {
        //            panel.Children.Remove(DisplayTriangle);
        //        }
        //    }
        //}
        //Display Stuff
        //public override void StartDisplay()
        //{
        //    base.StartDisplay();
        //    DisplayPath.Fill = new SolidColorBrush(Color.FromArgb(155, (byte)(this.Confidence < ConfidenceTreshold ? 255 : 0), 0, (byte)(this.Confidence < ConfidenceTreshold ? 0 : 255)));
        //    DisplayTriangle.Fill = DisplayPath.Fill;
        //    //Debug.WriteLine($"Displaying arc segmant {this}");
        //}
        //public override void UpdateDisplay()
        //{
        //    base.UpdateDisplay();
        //    DisplayPath.Fill = new SolidColorBrush(Color.FromArgb(155, (byte)(this.Confidence < ConfidenceTreshold ? 255 : 0), 0, (byte)(this.Confidence < ConfidenceTreshold ? 0 : 255)));
        //    DisplayTriangle.Fill = DisplayPath.Fill;
        //    //Debug.WriteLine($"Updating arc segmant {this}");
        //}


    }
}
