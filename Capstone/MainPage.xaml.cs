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
        public void AddDisplayItem(IDisplayable item)
        {
            DisplayedItems.Add(item);
            item.SubsricbeDisplayChanged(this);
            DisplayItems();
        }
        public void RemoveDisplayedItem(IDisplayable item)
        {
            DisplayedItems.Remove(item);
            item.UnsubsricbeDisplayChanged(this);
            DisplayItems();
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
        private void DisplayItems()
        {
            ClearCanvas();
            double scale = GenerateScale();
            var offset = GenerateOffset();
            double horizontalOffset = offset[0];
            double verticalOffset = offset[1];
            foreach (IDisplayable item in DisplayedItems)
            {
                item.Display(this.FindName("MapCanvas") as Canvas, scale, horizontalOffset, verticalOffset);
            }
        }
        private void ClearCanvas()
        {
            Canvas mapCanvas = this.FindName("MapCanvas") as Canvas;
            mapCanvas.Children.Clear();
        }
        private void CurrentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DisplayItems();
        }
        public void HearDisplayChanged()
        {
            //DisplayItems();
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
