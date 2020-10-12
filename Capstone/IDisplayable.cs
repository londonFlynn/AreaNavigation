using Windows.UI.Xaml.Controls;

namespace Capstone
{
    public interface IDisplayable
    {
        void Display(Panel panel, double scale, double horizontalOffset, double verticalOffset);
        double MaxWidth();
        double MaxHeight();
        double LeftMostPosition();
        double RightMostPosition();
        double TopMostPosition();
        double BottomMostPosition();
        void NotifyDisplayChanged();
        void SubsricbeDisplayChanged(ListenToDispalyChanged listener);
        void UnsubsricbeDisplayChanged(ListenToDispalyChanged listener);
    }
}
