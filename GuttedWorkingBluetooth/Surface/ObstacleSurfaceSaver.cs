using System;
using System.Windows.Media.Imaging;

namespace RoboticNavigation.Surface
{
    public static class ObstacleSurfaceSaver
    {
        public static string Save(this ObstacleSurface surface)
        {
            var image = surface.GenerateImage();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            var path = System.IO.Path.Combine(Environment.CurrentDirectory);
            path = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, @"..\..\"));
            string filename = $"{surface.CMPerPixel}CMPerPixel{DateTime.Now.ToString($"yyyy-dd-M--HH-mm-ss")}.png";
            path = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, "SavedObstacleSurfaces", filename));
            var uri = new Uri(path);
            using (var fileStream = new System.IO.FileStream(uri.AbsolutePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
            return filename;
        }
    }
}
