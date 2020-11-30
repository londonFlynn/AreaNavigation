namespace RoboticNavigation.Display
{
    public class DisplayItemDimensions
    {
        public readonly double LowestX;
        public readonly double HighestX;
        public readonly double LowestY;
        public readonly double HighestY;
        public DisplayItemDimensions(IDisplayablePositionedItem item)
        {
            this.LowestX = item.LowestX();
            this.LowestY = item.LowestY();
            this.HighestX = item.HighestX();
            this.HighestY = item.HighestY();
        }
        public override bool Equals(object obj)
        {
            if (obj is DisplayItemDimensions)
            {
                var that = obj as DisplayItemDimensions;
                return this.LowestX == that.LowestX && this.LowestY == that.LowestY && this.HighestX == that.HighestX && this.HighestY == that.HighestY;
            }
            return false;
        }
    }
}
