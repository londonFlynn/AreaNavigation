using Capstone.Display;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Capstone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProgramManager ProgramManager;
        PositionalDisplayItemManager DisplayItemManager;
        public MainWindow()
        {
            this.InitializeComponent();
            this.DisplayItemManager = new PositionalDisplayItemManager(this.FindName("MapCanvas") as Canvas, this);
            this.ProgramManager = new ProgramManager(this);
            InputManager.Current.PreProcessInput += (sender, e) =>
            {
                if (e.StagingItem.Input is MouseButtonEventArgs)
                    GlobalClickEventHandler(sender,
                      (MouseButtonEventArgs)e.StagingItem.Input);
            };
        }

        private void CurrentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DisplayItemManager.DisplayItemDimensionsChanged();
        }


        private void GlobalClickEventHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ProgramManager.MoveRobotToPoint(GetClickedPoint(e));
                //DisplayPath(e);
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
        private void UpdateMovementState()
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
            Debug.WriteLine($"Set movement state to {state}");
            this.ProgramManager.Robot.MovementCommandState = state;
        }
        void CoreWindow_KeyDown(object sender, KeyEventArgs e)
        {
            bool F1wasPressed = F1IsPressed;
            bool UpWasPressed = UpIsPressed;
            bool DownWasPressed = DownIsPressed;
            bool LeftWasPressed = LeftIsPressed;
            bool RightWasPressed = RightIsPressed;
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
            if (UpWasPressed != UpIsPressed || DownWasPressed != DownIsPressed || LeftWasPressed != LeftIsPressed || RightWasPressed != RightIsPressed)
            {
                UpdateMovementState();
            }
        }
        void CoreWindow_KeyUp(object sender, KeyEventArgs e)
        {
            bool F1WasPressed = F1IsPressed;
            bool UpWasPressed = UpIsPressed;
            bool DownWasPressed = DownIsPressed;
            bool LeftWasPressed = LeftIsPressed;
            bool RightWasPressed = RightIsPressed;
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
                HideArcSegments();
            }
            if (UpWasPressed != UpIsPressed || DownWasPressed != DownIsPressed || LeftWasPressed != LeftIsPressed || RightWasPressed != RightIsPressed)
            {
                UpdateMovementState();
            }
        }
        private void ShowArcConfidenceSegments()
        {
            //Debug.WriteLine("Showing arc segmants");
            //HideArcConfidenceSegments();
            if (!(ProgramManager.Robot.USSensor.GetCurrentReading() is null))
            {
                var arcs = ProgramManager.ContructedMap.ObstacleSurface.GetConfidenceArcSegmants((ProgramManager.Robot.USSensor.GetCurrentReading() as RangeReading).SensorPosition, ProgramManager.Robot.USSensor.SensorFalloffDistance / 2);
                //Debug.WriteLine($"Recived {arcs.Count} arc segmants");
                foreach (var arc in arcs)
                {

                    arc.GetItemDisplayer().StartDisplaying();
                }
            }
        }
        public void HideArcSegments()
        {
            DisplayItemManager.StopDisplayingItemsOfType<ArcConfidenceDisplayer>();
        }
        public void HideMoveToDestinationPath()
        {
            //Debug.WriteLine("Hiding arc segmants");
            //for (int i = 0; i < DisplayedItems.Count; i++)
            //{
            //    if (DisplayedItems[i] is MoveToDestinationController)
            //    {
            //        RemoveDisplayedItem(DisplayedItems[i]);
            //        i--;
            //    }
            //}
        }

    }
}
