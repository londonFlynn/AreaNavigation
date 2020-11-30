using RoboticNavigation.Surface;
using System.Windows.Controls;

namespace RoboticNavigation.Display
{
    public class ObstacleSurfaceDisplayer : PositionedItemDisplayer
    {

        public ObstacleSurfaceDisplayer(ObstacleSurface item) : base(item) { }

        public override void ElementOnScreenPositionChanged()
        {
            var image = this.RootElement as Image;
            image.Source = (this.DisplayedItem as ObstacleSurface).GenerateImage();
            image.Width = ItemPixelWidth();
            image.Height = ItemPixelHeight();
        }

        public override void StartDisplaying()
        {
            var image = new Image();
            image.Source = (this.DisplayedItem as ObstacleSurface).GenerateImage();
            image.Stretch = System.Windows.Media.Stretch.UniformToFill;

            this.RootElement = image;

            base.StartDisplaying();
        }
        public override void PosistionedItemValueChanged()
        {
            base.PosistionedItemValueChanged();
        }
    }
}
