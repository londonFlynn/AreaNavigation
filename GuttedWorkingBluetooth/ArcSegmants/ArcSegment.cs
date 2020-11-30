using RoboticNavigation.VectorMath;

namespace RoboticNavigation.ArcSegmants
{
    public class ArcSegment
    {

        public double AngleInRadians { get; set; }
        public Vector2d<double> Position { get; set; }
        public Vector2d<double> RaySegmant { get; set; }

        public ArcSegment(double angleInRadians, Vector2d<double> position, Vector2d<double> ray)
        {
            this.AngleInRadians = angleInRadians;
            this.Position = position;
            this.RaySegmant = ray;
        }
        public double FinalAngleA { get { return RaySegmant.Angle(); } }
        public double FinalAngleB { get { return RaySegmant.Angle() + AngleInRadians; } }

    }

}
