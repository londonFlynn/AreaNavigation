using RoboticNavigation.Display;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace RoboticNavigation.Surface
{
    public static class SurfaceImageGenerator
    {
        public static BitmapImage GenerateImage(this ObstacleSurface surface)
        {
            Bitmap map = new Bitmap(surface.Width, surface.Height);
            for (int x = 0; x < surface.Width; x++)
            {
                for (int y = 0; y < surface.Height; y++)
                {
                    var value = (surface.GetPixelValue(new SurfaceCoordinate(x, y)) + 1) / 2;
                    map.SetPixel(x, y, Color.FromArgb(255, (byte)(255 * value), (byte)(255 * value), (byte)(255 * value)));
                }
            }
            return map.ToBitmapImage();
        }
        public static BitmapImage GenerateImage(this ImagedObstacleSurface surface)
        {
            return surface.SurfaceBitmap.ToBitmapImage();
        }
    }
}
