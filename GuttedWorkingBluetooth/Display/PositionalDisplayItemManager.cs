using System.Collections.Generic;
using System.Windows.Controls;

namespace Capstone.Display
{
    public class PositionalDisplayItemManager
    {
        public static PositionalDisplayItemManager Current { get; set; }
        public double Scale { get; private set; }
        public DisplayOffset DisplayOffset { get; private set; }
        public Canvas PositionedItemsCanvas { get; private set; }
        public MainWindow MainWindow;
        private List<PositionedItemDisplayer> ItemDisplayers = new List<PositionedItemDisplayer>();

        public PositionalDisplayItemManager(Canvas itemCanvas, MainWindow mainWindow)
        {
            this.PositionedItemsCanvas = itemCanvas;
            this.MainWindow = mainWindow;
            Scale = GenerateScale();
            DisplayOffset = GenerateOffset();
            Current = this;
        }
        private double GenerateScale()
        {
            var windowWidth = MainWindow.Width;
            var windowHeight = MainWindow.Height;
            if (windowWidth == 0 || windowHeight == 0)
            {
                windowWidth = 1;
                windowHeight = 1;
            }
            double leftMost = double.MaxValue;
            double rightMost = double.MinValue;
            double topMost = double.MaxValue;
            double bottomMost = double.MinValue;
            double maxWidth = 0;
            double maxHeight = 0;
            foreach (var item in ItemDisplayers)
            {
                var itemLeft = item.DisplayedItem.LowestX();
                var itemRight = item.DisplayedItem.HighestX();
                var itemTop = item.DisplayedItem.LowestY();
                var itemBottom = item.DisplayedItem.HighestY();
                leftMost = itemLeft < leftMost ? itemLeft : leftMost;
                rightMost = itemRight > rightMost ? itemRight : rightMost;
                topMost = itemTop < topMost ? itemTop : topMost;
                bottomMost = itemBottom > bottomMost ? itemBottom : bottomMost;
                maxWidth = rightMost - leftMost;
                maxHeight = bottomMost - topMost;
            }
            var horizontalScale = windowWidth / maxWidth;
            var verticalScale = windowHeight / maxHeight;

            var scale = horizontalScale < verticalScale ? horizontalScale : verticalScale;
            return scale;


            throw new System.NotImplementedException();
        }
        private DisplayOffset GenerateOffset()
        {
            double topMost = double.MaxValue;
            double leftMost = double.MaxValue;
            foreach (var item in ItemDisplayers)
            {
                if (item.DisplayedItem.LowestY() < topMost)
                    topMost = item.DisplayedItem.LowestY();
                if (item.DisplayedItem.LowestX() < leftMost)
                    leftMost = item.DisplayedItem.LowestX();
            }
            return new DisplayOffset(leftMost, topMost);



        }
        public void AddItemDisplayer(PositionedItemDisplayer displayer)
        {
            if (!this.ItemDisplayers.Contains(displayer))
            {
                this.ItemDisplayers.Add(displayer);
                displayer.DimensionsChanged += DisplayItemDimensionsChanged;
                PositionedItemsCanvas.Children.Add(displayer.RootElement);
                DisplayItemDimensionsChanged();
            }
        }
        public void RemoveItemDisplayer(PositionedItemDisplayer displayer)
        {
            if (this.ItemDisplayers.Contains(displayer))
            {
                this.ItemDisplayers.Remove(displayer);
                displayer.DimensionsChanged -= DisplayItemDimensionsChanged;
                PositionedItemsCanvas.Children.Remove(displayer.RootElement);
                DisplayItemDimensionsChanged();
            }
        }
        public void DisplayItemDimensionsChanged()
        {
            var offset = GenerateOffset();
            var scale = GenerateScale();
            if (offset != DisplayOffset || Scale != scale)
            {
                this.Scale = scale;
                this.DisplayOffset = offset;
                UpdateAllItemsDisplayPosition();
            }
        }
        private void UpdateAllItemsDisplayPosition()
        {
            foreach (var item in ItemDisplayers)
            {
                item.ElementOnScreenPositionChanged();
            }
        }
        public void StopDisplayingItemsOfType<T>()
        {
            for (int i = 0; i < ItemDisplayers.Count; i++)
            {
                var item = ItemDisplayers[i];
                if (item is T)
                {
                    item.StopDisplaying();
                    i--;
                }
            }
        }

    }
}
