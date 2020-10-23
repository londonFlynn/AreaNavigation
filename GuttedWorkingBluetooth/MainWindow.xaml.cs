using System.Collections.Generic;
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

        //public MainPage()
        //{
        //    InitializeComponent();
        //    BrickSetup();
        //}
        //static async void BrickSetup()
        //{
        //    var coms = new Lego.Ev3.Desktop.BluetoothCommunication("COM3");
        //    var brick = new Brick(coms);
        //    brick.BrickChanged += OnBrickChanged;
        //    Debug.WriteLine("Attempting to connect to brick...");
        //    await brick.ConnectAsync();
        //    Debug.WriteLine("Brick connection successful");
        //    Debug.WriteLine(brick.Ports[InputPort.One].SIValue);
        //}
        //static void OnBrickChanged(object sender, BrickChangedEventArgs e)
        //{
        //    // print out the value of the sensor on Port 1 (more on this later...)
        //    Debug.WriteLine(e.Ports[InputPort.One].SIValue);
        //}
        ProgramManager ProgramManager;
        List<IDisplayable> DisplayedItems = new List<IDisplayable>();
        public MainWindow()
        {
            this.InitializeComponent();
            this.ProgramManager = new ProgramManager(this);
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

        private bool UpIsPressed { get; set; }
        private bool DownIsPressed { get; set; }
        private bool LeftIsPressed { get; set; }
        private bool RightIsPressed { get; set; }
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
            }
            UpdateMovmentState();
        }
        void CoreWindow_KeyUp(object sender, KeyEventArgs e)
        {
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
            }
            UpdateMovmentState();
        }

    }
}
