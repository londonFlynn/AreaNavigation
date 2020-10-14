using System;
using System.Numerics;

namespace Capstone
{
    public class EV3Robot : Robot
    {
        private GyroscopeSensor Gyro;
        private RotationSensor LeftMotor;
        private RotationSensor RightMotor;
        private InfraredSensor IRSensor;
        private UltrasonicSensor USSensor;

        protected override void UpdatePosition()
        {
            var movement = (this.LeftMotor.DistanceLastReading + this.RightMotor.DistanceLastReading) / 2;
            var moveVector = new Vector<double>(new double[] { movement, 0, 0, 0 });
            var angle = this.Gyro.Angle;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            moveVector = new Vector<double>(new double[] { (moveVector[0] * cos) - (moveVector[1] * sin), (moveVector[0] * sin) + (moveVector[1] * cos) });
            this.Position = this.Position + moveVector;
            this.Orientation = angle;
        }
    }
}
