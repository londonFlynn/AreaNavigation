using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Capstone
{
    public class ContructedMap : Map, ISensorReadingSubsriber
    {
        public List<RangeReading> RangeReadings = new List<RangeReading>();
        private Robot Robot;
        public ContructedMap(Robot robot)
        {
            this.Robot = robot;
            robot.USSensor.SubsribeToNewReadings(this);
            robot.IRSensor.SubsribeToNewReadings(this);
        }
        public void ReciveSensorReading(Sensor sensor)
        {
            if (!RangeReadings.Contains(sensor.GetCurrentReading() as RangeReading))
            {
                var top = this.TopMostPosition();
                var bottom = this.BottomMostPosition();
                var left = this.LeftMostPosition();
                var right = this.RightMostPosition();
                RangeReadings.Add(sensor.GetCurrentReading() as RangeReading);
                var newtop = this.TopMostPosition();
                var newbottom = this.BottomMostPosition();
                var newleft = this.LeftMostPosition();
                var newright = this.RightMostPosition();
                if (!(panel is null) && top == newtop && bottom == newbottom && left == newleft && right == newright)
                {
                    DisplayRangeReading(sensor.GetCurrentReading() as RangeReading);
                }
                else
                {
                    this.NotifyDisplayChanged();
                }
            }
        }
        private Panel panel;
        private double scale;
        private double hoffset;
        private double voffset;
        public override void Display(Panel panel, double scale, double horizontalOffset, double verticalOffset)
        {
            this.panel = panel;
            this.scale = scale;
            this.hoffset = horizontalOffset;
            this.voffset = verticalOffset;
            foreach (RangeReading reading in RangeReadings)
            {
                DisplayRangeReading(reading);
            }
            base.Display(panel, scale, horizontalOffset, verticalOffset);
        }
        private void DisplayRangeReading(RangeReading reading)
        {
            var line = new Line();
            line.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 15, 15, 15));
            line.Y1 = (reading.SensorPosition[1] - voffset) * scale;
            line.X1 = (reading.SensorPosition[0] - hoffset) * scale;
            line.Y2 = (reading.ReadingPosition[1] - voffset) * scale;
            line.X2 = (reading.ReadingPosition[0] - hoffset) * scale;
            panel.Children.Add(line);

            var circle = new Ellipse();
            circle.Fill = new SolidColorBrush(reading is UltrasonicRangeReading ? Windows.UI.Colors.Green : Windows.UI.Colors.Red);
            circle.Width = scale;
            circle.Height = scale;
            panel.Children.Add(circle);
            Canvas.SetTop(circle, (reading.ReadingPosition[1] - voffset) * scale - (scale / 2));
            Canvas.SetLeft(circle, (reading.ReadingPosition[0] - hoffset) * scale - (scale / 2));
        }
        public override double TopMostPosition()
        {
            double top = double.MaxValue;
            foreach (RangeReading reading in RangeReadings)
            {
                top = reading.SensorPosition[1] < top ? reading.SensorPosition[1] : top;
                top = reading.ReadingPosition[1] < top ? reading.ReadingPosition[1] : top;
            }
            return top < base.TopMostPosition() ? top : base.TopMostPosition();
        }
        public override double RightMostPosition()
        {
            double right = double.MinValue;
            foreach (RangeReading reading in RangeReadings)
            {
                right = reading.SensorPosition[0] > right ? reading.SensorPosition[0] : right;
                right = reading.ReadingPosition[0] > right ? reading.ReadingPosition[0] : right;
            }
            return right > base.RightMostPosition() ? right : base.RightMostPosition();
        }
        public override double LeftMostPosition()
        {
            double left = double.MaxValue;
            foreach (RangeReading reading in RangeReadings)
            {
                left = reading.SensorPosition[0] < left ? reading.SensorPosition[0] : left;
                left = reading.ReadingPosition[0] < left ? reading.ReadingPosition[0] : left;
            }
            return left < base.LeftMostPosition() ? left : base.LeftMostPosition();
        }
        public override double BottomMostPosition()
        {
            double bottom = double.MinValue;
            foreach (RangeReading reading in RangeReadings)
            {
                bottom = reading.SensorPosition[1] > bottom ? reading.SensorPosition[1] : bottom;
                bottom = reading.ReadingPosition[1] > bottom ? reading.ReadingPosition[1] : bottom;
            }
            return bottom > base.BottomMostPosition() ? bottom : base.BottomMostPosition();
        }
    }
}
