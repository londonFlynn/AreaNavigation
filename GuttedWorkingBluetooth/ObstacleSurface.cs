﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Capstone
{
    public class ObstacleSurface : IDisplayable
    {
        public double CMPerPixel { get; private set; }
        private double[][] PixelMapping { get; set; }
        public int Width { get { return PixelMapping.Length; } }
        public int Height { get { return PixelMapping[0].Length; } }
        public const double ReadingRadius = 6.5;
        public const double ReadingNegativeRadius = 6.5;
        public const double SensorFalloffDistance = 120;
        public ObstacleSurface(double cmPerPixel, int resolutionWidth, int resolutionHeight)
        {
            this.CMPerPixel = cmPerPixel;
            this.PixelMapping = new double[resolutionWidth][];
            for (int i = 0; i < PixelMapping.Length; i++)
            {
                PixelMapping[i] = new double[resolutionHeight];
            }
        }

        public void MatchToRangeReading(RangeReading reading, double amount, bool update = true)
        {
            //TODO Smooth trasitioning
            var endpoint = reading.ReadingPosition;
            var startPoint = reading.SensorPosition;
            List<SurfaceCoordinate> decrease;
            if (reading.Distance < SensorFalloffDistance)
            {
                var increase = CellsWithinRadiusOfPoint(endpoint * ((endpoint.Magnitude() + (ReadingRadius / 2)) / endpoint.Magnitude()), ReadingRadius);
                decrease = CellsWithinDistanceOfLineSegmant(endpoint, startPoint, ReadingNegativeRadius);
                decrease.RemoveAll(x => increase.Any(y => y.Equals(x)));
                foreach (var cell in increase)
                {
                    AdjustRangeReadingObstacleValueForCell(cell, endpoint, startPoint, amount);
                    SurfaceUpdateQueue.Add(cell);
                }
            }
            else
            {
                var point = reading.SensorPosition + (reading.DistanceVector.Unit() * (SensorFalloffDistance / 2));
                endpoint = point;
                decrease = CellsWithinDistanceOfLineSegmant(endpoint, startPoint, ReadingNegativeRadius);
            }
            foreach (var cell in decrease)
            {
                AdjustRangeReadingNoObstacleValueForCell(cell, endpoint, startPoint, amount);
                SurfaceUpdateQueue.Add(cell);
            }
            if (update)
                UpdateDisplayColors();
        }
        private void AdjustRangeReadingObstacleValueForCell(SurfaceCoordinate cell, Vector2d<double> readingPosition, Vector2d<double> SensorPostion, double scaleModifer)
        {

            var CurrentValue = GetPixelValue(cell);
            var DistanceFromReading = DistanceBetweenGridCellAndPoint(cell, readingPosition);
            var DistanceFromSensor = LineTool.DistanceBetweenPoints(CenterOfCell(cell), SensorPostion);
            var DistanceFromSensorScale = Math.Max(0, -((1d / SensorFalloffDistance) * DistanceFromSensor) + 1);
            var DistanceFromReadingScale = Math.Max(0, -((1d / ReadingNegativeRadius) * DistanceFromReading) + 1);
            var ReadingChanged = Math.Min(1, 2 * DistanceFromSensorScale * DistanceFromReadingScale * scaleModifer);
            var ChangedValue = CurrentValue + ReadingChanged > 1 ? 1 : CurrentValue + ReadingChanged;
            SetPixelValue(cell, ChangedValue);
        }
        private void AdjustRangeReadingNoObstacleValueForCell(SurfaceCoordinate cell, Vector2d<double> readingPosition, Vector2d<double> SensorPostion, double scaleModifer)
        {
            var CurrentValue = GetPixelValue(cell);
            var DistanceFromReading = DistanceBetweenGridCellAndLineSegmant(cell, SensorPostion, readingPosition);
            var DistanceFromSensor = LineTool.DistanceBetweenPoints(CenterOfCell(cell), SensorPostion);
            var DistanceFromSensorScale = Math.Max(0, -((1d / SensorFalloffDistance) * DistanceFromSensor) + 1);
            var DistanceFromReadingScale = Math.Max(0, -((1d / ReadingNegativeRadius) * DistanceFromReading) + 1);
            var ReadingChanged = Math.Min(1, 2 * DistanceFromSensorScale * DistanceFromReadingScale * scaleModifer);
            var ChangedValue = CurrentValue - ReadingChanged < -1 ? -1 : CurrentValue - ReadingChanged;
            SetPixelValue(cell, ChangedValue);
        }
        private void EnsureCoordinatesWithinGrid(List<SurfaceCoordinate> coords)
        {
            int minx = 0;
            int miny = 0;
            int maxx = 0;
            int maxy = 0;
            int shiftx = 0;
            int shifty = 0;
            foreach (var coord in coords)
            {
                if (coord.HorizontalCoordinate < minx)
                    minx = coord.HorizontalCoordinate;
                if (coord.VerticalCoorindate < miny)
                    miny = coord.VerticalCoorindate;
                if (coord.HorizontalCoordinate > maxx)
                    maxx = coord.HorizontalCoordinate;
                if (coord.VerticalCoorindate > miny)
                    maxy = coord.VerticalCoorindate;
            }
            if (minx < 0 || miny < 0)
            {
                var shift = new SurfaceCoordinate(minx, miny);
                SetPixelValue(shift, GetPixelValue(shift));
                shiftx = shift.HorizontalCoordinate - minx;
                shifty = shift.VerticalCoorindate - miny;
            }
            if (maxx >= this.PixelMapping.Length || maxy >= this.PixelMapping[0].Length)
            {
                var shift = new SurfaceCoordinate(maxx, maxy);
                SetPixelValue(shift, GetPixelValue(shift));
                shiftx = shift.HorizontalCoordinate - maxx > shiftx ? shift.HorizontalCoordinate - maxx : shiftx;
                shifty = shift.VerticalCoorindate - maxy > shifty ? shift.VerticalCoorindate - maxy : shifty;
            }
            if (shiftx > 0 || shifty > 0)
            {
                for (int i = 0; i < coords.Count; i++)
                {
                    coords[i] = new SurfaceCoordinate(coords[i].HorizontalCoordinate + shiftx, coords[i].VerticalCoorindate + shifty);
                }
            }
        }
        public void MatchToAreaEmptyReading(PositionOccupiedByRobotMemory reading, double amount, bool update = true)
        {
            //Debug.WriteLine("Matching To Past Occupied Reading");
            var cells = CellsWithinPolygon(reading.Shape);
            foreach (var cell in cells)
            {
                var CurrentValue = GetPixelValue(cell);
                var ChangedValue = Math.Max(-1, CurrentValue - (amount));
                SetPixelValue(cell, ChangedValue);
                SurfaceUpdateQueue.Add(cell);
            }
            if (update)
                UpdateDisplayColors();
        }
        public List<SurfaceCoordinate> CellsWithinRadiusOfPoint(Vector2d<double> point, double radius, bool ensureWithinGrid = true)
        {
            var startCell = CoordinateForPoint(point);
            var cellList = new List<SurfaceCoordinate>();
            CellsWithinRadiusOfPointRecursion(startCell, cellList, point, radius);
            if (ensureWithinGrid)
            {
                EnsureCoordinatesWithinGrid(cellList);
            }
            return cellList;
        }
        private void CellsWithinRadiusOfPointRecursion(SurfaceCoordinate checkCell, List<SurfaceCoordinate> containedCells, Vector2d<double> point, double radius)
        {
            if (!containedCells.Contains(checkCell) && DistanceBetweenGridCellAndPoint(checkCell, point) <= radius)
            {
                containedCells.Add(checkCell);
                var nextCellsToCheck = new List<SurfaceCoordinate>();
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate - 1, checkCell.VerticalCoorindate));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate - 1));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate + 1, checkCell.VerticalCoorindate));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate + 1));
                foreach (var next in nextCellsToCheck)
                {
                    CellsWithinRadiusOfPointRecursion(next, containedCells, point, radius);
                }
            }
        }
        private void SetPixelValue(SurfaceCoordinate cell, double value)
        {
            if (cell.HorizontalCoordinate >= PixelMapping.Length || cell.VerticalCoorindate >= PixelMapping[0].Length ||
                cell.HorizontalCoordinate < 0 || cell.VerticalCoorindate < 0)
            {
                var othercell = ExpandDimensions(cell);
                cell.VerticalCoorindate = othercell.VerticalCoorindate;
                cell.HorizontalCoordinate = othercell.HorizontalCoordinate;
            }
            PixelMapping[cell.HorizontalCoordinate][cell.VerticalCoorindate] = value;
        }
        public double GetPixelValue(SurfaceCoordinate cell)
        {
            if (cell.HorizontalCoordinate >= PixelMapping.Length || cell.VerticalCoorindate >= PixelMapping[0].Length ||
                cell.HorizontalCoordinate < 0 || cell.VerticalCoorindate < 0)
            {
                //var othercell = ExpandDimensions(cell);
                //cell.VerticalCoorindate = othercell.VerticalCoorindate;
                //cell.HorizontalCoordinate = othercell.HorizontalCoordinate;
                return 0;
            }
            return PixelMapping[cell.HorizontalCoordinate][cell.VerticalCoorindate];
        }
        private SurfaceCoordinate ExpandDimensions(SurfaceCoordinate coordinate)
        {
            int halfWidthDifference = 0;
            int halfHeightDifference = 0;
            int minWidth = this.PixelMapping.Length;
            if (coordinate.HorizontalCoordinate < 0 || coordinate.HorizontalCoordinate >= this.PixelMapping.Length)
            {
                halfWidthDifference = coordinate.HorizontalCoordinate < 0 ? Math.Abs(coordinate.HorizontalCoordinate) : 1 + coordinate.HorizontalCoordinate - this.PixelMapping.Length;
                halfWidthDifference = halfWidthDifference * 2 > minWidth ? halfWidthDifference : minWidth / 2;
                minWidth += (halfWidthDifference * 2);
            }
            int minHeight = this.PixelMapping[0].Length;
            if (coordinate.VerticalCoorindate < 0 || coordinate.VerticalCoorindate >= this.PixelMapping[0].Length)

            {
                halfHeightDifference = coordinate.VerticalCoorindate < 0 ? Math.Abs(coordinate.VerticalCoorindate) : 1 + coordinate.VerticalCoorindate - this.PixelMapping.Length;
                halfHeightDifference = halfHeightDifference * 2 > minHeight ? halfHeightDifference : minHeight / 2;
                minHeight += (halfHeightDifference * 2);
            }



            var map = new double[minWidth][];
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = new double[minHeight];
            }
            for (int i = 0; i < PixelMapping.Length; i++)
            {
                for (int j = 0; j < PixelMapping[i].Length; j++)
                {
                    map[i + halfWidthDifference][j + halfHeightDifference] = PixelMapping[i][j];
                }
            }
            this.PixelMapping = map;
            this.StopDisplaying();
            this.StartDisplay();
            this.NotifyDisplayChanged();
            return new SurfaceCoordinate(coordinate.HorizontalCoordinate + halfWidthDifference, coordinate.VerticalCoorindate + halfHeightDifference);
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
            return Math.Max(LineTool.DistanceBetweenPointAndLine(CenterOfCell(cell), point1, point2) - (CMPerPixel / 2d), 0);
        }
        public Vector2d<double> CenterOfCell(SurfaceCoordinate cell)
        {
            return new Vector2d<double>(new double[] {
                (cell.HorizontalCoordinate - PixelMapping.Length/2d) * CMPerPixel,
                (cell.VerticalCoorindate - PixelMapping[0].Length/2d) * CMPerPixel
            });
        }
        public List<SurfaceCoordinate> CellsWithinDistanceOfLineSegmant(Vector2d<double> point1, Vector2d<double> point2, double radius, bool ensureWithinGrid = true)
        {
            var startCell = CoordinateForPoint(point1);
            var cellList = new List<SurfaceCoordinate>();
            CellsWithinDistanceOfLineSegmantRecursion(startCell, cellList, point1, point2, radius);
            if (ensureWithinGrid)
            {
                EnsureCoordinatesWithinGrid(cellList);
            }
            return cellList;
        }
        private void CellsWithinDistanceOfLineSegmantRecursion(SurfaceCoordinate checkCell, List<SurfaceCoordinate> containedCells, Vector2d<double> point1, Vector2d<double> point2, double radius)
        {
            if (!containedCells.Contains(checkCell) && DistanceBetweenGridCellAndLineSegmant(checkCell, point1, point2) <= radius)
            {
                containedCells.Add(checkCell);
                var nextCellsToCheck = new List<SurfaceCoordinate>();
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate - 1, checkCell.VerticalCoorindate));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate + 1, checkCell.VerticalCoorindate));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate - 1));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate + 1));
                foreach (var next in nextCellsToCheck)
                {
                    CellsWithinDistanceOfLineSegmantRecursion(next, containedCells, point1, point2, radius);
                }
            }
        }
        public bool CellsWithinDistanceOfLineSegmantAreClear(Vector2d<double> point1, Vector2d<double> point2, double radius, double threshold)
        {
            var startCell = CoordinateForPoint(point1);
            var cellList = new List<SurfaceCoordinate>();
            return CellsWithinDistanceOfLineSegmantAreClearRecursion(startCell, cellList, point1, point2, radius, threshold);
        }
        private bool CellsWithinDistanceOfLineSegmantAreClearRecursion(SurfaceCoordinate checkCell, List<SurfaceCoordinate> containedCells, Vector2d<double> point1, Vector2d<double> point2, double radius, double threshold)
        {
            if (GetPixelValue(checkCell) >= threshold)
            {
                return false;
            }
            bool result = true;
            if (!containedCells.Contains(checkCell) && DistanceBetweenGridCellAndLineSegmant(checkCell, point1, point2) <= radius)
            {
                containedCells.Add(checkCell);
                var nextCellsToCheck = new List<SurfaceCoordinate>();
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate - 1, checkCell.VerticalCoorindate));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate + 1, checkCell.VerticalCoorindate));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate - 1));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate + 1));
                for (int i = 0; i < nextCellsToCheck.Count && result; i++)
                {
                    result = result && CellsWithinDistanceOfLineSegmantAreClearRecursion(nextCellsToCheck[i], containedCells, point1, point2, radius, threshold);
                }
            }
            return result;
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
        public SurfaceCoordinate CoordinateForPoint(Vector2d<double> point)
        {
            return new SurfaceCoordinate(
                (int)Math.Floor(point[0] / CMPerPixel) + (PixelMapping.Length / 2),
                (int)Math.Floor(point[1] / CMPerPixel) + (PixelMapping[0].Length / 2));
        }
        private List<SurfaceCoordinate> CellsWithinPolygon(Vector2d<double>[] polygon, bool ensureWithinGrid = true)
        {
            var startCell = CoordinateForPoint(polygon[0]);
            var cellList = new List<SurfaceCoordinate>();
            CellsWithinPolygonRecursion(startCell, cellList, polygon, true);
            if (ensureWithinGrid)
            {
                EnsureCoordinatesWithinGrid(cellList);
            }
            return cellList;
        }
        private void CellsWithinPolygonRecursion(SurfaceCoordinate checkCell, List<SurfaceCoordinate> containedCells, Vector2d<double>[] polygon, bool start = false)
        {
            bool within = CellIsWithinPolygon(checkCell, polygon);
            if (start || (!containedCells.Contains(checkCell) && within))
            {
                if (within)
                    containedCells.Add(checkCell);
                var nextCellsToCheck = new List<SurfaceCoordinate>();
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate - 1, checkCell.VerticalCoorindate));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate - 1));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate + 1, checkCell.VerticalCoorindate));
                nextCellsToCheck.Add(new SurfaceCoordinate(checkCell.HorizontalCoordinate, checkCell.VerticalCoorindate + 1));
                foreach (var next in nextCellsToCheck)
                {
                    CellsWithinPolygonRecursion(next, containedCells, polygon);
                }
            }
        }
        private bool CellIsWithinPolygon(SurfaceCoordinate checkCell, Vector2d<double>[] polygon)
        {
            return LineTool.PointIsWithinPolygon(CenterOfCell(checkCell), polygon);
        }
        public List<ArcSegmentConfidence> GetConfidenceArcSegmants(Vector2d<double> posistion)
        {
            double angleIncriment = Math.PI / 15;
            var arcs = new List<ArcSegmentConfidence>();
            double confidenceSum = 0;
            int confidenceCount = 0;
            int arcIndex = 0;
            for (double angle = 0; angle < 2 * Math.PI; angle += angleIncriment)
            {
                var direction = new Vector2d<double>(new double[] { SensorFalloffDistance / 2, 0 }).Rotate(angle);
                var confidence = GetRayConfidence(posistion, direction);

                if (arcs.Count == 0 || arcs[arcIndex - 1].AngleInRadians >= Math.PI ||
                    (arcs[arcIndex - 1].Confidence < ArcSegmentConfidence.ConfidenceTreshold && confidence >= ArcSegmentConfidence.ConfidenceTreshold) ||
                    (arcs[arcIndex - 1].Confidence >= ArcSegmentConfidence.ConfidenceTreshold && confidence < ArcSegmentConfidence.ConfidenceTreshold))
                {
                    arcs.Add(new ArcSegmentConfidence(angleIncriment, posistion, direction.Rotate(-angleIncriment / 2), confidence));
                    confidenceSum = confidence;
                    confidenceCount = 1;
                    arcIndex++;
                }
                else
                {
                    confidenceSum += confidence;
                    confidenceCount++;
                    arcs[arcIndex - 1].AngleInRadians += angleIncriment;
                    arcs[arcIndex - 1].Confidence = confidenceSum / confidenceCount;
                }
            }
            return arcs;
        }
        private double GetRayConfidence(Vector2d<double> posistion, Vector2d<double> direction)
        {
            var cells = CellsWithinDistanceOfLineSegmant(posistion, posistion + direction, CMPerPixel * 2, false);
            double highest = 0;
            double confidenceSum = 0;
            foreach (var cell in cells)
            {
                var cellConfidence = GetPixelValue(cell);
                if (cellConfidence > highest)
                    highest = cellConfidence;
                confidenceSum += Math.Abs(cellConfidence);
            }
            if (cells.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("There are no cells in that ray");
            }
            double result = Math.Max(confidenceSum / cells.Count, highest * 0.75);
            return Double.IsNaN(result) ? 0 : result;
        }
        public bool CellIsWithinGrid(SurfaceCoordinate cell)
        {
            return cell.HorizontalCoordinate > 0 && cell.VerticalCoorindate > 0 && cell.HorizontalCoordinate < Width && cell.VerticalCoorindate < Width;
        }

        //Display functionality
        private List<SurfaceCoordinate> SurfaceUpdateQueue = new List<SurfaceCoordinate>();
        private System.Windows.Shapes.Rectangle[][] PixelDisplay;
        protected Canvas panel;
        protected double scale;
        protected double verticalOffset;
        protected double horizontalOffset;
        private void UpdateDisplayColors()
        {
            for (int i = 0; i < SurfaceUpdateQueue.Count; i++)
            {
                //try
                //{
                if (!(SurfaceUpdateQueue[i] is null))
                    SetDisplayColor(SurfaceUpdateQueue[i].HorizontalCoordinate, SurfaceUpdateQueue[i].VerticalCoorindate);
                //}
                //catch (System.NullReferenceException e) { }
            }
            SurfaceUpdateQueue = new List<SurfaceCoordinate>();
        }
        public virtual void StartDisplay()
        {
            PixelDisplay = new System.Windows.Shapes.Rectangle[this.PixelMapping.Length][];
            for (int i = 0; i < PixelDisplay.Length; i++)
            {
                PixelDisplay[i] = new System.Windows.Shapes.Rectangle[this.PixelMapping[0].Length];
                for (int j = 0; j < PixelDisplay[i].Length; j++)
                {
                    PixelDisplay[i][j] = new System.Windows.Shapes.Rectangle();
                    Canvas.SetZIndex(PixelDisplay[i][j], int.MinValue);
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
            var coord = new SurfaceCoordinate(x, y);
            PixelDisplay[x][y].Fill = new SolidColorBrush(GetPixelValue(coord) >= 0 ?
                Color.FromArgb(255, (byte)(125 + Math.Floor(130 * GetPixelValue(coord))), 125, 125) :
                Color.FromArgb((byte)(255), (byte)(125 * (1 + GetPixelValue(coord))), (byte)(125 * (1 + GetPixelValue(coord))), (byte)(125 * (1 + GetPixelValue(coord)))));
        }
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
