using System.Windows.Media;

namespace Capstone
{
    public class ArcSegmentConfidence : ArcSegment
    {
        public double Confidence { get; set; }
        public const double ConfidenceTreshold = 0.75;
        public ArcSegmentConfidence(double angleInRadians, Vector2d<double> position, Vector2d<double> ray, double confidence) : base(angleInRadians, position, ray)
        {
            this.Confidence = confidence;
        }
        public override string ToString()
        {
            return $"Confidence: {Confidence}\r\nAngle: {this.AngleInRadians}\r\nPositon{this.Position}\r\nDistance{this.RaySegmant}";
        }




        //Display Stuff
        public override void StartDisplay()
        {
            base.StartDisplay();
            DisplayPath.Fill = new SolidColorBrush(Color.FromArgb(155, (byte)(this.Confidence < ConfidenceTreshold ? 255 : 0), 0, (byte)(this.Confidence < ConfidenceTreshold ? 0 : 255)));
            DisplayTriangle.Fill = DisplayPath.Fill;
            //Debug.WriteLine($"Displaying arc segmant {this}");
        }
        public override void UpdateDisplay()
        {
            base.UpdateDisplay();
            DisplayPath.Fill = new SolidColorBrush(Color.FromArgb(155, (byte)(this.Confidence < ConfidenceTreshold ? 255 : 0), 0, (byte)(this.Confidence < ConfidenceTreshold ? 0 : 255)));
            DisplayTriangle.Fill = DisplayPath.Fill;
            //Debug.WriteLine($"Updating arc segmant {this}");
        }

    }
}
