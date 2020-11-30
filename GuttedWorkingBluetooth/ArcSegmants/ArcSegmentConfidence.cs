using RoboticNavigation.Display;
using RoboticNavigation.VectorMath;

namespace RoboticNavigation.ArcSegmants
{
    public class ArcSegmentConfidence : ArcSegment, IDisplayablePositionedItem
    {
        public double Confidence { get; set; }
        public const double ConfidenceTreshold = 0.75;
        private ArcConfidenceDisplayer Displayer;
        public ArcSegmentConfidence(double angleInRadians, Vector2d<double> position, Vector2d<double> ray, double confidence) : base(angleInRadians, position, ray)
        {
            this.Confidence = confidence;
        }
        public override string ToString()
        {
            return $"Confidence: {Confidence}\r\nAngle: {this.AngleInRadians}\r\nPositon{this.Position}\r\nDistance{this.RaySegmant}";
        }

        public PositionedItemDisplayer GetDisplayer()
        {
            if (Displayer is null)
            {
                Displayer = new ArcConfidenceDisplayer(this);
            }
            return Displayer;
        }

        public void DisplayableValueChanged()
        {
            Displayer.PosistionedItemValueChanged();
        }

        public double LowestX()
        {
            return double.MaxValue;
        }

        public double HighestX()
        {
            return double.MinValue;
        }

        public double LowestY()
        {
            return double.MaxValue;
        }

        public double HighestY()
        {
            return double.MinValue;
        }
    }
}
