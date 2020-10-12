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

        public double BottomMostPosition()
        {
            return StartPosition[1] > EndPosition[1] ? StartPosition[1] : EndPosition[1];
        }

        public virtual void Display(Panel panel, double scale, double horizontalOffset, double verticalOffset)
        {
            var line = new Line();
            line.Stroke = new SolidColorBrush(Windows.UI.Colors.White);
            line.X1 = (StartPosition[0] - horizontalOffset) * scale;
            line.Y1 = (StartPosition[1] - verticalOffset) * scale;
            line.X2 = (EndPosition[0] - horizontalOffset) * scale;
            line.Y1 = (EndPosition[1] - verticalOffset) * scale;
            panel.Children.Add(line);
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

    }
}
