using System.Numerics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Capstone
{
    public class SimulatedWall : Wall
    {
        public SimulatedWall(int range = 200)
        {
            var rand = new System.Random();
            this.StartPosition = new Vector<double>(new double[] { (rand.NextDouble() - 0.5) * range, (rand.NextDouble() - 0.5) * range, 0, 0 });
            this.EndPosition = new Vector<double>(new double[] { (rand.NextDouble() - 0.5) * range, (rand.NextDouble() - 0.5) * range, 0, 0 });
        }
        public override void Display(Panel panel, double scale, double horizontalOffset, double verticalOffset)
        {
            var line = new Line();
            line.Stroke = new SolidColorBrush(Windows.UI.Colors.Gray);
            line.X1 = (StartPosition[0] - horizontalOffset) * scale;
            line.Y1 = (StartPosition[1] - verticalOffset) * scale;
            line.X2 = (EndPosition[0] - horizontalOffset) * scale;
            line.Y1 = (EndPosition[1] - verticalOffset) * scale;
            panel.Children.Add(line);
        }
    }
}
