using RoboticNavigation.Robots;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RoboticNavigation.Display
{
    public class RobotDisplayer : PositionedItemDisplayer
    {
        private string ImageFileName;
        public RobotDisplayer(Robot item) : base(item)
        {
            this.ImageFileName = item.ImageFileName;
        }

        public override void ElementOnScreenPositionChanged()
        {
            var canvas = this.RootElement as Canvas;
            //var grid = canvas.Children[0] as Grid;
            //grid.Width = this.ItemPixelWidth();
            //grid.Height = this.ItemPixelHeight();


            var image = canvas.Children[0] as Image;
            image.Width = (this.DisplayedItem as Robot).Width * Scale;
            image.Height = (this.DisplayedItem as Robot).Height * Scale;
            var angle = (this.DisplayedItem as Robot).Orientation;
            RotateTransform rt = new RotateTransform(angle * (180 / Math.PI));
            rt.CenterX = ((this.DisplayedItem as Robot).Width / 2) * Scale;
            rt.CenterY = ((this.DisplayedItem as Robot).Height / 2) * Scale;
            image.RenderTransform = rt;

            Canvas.SetLeft(image, ((this.DisplayedItem as Robot).Position.x - (image.Width / 3) - Offset.HorizontalOffset) * Scale);
            Canvas.SetTop(image, ((this.DisplayedItem as Robot).Position.y - (image.Height / 3) - Offset.VerticalOffset) * Scale);


            //var poly = canvas.Children[1] as Polygon;
            //var points = new PointCollection();
            //foreach (var point in (this.DisplayedItem as Robot).FullRobotPosition())
            //{
            //    points.Add(new Point((point[0] - Offset.HorizontalOffset) * Scale, (point[1] - Offset.VerticalOffset) * Scale));
            //}
            //poly.Points = points;
        }
        public override void StartDisplaying()
        {
            var image = new Image();
            var path = System.IO.Path.Combine(Environment.CurrentDirectory);
            path = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, @"..\..\"));
            path = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, "Assets", ImageFileName));
            var uri = new Uri(path);
            image.Source = new BitmapImage(uri);
            image.Stretch = System.Windows.Media.Stretch.Fill;

            //var grid = new Grid();
            //grid.Children.Add(image);
            //grid.Background = new SolidColorBrush(Colors.Beige);

            var poly = new Polygon();
            poly.Fill = new SolidColorBrush(Color.FromArgb(200, 200, 200, 255));

            var canvas = new Canvas();
            canvas.Children.Add(image);
            canvas.Children.Add(poly);
            this.RootElement = canvas;


            Panel.SetZIndex(RootElement, 50);
            Panel.SetZIndex(poly, 1);
            Panel.SetZIndex(image, 2);
            base.StartDisplaying();
        }
    }

}
