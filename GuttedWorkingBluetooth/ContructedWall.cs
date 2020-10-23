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
            if (RangeReadings.Count == 1)
            {
                this.StartPosition = RangeReadings[0].ReadingPosition;
                this.EndPosition = RangeReadings[0].ReadingPosition;
            }
            if (RangeReadings.Count > 2)
            {
                //TODO use line fitting algoritms to fit the line segmant to the readings.
            }
        }
        public void AddRangeReading(RangeReading reading)
        {
            //TODO
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
    }
}
