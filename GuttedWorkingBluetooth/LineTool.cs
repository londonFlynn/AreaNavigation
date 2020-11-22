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
        public static bool PointIsWithinPolygon(Vector2d<double> point, Vector2d<double>[] polygon)
        {
            if (polygon.Length < 3)
            {
                throw new ArgumentException("Polygon must have at least 3 verticies");
            }
            Vector2d<double> infinity = new Vector2d<double>(new double[] { point.x, double.MaxValue });
            int countOfCrossings = 0;
            for (int i = 0; i < polygon.Length; i++)
            {
                int next = i + 1 == polygon.Length ? 0 : i + 1;
                if (Intersect(point, infinity, polygon[i], polygon[next]))
                {
                    countOfCrossings++;
                }
            }
            return countOfCrossings % 2 == 1;
        }
        //public static double DistanceBetweenPointAndLine(Vector2d<double> point, Vector2d<double> p1, Vector2d<double> q1)
        //{
        //    var lengthSqaured = Math.Pow(p1.x - q1.x, 2) + Math.Pow(p1.y - q1.y, 2);
        //    if (lengthSqaured == 0)
        //    {
        //        return Math.Sqrt(Math.Pow(p1.x - point.x, 2) + Math.Pow(p1.y - point.y, 2));
        //    }
        //    var t = Math.Max(0, Math.Min(1, (point - p1).Dot((q1 - p1)) / lengthSqaured));
        //    var projection = p1 + (q1 - p1) * t;
        //    return Math.Sqrt(Math.Pow(p1.x - projection.x, 2) + Math.Pow(p1.y - projection.y, 2));
        //}
        public static double DistanceBetweenPointAndLine(Vector2d<double> E, Vector2d<double> A, Vector2d<double> B)
        {

            // vector AB 
            Vector2d<double> AB = B - A;

            // vector BP 
            Vector2d<double> BE = E - B;

            // vector AP 
            Vector2d<double> AE = E - A;

            // Variables to store dot product 
            double AB_BE, AB_AE;

            // Calculating the dot product 
            AB_BE = AB.Dot(BE);
            AB_AE = AB.Dot(AE);

            // Minimum distance from 
            // point E to the line segment 
            double reqAns = 0;

            // Case 1 
            if (AB_BE > 0)
            {
                reqAns = BE.Magnitude();
            }

            // Case 2 
            else if (AB_AE < 0)
            {
                reqAns = AE.Magnitude();
            }

            // Case 3 
            else
            {
                double mod = AB.Magnitude();
                reqAns = Math.Abs(AB.x * AE.y - AB.y * AE.x) / mod;
            }
            return reqAns;
        }
        public static double DistanceBetweenPoints(Vector2d<double> pointA, Vector2d<double> pointB)
        {
            return Math.Abs((pointA - pointB).Magnitude());
        }
    }
}
