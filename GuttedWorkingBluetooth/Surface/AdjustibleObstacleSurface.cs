using RoboticNavigation.Display;
using RoboticNavigation.Sensors.SensorReadings;
using RoboticNavigation.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoboticNavigation.Surface
{
    public class AdjustibleObstacleSurface : ObstacleSurface, IDisplayablePositionedItem
    {
        private double[][] _mapping;
        private double[][] PixelMapping { get { return _mapping; } set { _mapping = value; DisplayableValueChanged(); } }

        public override int Width { get => PixelMapping.Length; }
        public override int Height { get => PixelMapping[0].Length; }

        public AdjustibleObstacleSurface(double cmPerPixel, int resolutionWidth, int resolutionHeight)
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
            if (reading.Distance < reading.SensorFalloffDistance)
            {
                var increase = CellsWithinRadiusOfPoint(endpoint * ((endpoint.Magnitude() + (ReadingRadius / 2)) / endpoint.Magnitude()), ReadingRadius);
                decrease = CellsWithinDistanceOfLineSegmant(endpoint, startPoint, ReadingNegativeRadius);
                decrease.RemoveAll(x => increase.Any(y => y.Equals(x)));
                foreach (var cell in increase)
                {
                    AdjustRangeReadingObstacleValueForCell(cell, endpoint, startPoint, amount, reading.SensorFalloffDistance);
                }
            }
            else
            {
                var point = reading.SensorPosition + (reading.DistanceVector.Unit() * (reading.SensorFalloffDistance / 2));
                endpoint = point;
                decrease = CellsWithinDistanceOfLineSegmant(endpoint, startPoint, ReadingNegativeRadius);
            }
            foreach (var cell in decrease)
            {
                AdjustRangeReadingNoObstacleValueForCell(cell, endpoint, startPoint, amount, reading.SensorFalloffDistance);
            }
        }
        private void AdjustRangeReadingObstacleValueForCell(SurfaceCoordinate cell, Vector2d<double> readingPosition, Vector2d<double> SensorPostion, double scaleModifer, double sensorFalloffDistance)
        {

            var CurrentValue = GetPixelValue(cell);
            var DistanceFromReading = DistanceBetweenGridCellAndPoint(cell, readingPosition);
            var DistanceFromSensor = LineTool.DistanceBetweenPoints(CenterOfCell(cell), SensorPostion);
            var DistanceFromSensorScale = Math.Max(0, -((1d / sensorFalloffDistance) * DistanceFromSensor) + 1);
            var DistanceFromReadingScale = Math.Max(0, -((1d / ReadingNegativeRadius) * DistanceFromReading) + 1);
            var ReadingChanged = Math.Min(1, 2 * DistanceFromSensorScale * DistanceFromReadingScale * scaleModifer);
            var ChangedValue = CurrentValue + ReadingChanged > 1 ? 1 : CurrentValue + ReadingChanged;
            SetPixelValue(cell, ChangedValue);
        }
        private void AdjustRangeReadingNoObstacleValueForCell(SurfaceCoordinate cell, Vector2d<double> readingPosition, Vector2d<double> SensorPostion, double scaleModifer, double sensorFalloffDistance)
        {
            var CurrentValue = GetPixelValue(cell);
            var DistanceFromReading = DistanceBetweenGridCellAndLineSegmant(cell, SensorPostion, readingPosition);
            var DistanceFromSensor = LineTool.DistanceBetweenPoints(CenterOfCell(cell), SensorPostion);
            var DistanceFromSensorScale = Math.Max(0, -((1d / sensorFalloffDistance) * DistanceFromSensor) + 1);
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
            }
        }
        public override List<SurfaceCoordinate> CellsWithinRadiusOfPoint(Vector2d<double> point, double radius, bool ensureWithinGrid = true)
        {
            var cellList = base.CellsWithinRadiusOfPoint(point, radius, ensureWithinGrid);
            if (ensureWithinGrid)
            {
                EnsureCoordinatesWithinGrid(cellList);
            }
            return cellList;
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
            if (GetPixelValue(cell) != value)
            {
                DisplayableValueChanged();
            }
            PixelMapping[cell.HorizontalCoordinate][cell.VerticalCoorindate] = value;
        }
        public override double GetPixelValue(SurfaceCoordinate cell)
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
            return new SurfaceCoordinate(coordinate.HorizontalCoordinate + halfWidthDifference, coordinate.VerticalCoorindate + halfHeightDifference);
        }
        public override List<SurfaceCoordinate> CellsWithinDistanceOfLineSegmant(Vector2d<double> point1, Vector2d<double> point2, double radius, bool ensureWithinGrid = true)
        {
            var cellList = base.CellsWithinDistanceOfLineSegmant(point1, point2, radius, ensureWithinGrid);
            if (ensureWithinGrid)
            {
                EnsureCoordinatesWithinGrid(cellList);
            }
            return cellList;
        }

        protected override List<SurfaceCoordinate> CellsWithinPolygon(Vector2d<double>[] polygon, bool ensureWithinGrid = true)
        {
            var cellList = base.CellsWithinPolygon(polygon, ensureWithinGrid);
            if (ensureWithinGrid)
            {
                EnsureCoordinatesWithinGrid(cellList);
            }
            return cellList;
        }
    }
}
