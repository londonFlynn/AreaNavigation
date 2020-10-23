using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

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
            DisplayedLine.Stroke = new SolidColorBrush(Colors.White);
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
