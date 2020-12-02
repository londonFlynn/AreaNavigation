using System;

namespace RoboticNavigation
{
    public class ApplicationConfig
    {
        public static double ArcSegmantConfidenceThreshold { get; private set; }
        public static int DisplayFramrateMillis { get; private set; }
        public static double MovementRequiredConfidenceThreshold { get; private set; }
        public static double MovementMaximumObstacleConfidence { get; private set; }
        public static int MovementTimeoutTime { get; private set; }
        public static double NetworkObstacleCertintyThreshold { get; private set; }
        public static double ObstacleSurfaceReadingRadius { get; private set; }
        public static double ObstacleSurfaceReadingNegativeRadius { get; private set; }
        public static double ObstacleSurfaceAngleIncriment { get; private set; }
        public static double RangeReadingStartingSampleAdjustment { get; private set; }
        public static double RangeReadingResampleAdjustment { get; private set; }
        public static double PositionStartingSampleAdjustment { get; private set; }
        public static double PositionResampleAdjustment { get; private set; }
        public static string RobotDataFilePath { get; private set; }
        public static int ObstacleSurfaceCMPerPixel { get; private set; }
        public static int ObstacleSurfaceWidth { get; private set; }
        public static int ObstacleSurfaceHeight { get; private set; }

        public static void Load()
        {
            //TODO
            DefaultConfig();
        }
        public static void DefaultConfig()
        {
            ArcSegmantConfidenceThreshold = 0.75;
            DisplayFramrateMillis = 100;
            MovementRequiredConfidenceThreshold = 0.75;
            MovementMaximumObstacleConfidence = 0.5;
            MovementTimeoutTime = 1000;
            NetworkObstacleCertintyThreshold = 0.8;
            ObstacleSurfaceReadingRadius = 6.5;
            ObstacleSurfaceReadingNegativeRadius = 6.5;
            ObstacleSurfaceAngleIncriment = Math.PI / 15;
            RangeReadingStartingSampleAdjustment = 0.25;
            RangeReadingResampleAdjustment = 7;
            PositionStartingSampleAdjustment = 1;
            PositionResampleAdjustment = 0.25;
            RobotDataFilePath = null;
            ObstacleSurfaceCMPerPixel = 3;
            ObstacleSurfaceWidth = 150;
            ObstacleSurfaceHeight = 150;
        }
    }
}
