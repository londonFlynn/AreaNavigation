using System;

namespace Capstone
{
    public static class LineTool
    {
        private static bool OnSegment(Vector2d<double> p, Vector2d<double> q, Vector2d<double> r)
        {
            if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
                q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
                return true;

            return false;
        }
        private static int Orientation(Vector2d<double> p, Vector2d<double> q, Vector2d<double> r)
        {
            var val = (q.y - p.y) * (r.x - q.x) -
                    (q.x - p.x) * (r.y - q.y);
            if (val == 0) return 0;

            return (val > 0) ? 1 : 2;
        }
        public static bool Intersect(Vector2d<double> p1, Vector2d<double> q1, Vector2d<double> p2, Vector2d<double> q2)
        {
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);
            bool result = false;
            result = result || (o1 != o2 && o3 != o4);
            result = result || (o1 == 0 && OnSegment(p1, p2, q1));
            result = result || (o2 == 0 && OnSegment(p1, q2, q1));
            result = result || (o3 == 0 && OnSegment(p2, p1, q2));
            result = result || (o4 == 0 && OnSegment(p2, q1, q2));
            return result;
        }
        public static double DistanceBetweenPointAndLine(Vector2d<double> point, Vector2d<double> p1, Vector2d<double> q1)
        {
            var lengthSqaured = Math.Pow(p1.x - q1.x, 2) + Math.Pow(p1.y - q1.y, 2);
            if (lengthSqaured == 0)
            {
                return Math.Sqrt(Math.Pow(p1.x - point.x, 2) + Math.Pow(p1.y - point.y, 2));
            }
            var t = Math.Max(0, Math.Min(1, (point - p1).Dot((q1 - p1)) / lengthSqaured));
            var projection = p1 + (q1 - p1) * t;
            return Math.Sqrt(Math.Pow(p1.x - projection.x, 2) + Math.Pow(p1.y - projection.y, 2));
        }
    }
}
