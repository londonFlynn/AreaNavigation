using System.Collections.Generic;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
            this.ProgramManager = new SimulationManager(this);
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
            double maxWidth = 0;
            double maxHeight = 0;
            foreach (IDisplayable item in DisplayedItems)
            {
                var width = item.MaxWidth();
                if (width > maxWidth)
                    maxWidth = width;
                var height = item.MaxHeight();
                if (height > maxHeight)
                    maxHeight = height;
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
                item.Display(this.FindName("MapCanvas") as Canvas, scale, verticalOffset, horizontalOffset);
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
            DisplayItems();
        }
        void CanvasKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Left)
            {
                ProgramManager.Robot.MovementCommandState = MovementCommandState.LEFT;
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                ProgramManager.Robot.MovementCommandState = MovementCommandState.RIGHT;
            }
            else if (e.Key == Windows.System.VirtualKey.Up)
            {
                ProgramManager.Robot.MovementCommandState = MovementCommandState.FORWARD;
            }
            else if (e.Key == Windows.System.VirtualKey.Down)
            {
                ProgramManager.Robot.MovementCommandState = MovementCommandState.REVERSE;
            }
        }
        void CanvasKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Up || e.Key == Windows.System.VirtualKey.Down || e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.Right)
            {
                ProgramManager.Robot.MovementCommandState = MovementCommandState.NEUTRAL;
            }
        }
    }
}
