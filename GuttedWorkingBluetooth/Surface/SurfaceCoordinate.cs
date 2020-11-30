namespace RoboticNavigation.Surface
{
    public class SurfaceCoordinate
    {
        public int VerticalCoorindate;
        public int HorizontalCoordinate;
        public SurfaceCoordinate(int horizontalCoordinate, int verticalCoordinate)
        {
            this.VerticalCoorindate = verticalCoordinate;
            this.HorizontalCoordinate = horizontalCoordinate;
        }
        public override bool Equals(object obj)
        {
            if (obj is SurfaceCoordinate)
            {
                var that = obj as SurfaceCoordinate;
                return VerticalCoorindate == that.VerticalCoorindate && HorizontalCoordinate == that.HorizontalCoordinate;
            }
            else
            {
                return false;
            }
        }
    }
}
