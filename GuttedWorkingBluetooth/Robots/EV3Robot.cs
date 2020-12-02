using RoboticNavigation.Sensors;
using RoboticNavigation.VectorMath;

namespace RoboticNavigation.Robots
{
    public class EV3Robot : Robot
    {
        private EV3Robot(double turnPercent, double turnIntegral, double turnDerivative, double turnError, double distancePercent, double distanceIntegral, double distanceDerivative, double distanceError, string imageFileName) :
            base(turnPercent, turnIntegral, turnDerivative, turnError, distancePercent, distanceIntegral, distanceDerivative, distanceError, imageFileName)
        {
        }
        public static EV3Robot Load()
        {
            //TODO
            return Default();
        }
        public static EV3Robot Default()
        {
            var robot = new EV3Robot(5, 6, 0.1, System.Math.PI / 90, 3, 0.1, 0.1, 5, "Robot.png");
            robot.Gyro = new GyroscopeSensor();
            robot.LeftMotor = new RotationSensor(true, 0, 1.5);
            robot.RightMotor = new RotationSensor(false, 0, 1.5);
            robot.IRSensor = new InfraredSensor(new Vector2d<double>(new double[] { -5, 3 }), 120);
            robot.USSensor = new UltrasonicSensor(new Vector2d<double>(new double[] { 5, 3 }), 120);
            robot.RoboticCommunication = new EV3Communication(robot);
            robot.Gyro.SubsribeToNewReadings(robot);
            robot.LeftMotor.SubsribeToNewReadings(robot);
            robot.RightMotor.SubsribeToNewReadings(robot);
            robot.Shape = new Vector2d<double>[4];
            robot.Shape[0] = new Vector2d<double>(new double[] { 11.5, 15.3, 0, 0 });
            robot.Shape[1] = new Vector2d<double>(new double[] { 11.5, -15.3, 0, 0 });
            robot.Shape[2] = new Vector2d<double>(new double[] { -11.5, -15.3, 0, 0 });
            robot.Shape[3] = new Vector2d<double>(new double[] { -11.5, 15.3, 0, 0 });
            return robot;
        }

    }
}
