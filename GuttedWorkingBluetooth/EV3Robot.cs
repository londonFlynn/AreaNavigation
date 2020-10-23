namespace Capstone
{
    public class EV3Robot : Robot
    {


        public EV3Robot()
        {
            this.Gyro = new GyroscopeSensor();
            this.LeftMotor = new RotationSensor(true, 1.5);
            this.RightMotor = new RotationSensor(false, 1.6);
            this.IRSensor = new InfraredSensor(new Vector<double>(new double[] { 5, 0, 0, 0 }));
            this.USSensor = new UltrasonicSensor(new Vector<double>(new double[] { -5, 0, 0, 0 }));
            this.RoboticCommunication = new EV3Communication(this);
            Gyro.SubsribeToNewReadings(this);
            LeftMotor.SubsribeToNewReadings(this);
            RightMotor.SubsribeToNewReadings(this);

            this.Shape = new Vector<double>[4];
            this.Shape[0] = new Vector<double>(new double[] { 11.5, 15, 0, 0 });
            this.Shape[1] = new Vector<double>(new double[] { 11.5, -14, 0, 0 });
            this.Shape[2] = new Vector<double>(new double[] { -11.5, -14, 0, 0 });
            this.Shape[3] = new Vector<double>(new double[] { -11.5, 15, 0, 0 });

        }

    }
}
