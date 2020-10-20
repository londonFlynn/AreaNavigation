using System;
using System.Diagnostics;
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

        public EV3Robot()
        {
            this.Gyro = new GyroscopeSensor();
            this.LeftMotor = new RotationSensor(true, 13.5);
            this.RightMotor = new RotationSensor(false, 13.5);
            this.IRSensor = new InfraredSensor(new Vector<double>(new double[] { 5, 0, 0, 0 }));
            this.USSensor = new UltrasonicSensor(new Vector<double>(new double[] { -5, 0, 0, 0 }));
            this.RoboticCommunication = new EV3Communication();
            RoboticCommunication.StartGettingSensorReadings(Gyro);
            RoboticCommunication.StartGettingSensorReadings(LeftMotor);
            RoboticCommunication.StartGettingSensorReadings(RightMotor);
            RoboticCommunication.StartGettingSensorReadings(IRSensor);
            RoboticCommunication.StartGettingSensorReadings(USSensor);
            Gyro.SubsribeToNewReadings(this);
        }
        protected override void UpdatePosition()
        {
            var movement = (this.LeftMotor.DistanceLastReading + this.RightMotor.DistanceLastReading) / 2;
            var moveVector = new Vector<double>(new double[] { movement, 0, 0, 0 });
            var angle = ((GyroscopeReading)this.Gyro.GetCurrentReading()).Radians;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            moveVector = new Vector<double>(new double[] { (moveVector[0] * cos) - (moveVector[1] * sin), (moveVector[0] * sin) + (moveVector[1] * cos), 0, 0 });
            this.Position = this.Position + moveVector;
            this.Orientation = angle;
            Debug.WriteLine($"updating position: X={this.Position[0]}, Y={this.Position[1]}, Angle in Radians = {this.Orientation}");
        }
    }
}
