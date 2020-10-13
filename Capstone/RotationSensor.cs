using System;

namespace Capstone
{
    public class RotationSensor : Sensor
    {
        public readonly bool LeftDrive;
        public readonly double DistancePerRotation;
        public double DistanceLastReading
        {
            get { return (((RotationSensorReading)RecentReading).AngleChange / (2 * Math.PI)) * DistancePerRotation; }
        }
        public RotationSensor(bool IsLeftDrive, double distancePerRotation = 0)
        {
            this.LeftDrive = IsLeftDrive;
            this.DistancePerRotation = 0;
        }
    }
}
