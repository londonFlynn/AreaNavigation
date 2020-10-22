using System.Collections.Generic;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Capstone
{
    public abstract class Wall : IDisplayable
    {
        public Vector<double> StartPosition;
        public Vector<double> EndPosition;




        private Line DisplayedLine;
        public double BottomMostPosition()
        {
            return StartPosition[1] > EndPosition[1] ? StartPosition[1] : EndPosition[1];
        }

        public virtual void StartDisplay()
        {
            DisplayedLine = new Line();
            DisplayedLine.Stroke = new SolidColorBrush(Windows.UI.Colors.White);
            UpdateDisplay();
            panel.Children.Add(DisplayedLine);
        }
        public virtual void UpdateDisplay()
        {
            DisplayedLine.X1 = (StartPosition[0] - horizontalOffset) * scale;
            DisplayedLine.Y1 = (StartPosition[1] - verticalOffset) * scale;
            DisplayedLine.X2 = (EndPosition[0] - horizontalOffset) * scale;
            DisplayedLine.Y1 = (EndPosition[1] - verticalOffset) * scale;
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


        public double LeftMostPosition()
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

        public double RightMostPosition()
        {
            return StartPosition[0] > EndPosition[0] ? StartPosition[0] : EndPosition[0];
        }

        public double TopMostPosition()
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
        public void StopDisplaying()
        {
            panel.Children.Remove(DisplayedLine);
        }
    }
}
