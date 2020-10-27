using System.Collections.Generic;

namespace Capstone
{
    public abstract class Map : IDisplayable
    {
        public List<Wall> Walls = new List<Wall>();


        protected System.Windows.Controls.Canvas panel;
        protected double scale;
        protected double verticalOffset;
        protected double horizontalOffset;
        public virtual void StartDisplay()
        {
            foreach (Wall wall in Walls)
            {
                wall.StartDisplay();
            }
        }
        public virtual void UpdateDisplay()
        {
            foreach (Wall wall in Walls)
            {
                wall.UpdateDisplay();
            }
        }
        public virtual void SetPanel(System.Windows.Controls.Canvas panel)
        {
            this.panel = panel;
            foreach (Wall wall in Walls)
            {
                wall.SetPanel(panel);
            }
        }
        public virtual void SetScale(double scale, double horizontalOffset, double verticalOffset)
        {
            this.scale = scale;
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
            foreach (Wall wall in Walls)
            {
                wall.SetScale(scale, horizontalOffset, verticalOffset);
            }
        }
        public virtual double TopMostPosition()
        {
            double top = double.MaxValue;
            foreach (Wall wall in Walls)
            {
                if (wall.TopMostPosition() < top)
                    top = wall.TopMostPosition();
            }
            return top;
        }
        public virtual double RightMostPosition()
        {
            double right = double.MinValue;
            foreach (Wall wall in Walls)
            {
                if (wall.RightMostPosition() > right)
                    right = wall.RightMostPosition();
            }
            return right;
        }
        public virtual double LeftMostPosition()
        {
            double left = double.MaxValue;
            foreach (Wall wall in Walls)
            {
                if (wall.LeftMostPosition() < left)
                    left = wall.LeftMostPosition();
            }
            return left;
        }
        public virtual double BottomMostPosition()
        {
            double bottom = double.MinValue;
            foreach (Wall wall in Walls)
            {
                if (wall.BottomMostPosition() > bottom)
                    bottom = wall.BottomMostPosition();
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
        public virtual void StopDisplaying()
        {
            foreach (Wall wall in Walls)
            {
                wall.StopDisplaying();
            }
        }
    }
}
