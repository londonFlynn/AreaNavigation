namespace RoboticNavigation.Display
{
    public interface IDisplayablePositionedItem
    {
        PositionedItemDisplayer GetDisplayer();
        void DisplayableValueChanged();
        double LowestX();
        double HighestX();
        double LowestY();
        double HighestY();
    }
}
