using System.Numerics;

namespace Capstone
{
    public class SimulatedRobot : Robot
    {
        private double MaxSpeed;
        private double MaxRotationalSpeed;
        private double AccelerationRate;
        private double RotaionalAccelerationRate;
        private double CurrentAcceleration;
        private double CurrentRotationalAcceleration;

        public SimulatedRobot()
        {
            this.Shape = new System.Numerics.Vector<double>[4];
            this.Shape[0] = new Vector<double>(new double[] { 5, 10, 0, 0 });
            this.Shape[1] = new Vector<double>(new double[] { 5, -10, 0, 0 });
            this.Shape[2] = new Vector<double>(new double[] { -5, -10, 0, 0 });
            this.Shape[3] = new Vector<double>(new double[] { -5, 10, 0, 0 });
        }
    }
}
