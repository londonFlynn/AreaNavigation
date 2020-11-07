using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ListenToDispalyChanged
    {
        ProgramManager ProgramManager;
        List<IDisplayable> DisplayedItems = new List<IDisplayable>();
        public MainWindow()
        {
            this.InitializeComponent();
            this.ProgramManager = new ProgramManager(this);
            InputManager.Current.PreProcessInput += (sender, e) =>
            {
                if (e.StagingItem.Input is MouseButtonEventArgs)
                    GlobalClickEventHandler(sender,
                      (MouseButtonEventArgs)e.StagingItem.Input);
            };
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
            var windowWidth = this.Width;
            var windowHeight = this.Height;
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
        private Vector2d<double> GenerateOffset()
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
            return new Vector2d<double>(new double[] { leftMost, topMost, 0, 0 });
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
            //if (!(this.ProgramManager is null) && !(this.ProgramManager.Robot is null))
            //    this.ProgramManager.Robot.BringToFront();
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


        private void GlobalClickEventHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                DisplayPath(e);
            }
        }
        private async void DisplayPath(MouseButtonEventArgs e)
        {
            StopDisplayingPaths();
            var point = GetClickedPoint(e);
            //Debug.WriteLine(point);
            var path = await ProgramManager.PathFromRobotToPoint(point);
            //Debug.WriteLine(point);
            if (path is null)
            {
                Debug.WriteLine("There is not a path to that point");
            }
            else
            {
                Debug.WriteLine("Displaying Path");
                AddDisplayItem(path);
            }
        }
        private void StopDisplayingPaths()
        {
            for (int i = 0; i < DisplayedItems.Count; i++)
            {
                if (DisplayedItems[i] is NetworkPath)
                {
                    RemoveDisplayedItem(DisplayedItems[i]);
                    i--;
                }
            }
        }


        private Vector2d<double> GetClickedPoint(MouseButtonEventArgs e)
        {
            var point = e.GetPosition((this.FindName("MapCanvas") as Canvas));
            return new Vector2d<double>(new double[] { (point.X / recentScale) + recentHOffset, (point.Y / recentScale) + recentVOffset });
        }


        private bool UpIsPressed { get; set; }
        private bool DownIsPressed { get; set; }
        private bool LeftIsPressed { get; set; }
        private bool RightIsPressed { get; set; }
        private bool F1IsPressed { get; set; }
        private void UpdateMovmentState()
        {
            MovementCommandState state = MovementCommandState.NEUTRAL;
            if (UpIsPressed ^ DownIsPressed)
            {
                state = UpIsPressed ? MovementCommandState.FORWARD : MovementCommandState.REVERSE;
            }
            else if (LeftIsPressed ^ RightIsPressed)
            {
                state = LeftIsPressed ? MovementCommandState.LEFT : MovementCommandState.RIGHT;

            }
            this.ProgramManager.Robot.MovementCommandState = state;
        }
        void CoreWindow_KeyDown(object sender, KeyEventArgs e)
        {
            bool F1wasPressed = F1IsPressed;
            switch (e.Key)
            {
                case Key.Left:
                    LeftIsPressed = true;
                    break;
                case Key.Up:
                    UpIsPressed = true;
                    break;
                case Key.Right:
                    RightIsPressed = true;
                    break;
                case Key.Down:
                    DownIsPressed = true;
                    break;
                case Key.F1:
                    F1IsPressed = true;
                    break;
            }
            if (!F1wasPressed && F1IsPressed)
                ShowArcConfidenceSegments();
            UpdateMovmentState();
        }
        void CoreWindow_KeyUp(object sender, KeyEventArgs e)
        {
            bool F1WasPressed = F1IsPressed;
            switch (e.Key)
            {
                case Key.Left:
                    LeftIsPressed = false;
                    break;
                case Key.Up:
                    UpIsPressed = false;
                    break;
                case Key.Right:
                    RightIsPressed = false;
                    break;
                case Key.Down:
                    DownIsPressed = false;
                    break;
                case Key.F1:
                    F1IsPressed = false;
                    break;
            }
            if (F1WasPressed && !F1IsPressed)
            {
                HideArcConfidenceSegments();
            }
            UpdateMovmentState();
        }
        private void ShowArcConfidenceSegments()
        {
            //Debug.WriteLine("Showing arc segmants");
            //HideArcConfidenceSegments();
            if (!(ProgramManager.Robot.USSensor.GetCurrentReading() is null))
            {
                var arcs = ProgramManager.ContructedMap.ObstacleSurface.GetConfidenceArcSegmants((ProgramManager.Robot.USSensor.GetCurrentReading() as RangeReading).SensorPosition);
                //Debug.WriteLine($"Recived {arcs.Count} arc segmants");
                foreach (var arc in arcs)
                {

                    AddDisplayItem(arc);
                }
            }
        }
        private void HideArcConfidenceSegments()
        {
            //Debug.WriteLine("Hiding arc segmants");
            for (int i = 0; i < DisplayedItems.Count; i++)
            {
                if (DisplayedItems[i] is ArcSegmentConfidence)
                {
                    RemoveDisplayedItem(DisplayedItems[i]);
                    i--;
                }
            }
        }

    }
}
