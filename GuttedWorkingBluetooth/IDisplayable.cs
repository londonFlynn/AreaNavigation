namespace Capstone
{
    public interface IDisplayable
    {
        void StartDisplay();
        void UpdateDisplay();
        void SetPanel(System.Windows.Controls.Canvas panel);
        void SetScale(double scale, double horizontalOffset, double verticalOffset);
        double MaxWidth();
        double MaxHeight();
        double LeftMostPosition();
        double RightMostPosition();
        double TopMostPosition();
        double BottomMostPosition();
        void NotifyDisplayChanged();
        void StopDisplaying();
        void SubsricbeDisplayChanged(ListenToDispalyChanged listener);
        void UnsubsricbeDisplayChanged(ListenToDispalyChanged listener);
    }
}
