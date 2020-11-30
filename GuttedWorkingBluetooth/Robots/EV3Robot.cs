using RoboticNavigation.Sensors;
using RoboticNavigation.VectorMath;

namespace RoboticNavigation.Robots
{
    public class EV3Robot : Robot
    {


        public EV3Robot()
        {
            this.Gyro = new GyroscopeSensor();
            this.LeftMotor = new RotationSensor(true, 0, 1.5);
            this.RightMotor = new RotationSensor(false, 0, 1.5);
            this.IRSensor = new InfraredSensor(new Vector2d<double>(new double[] { -5, 3 }), 120);
            this.USSensor = new UltrasonicSensor(new Vector2d<double>(new double[] { 5, 3 }), 120);
            this.RoboticCommunication = new EV3Communication(this);
            Gyro.SubsribeToNewReadings(this);
            LeftMotor.SubsribeToNewReadings(this);
            RightMotor.SubsribeToNewReadings(this);
            this.Shape = new Vector2d<double>[4];
            this.Shape[0] = new Vector2d<double>(new double[] { 11.5, 15.3, 0, 0 });
            this.Shape[1] = new Vector2d<double>(new double[] { 11.5, -15.3, 0, 0 });
            this.Shape[2] = new Vector2d<double>(new double[] { -11.5, -15.3, 0, 0 });
            this.Shape[3] = new Vector2d<double>(new double[] { -11.5, 15.3, 0, 0 });
        }

    }
}
