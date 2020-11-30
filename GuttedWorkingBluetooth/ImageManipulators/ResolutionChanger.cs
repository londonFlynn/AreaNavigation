using System;
using System.Drawing;

namespace RoboticNavigation.ImageManipulators
{
    public static class ResolutionChanger
    {
        public static Bitmap ChangeResolution(this Bitmap bitmap, double ratio)
        {
            Bitmap resizedImg = new Bitmap(Convert.ToInt32(bitmap.Width * ratio), Convert.ToInt32(bitmap.Height * ratio));

            int newHeight = Convert.ToInt32(bitmap.Height * ratio);
            int newWidth = Convert.ToInt32(bitmap.Width * ratio);

            using (Graphics g = Graphics.FromImage(resizedImg))
            {
                g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
            }
            return resizedImg;
        }
    }
}
