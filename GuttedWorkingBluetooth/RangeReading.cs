using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Capstone
{
    public class RangeReading : SensorReading, IDisplayable
    {
        public Vector2d<double> SensorPosition;
        public readonly Vector2d<double> DistanceVector;
        public readonly double Distance;
        public int TimesSampled = 0;
        public readonly double SensorFalloffDistance;
        public Vector2d<double> ReadingPosition
        {
            get
            {
                return SensorPosition + DistanceVector;
            }

        }
        public RangeReading(double distance, Vector2d<double> sensorPosition, double angle, double sensorFalloff)
        {
            this.SensorFalloffDistance = sensorFalloff;
            this.SensorPosition = sensorPosition;
            this.Distance = distance;
            var v = new Vector2d<double>(new double[] { 0, distance, 0, 0 });
            this.DistanceVector = v.Rotate(angle);
            //this.DistanceVector = new Vector<double>(new double[] { 0, distance, 0, 0 });
        }
        public override bool Equals(object obj)
        {
            if (obj is RangeReading)
            {
                RangeReading that = obj as RangeReading;
                return that.SensorPosition[0] == this.SensorPosition[0]
                    && that.SensorPosition[1] == this.SensorPosition[1]
                    && that.ReadingPosition[0] == this.ReadingPosition[0]
                    && that.ReadingPosition[1] == this.ReadingPosition[1];
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public static double ConfidenceFromConfidenceChange(double startingConfidence, double confidenceChange)
        {
            return ((1 - startingConfidence) * confidenceChange) + startingConfidence;
        }
        public double DistanceFromOtherReading(RangeReading that)
        {
            var thisPoint = this.ReadingPosition;
            var thatPoint = that.ReadingPosition;
            return Math.Sqrt(Math.Pow(thisPoint[0] - thatPoint[0], 2) + Math.Pow(thisPoint[1] - thatPoint[1], 2));
        }



        private Line DisplayedLine;
        private Ellipse DisplayedEllipse;
        public double BottomMostPosition()
        {
            return SensorPosition[1] > ReadingPosition[1] ? SensorPosition[1] : ReadingPosition[1];
        }

        public virtual void StartDisplay()
        {
            DisplayedLine = new Line();
            DisplayedLine.Stroke = new SolidColorBrush(Color.FromArgb(255, 15, 15, 15));
            DisplayedEllipse = new Ellipse();
            DisplayedEllipse.Fill = new SolidColorBrush(this is UltrasonicRangeReading ? Color.FromArgb(255, 0, 255, 0) : Color.FromArgb(255, 255, 0, 0));
            UpdateDisplay();
            panel.Children.Add(DisplayedLine);
            panel.Children.Add(DisplayedEllipse);
        }
        public virtual void UpdateDisplay()
        {
            DisplayedLine.Y1 = (SensorPosition[1] - verticalOffset) * scale;
            DisplayedLine.X1 = (SensorPosition[0] - horizontalOffset) * scale;
            DisplayedLine.Y2 = (ReadingPosition[1] - verticalOffset) * scale;
            DisplayedLine.X2 = (ReadingPosition[0] - horizontalOffset) * scale;

            DisplayedEllipse.Width = scale;
            DisplayedEllipse.Height = scale;
            System.Windows.Controls.Canvas.SetTop(DisplayedEllipse, (ReadingPosition[1] - verticalOffset) * scale - (scale / 2));
            System.Windows.Controls.Canvas.SetLeft(DisplayedEllipse, (ReadingPosition[0] - horizontalOffset) * scale - (scale / 2));
        }
        private System.Windows.Controls.Canvas panel;
        private double scale;
        private double verticalOffset;
        private double horizontalOffset;
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


        public double LeftMostPosition()
        {
            return SensorPosition[0] < ReadingPosition[0] ? SensorPosition[0] : ReadingPosition[0];
        }

        public double MaxHeight()
        {
            return BottomMostPosition() - TopMostPosition();
        }

        public double MaxWidth()
        {
            return RightMostPosition() - LeftMostPosition();
        }

        public double RightMostPosition()
        {
            return SensorPosition[0] > ReadingPosition[0] ? SensorPosition[0] : ReadingPosition[0];
        }

        public double TopMostPosition()
        {
            return SensorPosition[1] < ReadingPosition[1] ? SensorPosition[1] : ReadingPosition[1];
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
        public void StopDisplaying()
        {
            panel.Children.Remove(DisplayedLine);
        }
    }
}
