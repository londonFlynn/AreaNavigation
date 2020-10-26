using System;
using System.Collections.Generic;

namespace Capstone
{
    public class ContructedWall : Wall
    {
        private List<RangeReading> RangeReadings;

        public const double SplitWallMetricThreshold = 4;
        public const double SplitWallMaximumSpanThreshold = 4;
        public ContructedWall(List<RangeReading> RangeReadings)
        {
            this.RangeReadings = RangeReadings;
            FitLineToReadings();
        }
        private double AverageFittingMetric()
        {
            double metric = 0;
            foreach (RangeReading reading in RangeReadings)
            {
                metric += FittingMetric(reading) / RangeReadings.Count;
            }
            return metric;
        }
        private void FitLineToReadings()
        {
            if (RangeReadings.Count == 2)
            {
                this.StartPosition = RangeReadings[0].ReadingPosition;
                this.EndPosition = RangeReadings[1].ReadingPosition;
            }
            if (RangeReadings.Count > 2)
            {
                var yMean = MeanOfY();
                var xMean = MeanOfX();
                var slope = -Slope(xMean, yMean);
                var yIntercept = yMean - (slope * xMean);
                SetWallFromLine(slope, yIntercept);
            }
        }
        private void SetWallFromLine(double m, double b)
        {
            if (double.IsNaN(m) || double.IsNaN(b))
            {
                string message = "";
                if (double.IsNaN(m))
                {
                    message = "m is NaN";
                }
                else if (double.IsNaN(b))
                {
                    message = "b is NaN";

                }
                throw new ArgumentException($"m&b cannot be NaN: {message}");
            }
            var x = this.LeftMostPosition();
            var y = (m * x) + b;
            this.StartPosition = new Vector2d<double>(new double[] { x, y });

            x = this.RightMostPosition();
            y = (m * x) + b;
            this.EndPosition = new Vector2d<double>(new double[] { x, y });
        }
        private double MeanOfX()
        {
            double total = 0;
            foreach (RangeReading reading in RangeReadings)
            {
                total += reading.ReadingPosition[0];
            }
            return total / RangeReadings.Count;
        }
        private double MeanOfY()
        {
            double total = 0;
            foreach (RangeReading reading in RangeReadings)
            {
                total += reading.ReadingPosition[1];
            }
            return total / RangeReadings.Count;
        }
        private static double NumeratorPart(Vector2d<double> readingPos, double xMean, double yMean)
        {
            return (readingPos[0] - xMean) * (readingPos[1] - yMean);
        }
        private static double DenominatorPart(Vector2d<double> readingPos, double xMean)
        {
            return Math.Pow(readingPos[0] - xMean, 2);
        }
        private double Slope(double xMean, double yMean)
        {
            double numerator = 0;
            double denominator = 0;
            foreach (RangeReading reading in RangeReadings)
            {
                var pos = reading.ReadingPosition;
                numerator += NumeratorPart(pos, xMean, yMean);
                denominator += DenominatorPart(pos, xMean);
            }
            var slope = numerator / denominator;
            //return double.IsNegativeInfinity(slope) ? double.MinValue : double.IsPositiveInfinity(slope) ? double.MaxValue : slope;
            return slope;
        }


        public void AddRangeReading(RangeReading reading)
        {
            if (!RangeReadings.Contains(reading))
            {
                RangeReadings.Add(reading);
                FitLineToReadings();
                UpdateDisplay();
            }
        }
        private double FittingMetric(RangeReading reading)
        {
            //TODO, find distance from reading point to line segmant, and square it.
            return 0;
        }
        private RangeReading HighestMetric()
        {
            double highest = 0;
            RangeReading result = null;
            foreach (RangeReading reading in RangeReadings)
            {
                var metric = FittingMetric(reading);
                if (metric > highest)
                {
                    highest = metric;
                    result = reading;
                }
            }
            return result;
        }
        public ContructedWall SplitWall()
        {
            /*
             * TODO
             */
            return null;
        }
        public bool ShouldSplitWall()
        {
            return this.RangeReadings.Count >= 3 && this.AverageFittingMetric() >= SplitWallMetricThreshold;
        }
        public bool ShouldCombineWall(ContructedWall that)
        {
            double metric = 0;
            foreach (RangeReading reading in that.RangeReadings)
            {
                metric += FittingMetric(reading) / that.RangeReadings.Count;
            }
            return metric < SplitWallMetricThreshold;
        }
        public void CombineWall(ContructedWall that)
        {
            //TODO, combine the two walls
        }







        public override double TopMostPosition()
        {
            double top = double.MaxValue;
            foreach (RangeReading reading in RangeReadings)
            {
                top = reading.SensorPosition[1] < top ? reading.SensorPosition[1] : top;
                top = reading.ReadingPosition[1] < top ? reading.ReadingPosition[1] : top;
            }
            return top;
        }
        public override double RightMostPosition()
        {
            double right = double.MinValue;
            foreach (RangeReading reading in RangeReadings)
            {
                right = reading.SensorPosition[0] > right ? reading.SensorPosition[0] : right;
                right = reading.ReadingPosition[0] > right ? reading.ReadingPosition[0] : right;
            }
            return right;
        }
        public override double LeftMostPosition()
        {
            double left = double.MaxValue;
            foreach (RangeReading reading in RangeReadings)
            {
                left = reading.SensorPosition[0] < left ? reading.SensorPosition[0] : left;
                left = reading.ReadingPosition[0] < left ? reading.ReadingPosition[0] : left;
            }
            return left;
        }
        public override double BottomMostPosition()
        {
            double bottom = double.MinValue;
            foreach (RangeReading reading in RangeReadings)
            {
                bottom = reading.SensorPosition[1] > bottom ? reading.SensorPosition[1] : bottom;
                bottom = reading.ReadingPosition[1] > bottom ? reading.ReadingPosition[1] : bottom;
            }
            return bottom;
        }
    }
}
