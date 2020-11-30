using RoboticNavigation.NavNetworks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RoboticNavigation.Display
{
    public class NetworkPathDisplayer : PositionedItemDisplayer
    {
        public NetworkPathDisplayer(NetworkPath item) : base(item) { }

        public override void ElementOnScreenPositionChanged()
        {
            var canvas = this.RootElement as Canvas;
            var path = this.DisplayedItem as NetworkPath;
            if (path.Route.Count > 1)
            {
                for (int i = 1; i < path.Route.Count; i++)
                {

                    SetLine(canvas.Children[i - 1] as Line, path.Route[i - 1].Position, path.Route[i].Position);
                }
            }
        }
        private void SetLine(Line line, VectorMath.Vector2d<double> start, VectorMath.Vector2d<double> end)
        {
            var x1 = (start.x - Offset.HorizontalOffset) * Scale;
            var y1 = (start.y - Offset.VerticalOffset) * Scale;
            var x2 = (end.x - Offset.HorizontalOffset) * Scale;
            var y2 = (end.y - Offset.VerticalOffset) * Scale;
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = x2 - x1;
            //Y1 =?
            line.Y2 = y2 - y1;
            Canvas.SetLeft(line, x1);
            Canvas.SetTop(line, y1);
        }

        public override void StartDisplaying()
        {
            var canvas = new Canvas();
            var path = this.DisplayedItem as NetworkPath;
            for (int i = 1; i < path.Route.Count; i++)
            {
                var line = new Line();
                line.Stroke = new SolidColorBrush(Colors.LightBlue);
                canvas.Children.Add(line);
            }
            this.RootElement = canvas;
            base.StartDisplaying();
        }





        //public virtual void StartDisplay()
        //{
        //    Lines = new System.Windows.Shapes.Line[Route.Count - 1];
        //    for (int i = 1; i < Route.Count; i++)
        //    {
        //        StartDisplayingLine(Route[i - 1], Route[i], i - 1);
        //    }
        //}
        //private void StartDisplayingLine(NetworkNode start, NetworkNode end, int lineIndex)
        //{
        //    var line = new System.Windows.Shapes.Line();
        //    Lines[lineIndex] = line;
        //    line.Stroke = new SolidColorBrush(Colors.White);
        //    UpdateDisplayingLine(start, end, lineIndex);
        //    panel.Children.Add(line);
        //}
        //public virtual void UpdateDisplay()
        //{
        //    for (int i = 1; i < Route.Count; i++)
        //    {
        //        UpdateDisplayingLine(Route[i - 1], Route[i], i - 1);
        //    }
        //}
        //private void UpdateDisplayingLine(NetworkNode start, NetworkNode end, int lineIndex)
        //{
        //    var x1 = (start.Position.x - horizontalOffset) * scale;
        //    var y1 = (start.Position.y - verticalOffset) * scale;
        //    var x2 = (end.Position.x - horizontalOffset) * scale;
        //    var y2 = (end.Position.y - verticalOffset) * scale;
        //    var line = Lines[lineIndex];
        //    line.X1 = 0;
        //    line.Y1 = 0;
        //    line.X2 = x2 - x1;
        //    //Y1 =?
        //    line.Y2 = y2 - y1;
        //    Canvas.SetLeft(line, x1);
        //    Canvas.SetTop(line, y1);
        //}
    }
}
