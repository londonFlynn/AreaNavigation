using RoboticNavigation.ArcSegmants;
using RoboticNavigation.Display;
using RoboticNavigation.ImageManipulators;
using RoboticNavigation.VectorMath;
using System;
using System.Collections.Generic;
namespace RoboticNavigation.Surface
{
    public abstract class ObstacleSurface : IDisplayablePositionedItem
    {
        public double CMPerPixel { get; protected set; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public const double ReadingRadius = 6.5;
        public const double ReadingNegativeRadius = 6.5;
        protected const double AngleIncriment = Math.PI / 15;
        private ObstacleSurfaceDisplayer Displayer;


        public virtual List<SurfaceCoordinate> CellsWithinRadiusOfPoint(Vector2d<double> point, double radius, bool ensureWithinGrid = true)
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
        public abstract double GetPixelValue(SurfaceCoordinate cell);

        protected double DistanceBetweenGridCellAndPoint(SurfaceCoordinate cell, Vector2d<double> point)
        {
            var bounding = CellBoundingRange(cell);
            var minBounding = bounding[0];
            var maxBounding = bounding[1];
            var dx = Math.Max(0, Math.Max(minBounding[0] - point[0], point[0] - maxBounding[0]));
            var dy = Math.Max(0, Math.Max(minBounding[1] - point[1], point[1] - maxBounding[1]));
            return Math.Sqrt((dx * dx) + (dy * dy));
        }
        public bool PathIsClear(Vector2d<double>[] path, double distaceFromPath, double threshold)
        {
            bool isClear = true;
            for (int i = 1; isClear && i < path.Length; i++)
            {
                isClear = isClear && CellsWithinDistanceOfLineSegmantAreClear(path[i - 1], path[i], distaceFromPath, threshold);
            }
            return isClear;
        }
        protected double DistanceBetweenGridCellAndLineSegmant(SurfaceCoordinate cell, Vector2d<double> point1, Vector2d<double> point2)
        {
            return Math.Max(LineTool.DistanceBetweenPointAndLine(CenterOfCell(cell), point1, point2) - (CMPerPixel / 2d), 0);
        }
        public Vector2d<double> CenterOfCell(SurfaceCoordinate cell)
        {
            return new Vector2d<double>(new double[] {
                (cell.HorizontalCoordinate - Width/2d) * CMPerPixel,
                (cell.VerticalCoorindate - Height/2d) * CMPerPixel
            });
        }
        public virtual List<SurfaceCoordinate> CellsWithinDistanceOfLineSegmant(Vector2d<double> point1, Vector2d<double> point2, double radius, bool ensureWithinGrid = true)
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
        protected Vector2d<double>[] CellBoundingRange(SurfaceCoordinate cell)
        {
            Vector2d<double>[] result = new Vector2d<double>[4];
            result[0] = new Vector2d<double>(new double[] { (cell.HorizontalCoordinate - (Width / 2)) * CMPerPixel, (cell.VerticalCoorindate - (Height / 2)) * CMPerPixel });
            result[1] = new Vector2d<double>(new double[] { result[0][0] + CMPerPixel - double.Epsilon, result[0][1] + CMPerPixel - double.Epsilon });
            result[2] = new Vector2d<double>(new double[] { result[0][0], result[1][1] });
            result[3] = new Vector2d<double>(new double[] { result[1][0], result[0][1] });
            return result;
        }
        public SurfaceCoordinate CoordinateForPoint(Vector2d<double> point)
        {
            return new SurfaceCoordinate(
                (int)Math.Floor(point[0] / CMPerPixel) + (Width / 2),
                (int)Math.Floor(point[1] / CMPerPixel) + (Height / 2));
        }
        protected virtual List<SurfaceCoordinate> CellsWithinPolygon(Vector2d<double>[] polygon, bool ensureWithinGrid = true)
        {
            var startCell = CoordinateForPoint(polygon[0]);
            var cellList = new List<SurfaceCoordinate>();
            CellsWithinPolygonRecursion(startCell, cellList, polygon, true);
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
        protected bool CellIsWithinPolygon(SurfaceCoordinate checkCell, Vector2d<double>[] polygon)
        {
            return LineTool.PointIsWithinPolygon(CenterOfCell(checkCell), polygon);
        }
        public List<ArcSegmentConfidence> GetConfidenceArcSegmants(Vector2d<double> posistion, double distance)
        {
            var arcs = new List<ArcSegmentConfidence>();
            double confidenceSum = 0;
            int confidenceCount = 0;
            int arcIndex = 0;
            for (double angle = 0; angle < 2 * Math.PI; angle += AngleIncriment)
            {
                var direction = new Vector2d<double>(new double[] { distance, 0 }).Rotate(angle);
                var confidence = GetRayConfidence(posistion, direction);

                if (arcs.Count == 0 || arcs[arcIndex - 1].AngleInRadians >= Math.PI ||
                    (arcs[arcIndex - 1].Confidence < ArcSegmentConfidence.ConfidenceTreshold && confidence >= ArcSegmentConfidence.ConfidenceTreshold) ||
                    (arcs[arcIndex - 1].Confidence >= ArcSegmentConfidence.ConfidenceTreshold && confidence < ArcSegmentConfidence.ConfidenceTreshold))
                {
                    arcs.Add(new ArcSegmentConfidence(AngleIncriment, posistion, direction.Rotate(-AngleIncriment / 2), confidence));
                    confidenceSum = confidence;
                    confidenceCount = 1;
                    arcIndex++;
                }
                else
                {
                    confidenceSum += confidence;
                    confidenceCount++;
                    arcs[arcIndex - 1].AngleInRadians += AngleIncriment;
                    arcs[arcIndex - 1].Confidence = confidenceSum / confidenceCount;
                }
            }
            return arcs;
        }
        public double GetLowestConfidneceInArcSegmant(ArcSegment arc)
        {
            double lowest = 1;
            for (double angle = 0; angle - AngleIncriment < arc.AngleInRadians; angle += AngleIncriment)
            {
                var direction = arc.RaySegmant.Rotate(angle);
                var confidence = Math.Abs(GetRayConfidence(arc.Position, direction));
                if (confidence < lowest)
                {
                    lowest = confidence;
                }
            }
            return lowest;
        }
        public double GetHighestObstacleConfidenceInArcSegmant(ArcSegment arc)
        {
            double HighestObstacleConfidence = -1;
            for (double angle = 0; angle <= arc.AngleInRadians; angle += AngleIncriment)
            {
                var direction = arc.RaySegmant.Rotate(angle);
                var confidence = GetRayObstacle(arc.Position, direction);
                if (confidence > HighestObstacleConfidence)
                {
                    HighestObstacleConfidence = confidence;
                }
            }
            return HighestObstacleConfidence;
        }
        protected double GetRayConfidence(Vector2d<double> posistion, Vector2d<double> direction)
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
        protected double GetRayObstacle(Vector2d<double> posistion, Vector2d<double> direction)
        {
            var cells = CellsWithinDistanceOfLineSegmant(posistion, posistion + direction, CMPerPixel * 2, false);
            double highest = -1;
            foreach (var cell in cells)
            {
                var obstacleConfidence = GetPixelValue(cell);
                if (obstacleConfidence > highest)
                    highest = obstacleConfidence;
            }
            if (cells.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("There are no cells in that ray");
            }
            return highest;
        }
        public bool CellIsWithinGrid(SurfaceCoordinate cell)
        {
            return cell.HorizontalCoordinate > 0 && cell.VerticalCoorindate > 0 && cell.HorizontalCoordinate < Width && cell.VerticalCoorindate < Width;
        }
        public PositionedItemDisplayer GetDisplayer()
        {
            if (Displayer is null)
            {
                Displayer = new ObstacleSurfaceDisplayer(this);
            }
            return Displayer;
        }

        public void DisplayableValueChanged()
        {
            if (!(Displayer is null))
            {
                Displayer.PosistionedItemValueChanged();
            }
        }
        public double LowestX()
        {
            return (-Width / 2) * CMPerPixel;
        }

        public double HighestX()
        {
            return (Width / 2) * CMPerPixel;
        }

        public double LowestY()
        {
            return (-Height / 2) * CMPerPixel;
        }

        public double HighestY()
        {
            return (Height / 2) * CMPerPixel;
        }

        public Vector2d<double> LowXLowYCorner()
        {
            return new Vector2d<double>(new double[] { LowestX(), LowestY() });
        }

        public Vector2d<double> LowXHighYCorner()
        {
            return new Vector2d<double>(new double[] { LowestX(), HighestY() });
        }

        public Vector2d<double> HighXHighYCorner()
        {
            return new Vector2d<double>(new double[] { HighestX(), HighestY() });
        }

        public Vector2d<double> HighXLowYCorner()
        {
            return new Vector2d<double>(new double[] { HighestX(), LowestY() });
        }

        public virtual ObstacleSurface ChangeResolution(double cmPerPixel)
        {
            var scaleRatio = (float)(CMPerPixel / cmPerPixel);
            var surface = new ImagedObstacleSurface(this.GenerateImage().ToBitmap().ChangeResolution(scaleRatio), cmPerPixel);
            System.Diagnostics.Debug.WriteLine(surface.Width);
            return surface;
        }
    }
}
