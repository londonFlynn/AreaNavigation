using RoboticNavigation.VectorMath;
using System;

namespace RoboticNavigation.Surface
{
    public class CompositeObstacleSurface : ObstacleSurface
    {
        private ObstacleSurface StaticSurface;
        private ObstacleSurface ShiftedSurface;
        private double RotationRadians;
        private VectorMath.Vector2d<double> TranslationVector;

        public CompositeObstacleSurface(ObstacleSurface staticSurface, ObstacleSurface translatedSurface, double rotationInRadians, VectorMath.Vector2d<double> translationVector)
        {
            this.StaticSurface = staticSurface;
            this.ShiftedSurface = translatedSurface;
            this.RotationRadians = rotationInRadians;
            this.TranslationVector = translationVector;
            this.CMPerPixel = staticSurface.CMPerPixel;
        }

        public override int Width => WidthCalculation();

        public override int Height => HeightCalculation();

        public override double GetPixelValue(SurfaceCoordinate cell)
        {
            var shiftedValue = ShiftedSurface.GetPixelValue(ShiftedSurface.CoordinateForPoint(StaticSurface.CenterOfCell(cell)));
            var staticValue = StaticSurface.GetPixelValue(cell);
            return (shiftedValue + staticValue) / 2d;
        }
        private int WidthCalculation()
        {
            var lowest = Math.Min(ShiftedLowestX(), StaticSurface.LowestX());
            var highest = Math.Min(ShiftedHighestX(), StaticSurface.HighestX());
            return (int)Math.Ceiling((highest - lowest) / CMPerPixel);
        }
        private int HeightCalculation()
        {
            var lowest = Math.Min(ShiftedLowestY(), StaticSurface.LowestY());
            var highest = Math.Min(ShiftedHighestY(), StaticSurface.HighestY());
            return (int)Math.Ceiling((highest - lowest) / CMPerPixel);
        }
        public Vector2d<double>[] ShiftedExtremePoints()
        {
            var array = new Vector2d<double>[4];
            array[0] = this.ShiftedSurface.HighXHighYCorner();
            array[1] = this.ShiftedSurface.LowXLowYCorner();
            array[2] = this.ShiftedSurface.LowXHighYCorner();
            array[3] = this.ShiftedSurface.HighXLowYCorner();
            for (int i = 0; i < array.Length; i++)
            {
                var vector = array[i];
                array[i] = vector.Rotate(this.RotationRadians) + this.TranslationVector;
            }
            return array;
        }
        public double ShiftedLowestX()
        {
            var value = double.MaxValue;
            foreach (var vector in this.ShiftedExtremePoints())
            {
                if (vector.x < value)
                {
                    value = vector.x;
                }
            }
            return value;
        }
        public double ShiftedHighestX()
        {
            var value = double.MinValue;
            foreach (var vector in this.ShiftedExtremePoints())
            {
                if (vector.x > value)
                {
                    value = vector.x;
                }
            }
            return value;
        }
        public double ShiftedLowestY()
        {
            var value = double.MaxValue;
            foreach (var vector in this.ShiftedExtremePoints())
            {
                if (vector.y < value)
                {
                    value = vector.y;
                }
            }
            return value;
        }
        public double ShiftedHighestY()
        {
            var value = double.MinValue;
            foreach (var vector in this.ShiftedExtremePoints())
            {
                if (vector.y > value)
                {
                    value = vector.y;
                }
            }
            return value;
        }

        public override ObstacleSurface ChangeResolution(double cmPerPixel)
        {
            return new CompositeObstacleSurface(StaticSurface.ChangeResolution(cmPerPixel), ShiftedSurface.ChangeResolution(CMPerPixel), RotationRadians, TranslationVector);
        }
        public double FittingMetric()
        {
            throw new System.NotImplementedException();
        }
    }
}
