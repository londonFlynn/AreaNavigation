using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace RoboticNavigation.Surface
{
    public static class ObstacleSurfaceLoader
    {
        public static ImagedObstacleSurface Load(string filepath)
        {
            var bitmap = new Bitmap(filepath);
            var filename = Path.GetFileName(filepath);

            var sizeregex = new Regex(@"(\d+\.?\d*)CMPerPixel.*");
            var match = sizeregex.Match(filename);
            var cmPerPixelText = match.Groups[1].Value;
            var CMPerPixel = double.Parse(cmPerPixelText);
            return new ImagedObstacleSurface(bitmap, CMPerPixel);
        }
    }
}
