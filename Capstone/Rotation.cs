using System;
using System.Numerics;

namespace Capstone
{
    public static class Rotation
    {
        public static Vector<double> Rotate(this Vector<double> vector, double radians)
        {
            var sin = Math.Sin(radians);
            var cos = Math.Cos(radians);
            return new Vector<double>(new double[] {vector[0] * cos - vector[1] * sin,
                vector[0] * sin + vector[1] * cos,
                0,
                0, });
        }
    }
}
