using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Capstone
{
    public abstract class Wall : IDisplayable
    {
        public Vector2d<double> StartPosition;
        public Vector2d<double> EndPosition;



        private Line DisplayedLine;
        public virtual double BottomMostPosition()
        {
            return StartPosition[1] > EndPosition[1] ? StartPosition[1] : EndPosition[1];
        }

        public virtual void StartDisplay()
        {
            DisplayedLine = new Line();
            DisplayedLine.Stroke = new SolidColorBrush(Colors.White);
            UpdateDisplay();
            panel.Children.Add(DisplayedLine);
        }
        public virtual void UpdateDisplay()
        {
            if (!(DisplayedLine is null))
            {
                Debug.WriteLine(StartPosition);
                Debug.WriteLine(EndPosition);
                var x1 = (StartPosition[0] - horizontalOffset) * scale;
                //x1 = x1 > panel.ActualWidth ? panel.ActualWidth : x1 < 0 ? 0 : x1;
                var y1 = (StartPosition[1] - verticalOffset) * scale;
                //y1 = y1 > panel.ActualHeight ? panel.ActualHeight : y1 < 0 ? 0 : y1;
                var x2 = (EndPosition[0] - horizontalOffset) * scale;
                //x2 = x2 > panel.ActualWidth ? panel.ActualWidth : x1 < 0 ? 0 : x2;
                var y2 = (EndPosition[1] - verticalOffset) * scale;
                //y2 = y2 > panel.ActualHeight ? panel.ActualHeight : y2 < 0 ? 0 : y2;
                Debug.WriteLine($"({x1},{y1})");
                Debug.WriteLine($"({x2},{y2})");

                DisplayedLine.X1 = 0;
                DisplayedLine.Y1 = 0;
                DisplayedLine.X2 = x2 - x1;
                DisplayedLine.Y1 = y2 - y1;
                Canvas.SetLeft(DisplayedLine, x1);
                Canvas.SetTop(DisplayedLine, y1);
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
            return StartPosition[0] < EndPosition[0] ? StartPosition[0] : EndPosition[0];
        }

        public double MaxHeight()
        {
            return BottomMostPosition() - TopMostPosition();
        }

        public double MaxWidth()
        {
            return RightMostPosition() - LeftMostPosition();
        }

        public virtual double RightMostPosition()
        {
            return StartPosition[0] > EndPosition[0] ? StartPosition[0] : EndPosition[0];
        }

        public virtual double TopMostPosition()
        {
            return StartPosition[1] < EndPosition[1] ? StartPosition[1] : EndPosition[1];
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
            panel.Children.Remove(DisplayedLine);
        }
    }
}
