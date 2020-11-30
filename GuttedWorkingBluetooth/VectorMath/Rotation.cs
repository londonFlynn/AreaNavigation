using System;

namespace RoboticNavigation.VectorMath
{
    public static class Rotation
    {
        public static Vector2d<double> Rotate(this Vector2d<double> vector, double radians)
        {
            var sin = Math.Sin(radians);
            var cos = Math.Cos(radians);
            return new Vector2d<double>(new double[] {vector[0] * cos - vector[1] * sin,
                vector[0] * sin + vector[1] * cos,
                0,
                0, });
        }
    }
}
