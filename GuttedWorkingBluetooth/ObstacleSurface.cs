using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Capstone
{
    public class ObstacleSurface : IDisplayable
    {
        private double CMPerPixel { get; set; }
        private double[][] PixelMapping { get; set; }
        public ObstacleSurface(double cmPerPixel, int resolutionWidth, int resolutionHeight)
        {
            this.CMPerPixel = cmPerPixel;
            this.PixelMapping = new double[resolutionWidth][];
            for (int i = 0; i < PixelMapping.Length; i++)
            {
                PixelMapping[i] = new double[resolutionHeight];
            }
        }
        public const double ReadingRadius = 6.5;
        public void MatchToRangeReading(RangeReading reading, double amount)
        {
            //TODO Smooth trasitioning
            var point = reading.ReadingPosition;
            var increaseCell = CoordinateForPoint(point);
            var increease = CellsWithinRadiusOfPoint(point, ReadingRadius);
            var decrease = CellsWithinDistanceOfLineSegmant(point, reading.SensorPosition, 0);
            decrease.Remove(increaseCell);
            if (reading.Distance < 120)
            {
                foreach (var cell in increease)
                {
                    var value = GetPixelValue(cell);
                    var distance = DistanceBetweenGridCellAndPoint(cell, point);
                    var distanceModifier = -(distance - ReadingRadius) / ReadingRadius;
                    var valueDistanceFrom1 = 1 - value;
                    var valueIncrease = valueDistanceFrom1 * distanceModifier * amount;
                    SetPixelValue(cell, value + valueIncrease);
                    SetDisplayColor(cell.HorizontalCoordinate, cell.VerticalCoorindate);
                }
            }
            foreach (var cell in decrease)
            {
                SetPixelValue(cell, -1);
                //Decrease Confidence based on the distance from the sensor, when reading is long range
                //get an area around the line to change to confidence of

                SetDisplayColor(cell.HorizontalCoordinate, cell.VerticalCoorindate);
            }
            Debug.WriteLine($"Changed {increease.Count + decrease.Count} cells");
        }
        public void MatchToAreaEmptyReading(AreaEmptyReading reading, double amount)
        {
            //TODO
            throw new System.NotImplementedException();
        }
        private List<SurfaceCoordinate> CellsWithinRadiusOfPoint(Vector2d<double> point, double radius)
        {
            var startCell = CoordinateForPoint(point);
            var cellList = new List<SurfaceCoordinate>();
            CellsWithinRadiusOfPointRecursion(startCell, cellList, point, radius);
            return cellList;
        }
        private void CellsWithinRadiusOfPointRecursion(SurfaceCoordinate checkCell, List<SurfaceCoordinate> containedCells, Vector2d<double> point, double radius)
        {
            if (!containedCells.Contains(checkCell) && DistanceBetweenGridCellAndPoint(checkCell, point) <= radius)
            {
                containedCells.Add(checkCell);
                var nextCellsToCheck = new List<SurfaceCoordinate>();
                if (checkCell.HorizontalCoordinate > 0)
                {
                    nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate - 1, checkCell.VerticalCoorindate));
                }
                if (checkCell.HorizontalCoordinate < PixelMapping.Length - 1)
                {
                    nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate + 1, checkCell.VerticalCoorindate));

                }
                if (checkCell.VerticalCoorindate > 0)
                {
                    nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate - 1));

                }
                if (checkCell.VerticalCoorindate < PixelMapping[0].Length - 1)
                {
                    nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate + 1));

                }
                foreach (var next in nextCellsToCheck)
                {
                    CellsWithinRadiusOfPointRecursion(next, containedCells, point, radius);
                }
            }
        }
        private void SetPixelValue(SurfaceCoordinate cell, double value)
        {
            PixelMapping[cell.HorizontalCoordinate][cell.VerticalCoorindate] = value;
        }
        private double GetPixelValue(SurfaceCoordinate cell)
        {
            return PixelMapping[cell.HorizontalCoordinate][cell.VerticalCoorindate];
        }
        private double DistanceBetweenGridCellAndPoint(SurfaceCoordinate cell, Vector2d<double> point)
        {
            var bounding = CellBoundingRange(cell);
            var minBounding = bounding[0];
            var maxBounding = bounding[1];
            var dx = Math.Max(0, Math.Max(minBounding[0] - point[0], point[0] - maxBounding[0]));
            var dy = Math.Max(0, Math.Max(minBounding[1] - point[1], point[1] - maxBounding[1]));
            return Math.Sqrt((dx * dx) + (dy * dy));
        }
        private double DistanceBetweenGridCellAndLineSegmant(SurfaceCoordinate cell, Vector2d<double> point1, Vector2d<double> point2)
        {
            var intersect = LineIntersectsCell(cell, point1, point2);
            if (intersect)
                return 0;
            else
                return 1;
            //return Math.Min(Math.Min(LineTool.DistanceBetweenPointAndLine(minBounding, point1, point2), LineTool.DistanceBetweenPointAndLine(maxBounding, point1, point2)), Math.Min(LineTool.DistanceBetweenPointAndLine(otherCorner1, point1, point2), LineTool.DistanceBetweenPointAndLine(otherCorner2, point1, point2)));
        }
        private bool LineIntersectsCell(SurfaceCoordinate cell, Vector2d<double> point1, Vector2d<double> point2)
        {
            var bounding = CellBoundingRange(cell);
            var minBounding = bounding[0];
            var maxBounding = bounding[1];
            var otherCorner1 = bounding[2];
            var otherCorner2 = bounding[3];
            return LineTool.Intersect(minBounding, otherCorner1, point1, point2) ||
                LineTool.Intersect(otherCorner1, maxBounding, point1, point2) ||
                LineTool.Intersect(maxBounding, otherCorner2, point1, point2) ||
                LineTool.Intersect(otherCorner2, minBounding, point1, point2);
        }
        private List<SurfaceCoordinate> CellsWithinDistanceOfLineSegmant(Vector2d<double> point1, Vector2d<double> point2, double radius)
        {
            var startCell = CoordinateForPoint(point1);
            var cellList = new List<SurfaceCoordinate>();
            CellsWithinDistanceOfLineSegmantRecursion(startCell, cellList, point1, point2, radius);
            return cellList;
        }
        private void CellsWithinDistanceOfLineSegmantRecursion(SurfaceCoordinate checkCell, List<SurfaceCoordinate> containedCells, Vector2d<double> point1, Vector2d<double> point2, double radius)
        {
            if (!containedCells.Contains(checkCell) && DistanceBetweenGridCellAndLineSegmant(checkCell, point1, point2) <= radius)
            {
                containedCells.Add(checkCell);
                var nextCellsToCheck = new List<SurfaceCoordinate>();
                if (checkCell.HorizontalCoordinate > 0)
                {
                    nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate - 1, checkCell.VerticalCoorindate));
                }
                if (checkCell.HorizontalCoordinate < PixelMapping.Length - 1)
                {
                    nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate + 1, checkCell.VerticalCoorindate));

                }
                if (checkCell.VerticalCoorindate > 0)
                {
                    nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate - 1));

                }
                if (checkCell.VerticalCoorindate < PixelMapping[0].Length - 1)
                {
                    nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate + 1));

                }
                foreach (var next in nextCellsToCheck)
                {
                    CellsWithinDistanceOfLineSegmantRecursion(next, containedCells, point1, point2, radius);
                }
            }
        }
        private Vector2d<double>[] CellBoundingRange(SurfaceCoordinate cell)
        {
            Vector2d<double>[] result = new Vector2d<double>[4];
            result[0] = new Vector2d<double>(new double[] { (cell.HorizontalCoordinate - (PixelMapping.Length / 2)) * CMPerPixel, (cell.VerticalCoorindate - (PixelMapping[0].Length / 2)) * CMPerPixel });
            result[1] = new Vector2d<double>(new double[] { result[0][0] + CMPerPixel - double.Epsilon, result[0][1] + CMPerPixel - double.Epsilon });
            result[2] = new Vector2d<double>(new double[] { result[0][0], result[1][1] });
            result[3] = new Vector2d<double>(new double[] { result[1][0], result[0][1] });
            return result;
        }
        private SurfaceCoordinate CoordinateForPoint(Vector2d<double> point)
        {
            return new SurfaceCoordinate(
                (int)Math.Floor(point[0] / CMPerPixel) + (PixelMapping.Length / 2),
                (int)Math.Floor(point[1] / CMPerPixel) + (PixelMapping[0].Length / 2));
        }
        private SurfaceCoordinate[] CoordinatesForLine(Vector2d<double> start, Vector2d<double> end)
        {
            List<SurfaceCoordinate> coordinates = new List<SurfaceCoordinate>();
            coordinates.Add(CoordinateForPoint(start));
            var endCoordinate = CoordinateForPoint(end);
            if (!coordinates.Contains(endCoordinate))
            {
                coordinates.Add(endCoordinate);
            }
            var VectorIncriment = start - end;
            VectorIncriment = VectorIncriment / CMPerPixel;
            var checkVector = start + VectorIncriment;
            while (PointIsBetweenPoints(checkVector, start, end))
            {
                var coords = CoordinateForPoint(checkVector);
                if (!coordinates.Contains(coords))
                {
                    coordinates.Add(coords);
                }
                checkVector += VectorIncriment;
            }
            return coordinates.ToArray();
        }
        private bool PointIsBetweenPoints(Vector2d<double> checkPoint, Vector2d<double> start, Vector2d<double> end)
        {
            var isBetween = true;
            if (start[0] < end[0])
            {
                isBetween = isBetween && checkPoint[0] > start[0] && checkPoint[0] < end[0];
            }
            else if (start[0] > end[0])
            {
                isBetween = isBetween && checkPoint[0] < start[0] && checkPoint[0] > end[0];
            }
            if (start[1] < end[1])
            {
                isBetween = isBetween && checkPoint[1] > start[1] && checkPoint[1] < end[1];
            }
            else if (start[1] > end[1])
            {
                isBetween = isBetween && checkPoint[1] < start[1] && checkPoint[1] > end[1];
            }
            return isBetween;
        }






        //Display functionality
        private System.Windows.Shapes.Rectangle[][] PixelDisplay;
        public virtual void StartDisplay()
        {
            PixelDisplay = new System.Windows.Shapes.Rectangle[this.PixelMapping.Length][];
            for (int i = 0; i < PixelDisplay.Length; i++)
            {
                PixelDisplay[i] = new System.Windows.Shapes.Rectangle[this.PixelMapping[0].Length];
                for (int j = 0; j < PixelDisplay[i].Length; j++)
                {
                    PixelDisplay[i][j] = new System.Windows.Shapes.Rectangle();
                    panel.Children.Add(PixelDisplay[i][j]);
                }
            }
            UpdateDisplay();
        }
        public virtual void UpdateDisplay()
        {
            if (!(PixelDisplay is null) && PixelDisplay.Length > 0 && !(PixelDisplay[0] is null) && PixelDisplay[0].Length > 0 && !(PixelDisplay[0][0] is null))
            {
                for (int i = 0; i < PixelDisplay.Length; i++)
                {
                    for (int j = 0; j < PixelDisplay[i].Length; j++)
                    {
                        SetDisplaySize(i, j);
                        SetDisplayPosition(i, j);
                        SetDisplayColor(i, j);
                    }
                }
            }
        }
        private void SetDisplaySize(int x, int y)
        {
            PixelDisplay[x][y].Width = CMPerPixel * scale;
            PixelDisplay[x][y].Height = CMPerPixel * scale;
        }
        private void SetDisplayPosition(int x, int y)
        {
            Canvas.SetLeft(PixelDisplay[x][y], x * CMPerPixel * scale);
            Canvas.SetTop(PixelDisplay[x][y], y * CMPerPixel * scale);
        }
        private void SetDisplayColor(int x, int y)
        {
            PixelDisplay[x][y].Fill = new SolidColorBrush(PixelMapping[x][y] >= 0 ?
                Color.FromArgb(255, (byte)(125 + Math.Floor(130 * PixelMapping[x][y])), 125, 125) :
                Color.FromArgb((byte)(255), (byte)(125 * (1 + PixelMapping[x][y])), (byte)(125 * (1 + PixelMapping[x][y])), (byte)(125 * (1 + PixelMapping[x][y]))));
        }
        protected Canvas panel;
        protected double scale;
        protected double verticalOffset;
        protected double horizontalOffset;
        public void SetPanel(System.Windows.Controls.Canvas panel)
        {
            this.panel = panel;
        }
        public void SetScale(double scale, double horizontalOffset, double verticalOffset)
        {
            this.scale = scale;
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
        }
        public virtual double LeftMostPosition()
        {
            return CellBoundingRange(new SurfaceCoordinate(0, 0))[0].x;
        }
        public virtual double RightMostPosition()
        {
            return CellBoundingRange(new SurfaceCoordinate(PixelMapping.Length - 1, 0))[1].x;
        }
        public virtual double TopMostPosition()
        {
            return CellBoundingRange(new SurfaceCoordinate(0, 0))[0].y;
        }
        public virtual double BottomMostPosition()
        {
            return CellBoundingRange(new SurfaceCoordinate(0, PixelMapping[0].Length - 1))[1].y;
        }
        public double MaxHeight()
        {
            return BottomMostPosition() - TopMostPosition();
        }
        public double MaxWidth()
        {
            return RightMostPosition() - LeftMostPosition();
        }
        public void NotifyDisplayChanged()
        {
            foreach (var listener in listeners)
            {
                listener.HearDisplayChanged();
            }
        }
        private List<ListenToDispalyChanged> listeners = new List<ListenToDispalyChanged>();
        public void SubsricbeDisplayChanged(ListenToDispalyChanged listener)
        {
            listeners.Add(listener);
        }
        public void UnsubsricbeDisplayChanged(ListenToDispalyChanged listener)
        {
            listeners.Remove(listener);
        }
        public void UnsubsribeAll()
        {
            listeners = new List<ListenToDispalyChanged>();
        }
        public void StopDisplaying()
        {
            for (int i = 0; i < PixelDisplay.Length; i++)
            {
                for (int j = 0; j < PixelDisplay[i].Length; j++)
                {
                    panel.Children.Remove(PixelDisplay[i][j]);
                }
            }
        }

    }
}
