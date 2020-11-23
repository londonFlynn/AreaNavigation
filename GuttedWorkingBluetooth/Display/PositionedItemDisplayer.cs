using System.Windows;
using System.Windows.Controls;

namespace Capstone.Display
{
    public abstract class PositionedItemDisplayer
    {
        public IDisplayablePositionedItem DisplayedItem { get; private set; }
        public double Scale { get { return PositionalDisplayItemManager.Current.Scale; } }
        public DisplayOffset Offset { get { return PositionalDisplayItemManager.Current.DisplayOffset; } }
        public Canvas Canvas { get { return PositionalDisplayItemManager.Current.PositionedItemsCanvas; } }
        public UIElement RootElement { get; private set; }
        protected DisplayItemDimensions LastDimensions;
        public DimensionsChanged DimensionsChanged = delegate { };
        public bool IsDisplaying { get; private set; }

        public PositionedItemDisplayer(IDisplayablePositionedItem itemToBeDisplayed)
        {
            this.DisplayedItem = itemToBeDisplayed;
            this.LastDimensions = new DisplayItemDimensions(DisplayedItem);
        }
        public virtual void StartDisplaying()
        {
            IsDisplaying = true;
            PositionalDisplayItemManager.Current.AddItemDisplayer(this);
        }
        public void StopDisplaying()
        {
            IsDisplaying = false;
            PositionalDisplayItemManager.Current.RemoveItemDisplayer(this);
        }
        public virtual void PosistionedItemValueChanged()
        {
            var dimensions = new DisplayItemDimensions(DisplayedItem);
            if (dimensions != LastDimensions)
            {
                LastDimensions = dimensions;
                DimensionsChanged.DynamicInvoke();
            }
        }
        public double ItemWidth()
        {
            return DisplayedItem.HighestX() > DisplayedItem.LowestX() ? DisplayedItem.HighestX() - DisplayedItem.LowestX() : 0;
        }
        public double ItemPixelWidth()
        {
            return ItemWidth() * Scale;
        }
        public double ItemHeight()
        {
            return DisplayedItem.HighestY() > DisplayedItem.LowestY() ? DisplayedItem.HighestY() - DisplayedItem.LowestY() : 0;
        }
        public double ItemPixelHeight()
        {
            return ItemHeight() * Scale;
        }
        public abstract void ElementOnScreenPositionChanged();
    }
    public delegate void DimensionsChanged();
}
