using System;

namespace RoboticNavigation.VectorMath
{
    public static class AngleMath
    {
        public static double SignedAngleDifference(this Double angleA, double angleB)
        {
            var result = angleA - angleB;
            result = (result + Math.PI).AngleMod(Math.PI * 2) - Math.PI;
            return result;
        }
        public static double AngleMod(this Double angle, double mod)
        {
            return (angle % mod + mod) % mod;
        }
    }
}
