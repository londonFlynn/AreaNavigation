using System.Diagnostics;
using System.Numerics;

namespace Capstone
{
    public class EV3Robot : Robot
    {


        public EV3Robot()
        {
            this.Gyro = new GyroscopeSensor();
            this.LeftMotor = new RotationSensor(true, 5);
            this.RightMotor = new RotationSensor(false, 5);
            this.IRSensor = new InfraredSensor(new Vector<double>(new double[] { 5, 0, 0, 0 }));
            this.USSensor = new UltrasonicSensor(new Vector<double>(new double[] { -5, 0, 0, 0 }));
            this.RoboticCommunication = new EV3Communication(this);
            Gyro.SubsribeToNewReadings(this);
            LeftMotor.SubsribeToNewReadings(this);
            RightMotor.SubsribeToNewReadings(this);

            this.Shape = new System.Numerics.Vector<double>[4];
            this.Shape[0] = new Vector<double>(new double[] { 11.5, 15, 0, 0 });
            this.Shape[1] = new Vector<double>(new double[] { 11.5, -14, 0, 0 });
            this.Shape[2] = new Vector<double>(new double[] { -11.5, -14, 0, 0 });
            this.Shape[3] = new Vector<double>(new double[] { -11.5, 15, 0, 0 });

        }
        protected override void UpdatePosition()
        {
            //var movement = (this.LeftMotor.DistanceLastReading + this.RightMotor.DistanceLastReading) / 2;
            //var moveVector = new Vector<double>(new double[] { movement, 0, 0, 0 });
            //var angle = ((GyroscopeReading)this.Gyro.GetCurrentReading()).Radians;
            //var cos = Math.Cos(angle);
            //var sin = Math.Sin(angle);
            //moveVector = new Vector<double>(new double[] { (moveVector[0] * cos) - (moveVector[1] * sin), (moveVector[0] * sin) + (moveVector[1] * cos), 0, 0 });
            //this.Position = this.Position + moveVector;
            //this.Orientation = angle;
            Debug.WriteLine($"updating position: X={this.Position[0]}, Y={this.Position[1]}, Angle in Radians = {this.Orientation}");
            this.NotifyDisplayChanged();
        }
    }
}
