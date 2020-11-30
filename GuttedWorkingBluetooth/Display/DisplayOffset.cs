namespace RoboticNavigation.Display
{
    public class DisplayOffset
    {
        public readonly double HorizontalOffset;
        public readonly double VerticalOffset;
        public DisplayOffset(double hOffset, double vOffset)
        {
            this.HorizontalOffset = hOffset;
            this.VerticalOffset = vOffset;
        }
        public override bool Equals(object obj)
        {
            if (obj is DisplayOffset)
            {
                var that = obj as DisplayOffset;
                return this.HorizontalOffset == that.HorizontalOffset && this.VerticalOffset == that.VerticalOffset;
            }
            return false;
        }
    }
}
