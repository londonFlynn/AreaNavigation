using System.Collections.Generic;
using System.Numerics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Capstone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, ListenToDispalyChanged
    {
        ProgramManager ProgramManager;
        List<IDisplayable> DisplayedItems = new List<IDisplayable>();
        public MainPage()
        {
            this.InitializeComponent();
            this.ProgramManager = new ProgramManager(this);
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
        }
        private double recentScale = 1;
        private double recentVOffset = 0;
        private double recentHOffset = 0;
        public void AddDisplayItem(IDisplayable item)
        {
            DisplayedItems.Add(item);
            item.SubsricbeDisplayChanged(this);
            item.SetPanel(this.FindName("MapCanvas") as Canvas);
            item.SetScale(recentScale, recentHOffset, recentVOffset);
            item.StartDisplay();
            HearDisplayChanged();
        }
        public void RemoveDisplayedItem(IDisplayable item)
        {
            DisplayedItems.Remove(item);
            item.UnsubsricbeDisplayChanged(this);
            item.StopDisplaying();
            HearDisplayChanged();
        }
        private double GenerateScale()
        {
            var windowWidth = ((Frame)Window.Current.Content).ActualWidth;
            var windowHeight = ((Frame)Window.Current.Content).ActualHeight;
            if (windowWidth == 0 || windowHeight == 0)
            {
                windowWidth = 1;
                windowHeight = 1;
            }
            double leftMost = double.MaxValue;
            double rightMost = double.MinValue;
            double topMost = double.MaxValue;
            double bottomMost = double.MinValue;
            double maxWidth = 0;
            double maxHeight = 0;
            foreach (IDisplayable item in DisplayedItems)
            {
                var itemLeft = item.LeftMostPosition();
                var itemRight = item.RightMostPosition();
                var itemTop = item.TopMostPosition();
                var itemBottom = item.BottomMostPosition();
                leftMost = itemLeft < leftMost ? itemLeft : leftMost;
                rightMost = itemRight > rightMost ? itemRight : rightMost;
                topMost = itemTop < topMost ? itemTop : topMost;
                bottomMost = itemBottom > bottomMost ? itemBottom : bottomMost;
                maxWidth = rightMost - leftMost;
                maxHeight = bottomMost - topMost;
            }
            var horizontalScale = windowWidth / maxWidth;
            var verticalScale = windowHeight / maxHeight;

            var scale = horizontalScale < verticalScale ? horizontalScale : verticalScale;
            return scale;
        }
        private Vector<double> GenerateOffset()
        {
            double topMost = double.MaxValue;
            double leftMost = double.MaxValue;
            foreach (IDisplayable item in DisplayedItems)
            {
                if (item.TopMostPosition() < topMost)
                    topMost = item.TopMostPosition();
                if (item.LeftMostPosition() < leftMost)
                    leftMost = item.LeftMostPosition();
            }
            return new Vector<double>(new double[] { leftMost, topMost, 0, 0 });
        }
        private void UpdateDisplay()
        {
            foreach (IDisplayable item in DisplayedItems)
            {
                item.SetScale(recentScale, recentHOffset, recentVOffset);
                item.UpdateDisplay();
            }
        }
        private void CurrentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HearDisplayChanged();
        }
        public void HearDisplayChanged()
        {
            var offset = GenerateOffset();
            var scale = GenerateScale();
            if (offset[0] != recentHOffset || offset[1] != recentVOffset || recentScale != scale)
            {
                this.recentScale = scale;
                this.recentVOffset = offset[1];
                this.recentHOffset = offset[0];
                UpdateDisplay();
            }
        }

        private bool UpIsPressed()
        {
            var keyState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Up);
            return (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        private bool DownIsPressed()
        {
            var keyState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Down);
            return (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        private bool LeftIsPressed()
        {
            var keyState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Left);
            return (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        private bool RightIsPressed()
        {
            var keyState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Right);
            return (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
        private void UpdateMovmentState()
        {
            MovementCommandState state = MovementCommandState.NEUTRAL;
            if (UpIsPressed() ^ DownIsPressed())
            {
                state = UpIsPressed() ? MovementCommandState.FORWARD : MovementCommandState.REVERSE;
            }
            else if (LeftIsPressed() ^ RightIsPressed())
            {
                state = LeftIsPressed() ? MovementCommandState.LEFT : MovementCommandState.RIGHT;

            }
            this.ProgramManager.Robot.MovementCommandState = state;
        }
        void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {

            if (IsArrowKey(e.VirtualKey))
                UpdateMovmentState();
        }
        void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (IsArrowKey(e.VirtualKey))
                UpdateMovmentState();
        }
        private bool IsArrowKey(VirtualKey key)
        {
            return (key == VirtualKey.Up || key == VirtualKey.Down || key == VirtualKey.Left || key == VirtualKey.Right);
        }
    }
}
