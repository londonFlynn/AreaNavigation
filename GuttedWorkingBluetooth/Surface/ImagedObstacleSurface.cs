using System;
using System.Drawing;

namespace RoboticNavigation.Surface
{
    public class ImagedObstacleSurface : ObstacleSurface
    {
        public Bitmap SurfaceBitmap { get; private set; }
        public override int Width => SurfaceBitmap.Width;
        public override int Height => SurfaceBitmap.Height;
        public ImagedObstacleSurface(Bitmap surfaceMap, double cmPerPixel)
        {
            this.CMPerPixel = cmPerPixel;
            if (surfaceMap is null)
            {
                throw new ArgumentNullException("the bitmap for the surface cannot be null");
            }
            this.SurfaceBitmap = surfaceMap;
        }
        public override double GetPixelValue(SurfaceCoordinate cell)
        {
            if (cell.HorizontalCoordinate < 0 || cell.HorizontalCoordinate >= Width || cell.VerticalCoorindate < 0 || cell.VerticalCoorindate >= Height)
            {
                return 0;
            }
            else
            {
                var pixel = SurfaceBitmap.GetPixel(cell.HorizontalCoordinate, cell.VerticalCoorindate);
                var average = pixel.R;
                var value = (average - 128d) / 128d;
                return value;
            }
        }

    }
}
