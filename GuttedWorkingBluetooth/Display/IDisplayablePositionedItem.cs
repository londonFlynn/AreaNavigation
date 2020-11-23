namespace Capstone.Display
{
    public interface IDisplayablePositionedItem
    {
        PositionedItemDisplayer GetItemDisplayer();
        void OnDisplayableValueChanged();
        double LowestX();
        double HighestX();
        double LowestY();
        double HighestY();
    }
}
