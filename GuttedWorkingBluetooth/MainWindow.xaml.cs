using Microsoft.Win32;
using RoboticNavigation.Display;
using RoboticNavigation.MovementControls;
using RoboticNavigation.VectorMath;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticNavigation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Viewbox : Window
    {
        ProgramManager ProgramManager;
        PositionalDisplayItemManager DisplayItemManager;
        private System.Collections.Generic.Dictionary<Key, bool> KeyPressed = new System.Collections.Generic.Dictionary<Key, bool>();
        public Viewbox()
        {
            this.InitializeComponent();
            (this.FindName("ContainingBox") as Panel).Background = new SolidColorBrush(Color.FromArgb(255, 255 / 2, 255 / 2, 255 / 2));
            this.DisplayItemManager = new PositionalDisplayItemManager(this.FindName("MapCanvas") as Canvas, this.FindName("ContainingBox") as System.Windows.Controls.Panel);
            this.ProgramManager = new ProgramManager(this);

            InputManager.Current.PreProcessInput += (sender, e) =>
            {
                if (e.StagingItem.Input is MouseButtonEventArgs)
                    GlobalClickEventHandler(sender,
                      (MouseButtonEventArgs)e.StagingItem.Input);
            };
            SetupKeys();
        }
        private void SetupKeys()
        {
            KeyPressed[Key.Up] = false;
            KeyPressed[Key.Down] = false;
            KeyPressed[Key.Left] = false;
            KeyPressed[Key.Right] = false;
            KeyPressed[Key.F1] = false;
            KeyPressed[Key.F2] = false;
            KeyPressed[Key.F3] = false;
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
            }
        }

        private Vector2d<double> GetClickedPoint(MouseButtonEventArgs e)
        {
            var point = e.GetPosition((this.FindName("MapCanvas") as Canvas));
            return new Vector2d<double>(new double[] { (point.X / DisplayItemManager.Scale) + DisplayItemManager.DisplayOffset.HorizontalOffset, (point.Y / DisplayItemManager.Scale) + DisplayItemManager.DisplayOffset.VerticalOffset });
        }



        private void UpdateMovementState()
        {
            MovementDirection state = MovementDirection.NEUTRAL;
            if (KeyPressed[Key.Up] ^ KeyPressed[Key.Down])
            {
                state = KeyPressed[Key.Up] ? MovementDirection.FORWARD : MovementDirection.REVERSE;
            }
            else if (KeyPressed[Key.Left] ^ KeyPressed[Key.Right])
            {
                state = KeyPressed[Key.Left] ? MovementDirection.LEFT : MovementDirection.RIGHT;

            }
            Debug.WriteLine($"Set movement state to {state}");
            this.ProgramManager.Robot.MovementCommandState = state;
        }
        void CoreWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var wasPressed = false;
            if (KeyPressed.ContainsKey(e.Key))
                wasPressed = KeyPressed[e.Key];
            KeyPressed[e.Key] = true;
            if (e.Key == Key.F1 && !wasPressed)
                ShowArcConfidenceSegments();
            else if (e.Key == Key.F2 && !wasPressed)
                SaveObstacleSurface();
            else if (e.Key == Key.F3 && !wasPressed)
                LoadObstacleSurface();
            else if (!wasPressed && (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right))
            {
                UpdateMovementState();
            }
        }
        void CoreWindow_KeyUp(object sender, KeyEventArgs e)
        {
            var wasPressed = KeyPressed[e.Key];
            KeyPressed[e.Key] = false;

            if (e.Key == Key.F1 && wasPressed)
            {
                HideArcSegments();
            }
            if (wasPressed && (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right))
            {
                UpdateMovementState();
            }
        }
        private void LoadObstacleSurface()
        {
            Debug.WriteLine("Loading Obstacle Surface");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ProgramManager.LoadObstacleSurface(openFileDialog.FileName);
            }
        }
        private void SaveObstacleSurface()
        {
            Debug.WriteLine("Saving Obstacle Surface");
            ProgramManager.SaveObstacleSurface();
        }
        private void ShowArcConfidenceSegments()
        {
            ProgramManager.ShowArcConfidenceSegments();
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
