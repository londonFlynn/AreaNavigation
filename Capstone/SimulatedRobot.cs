using System.Numerics;

namespace Capstone
{
    public class SimulatedRobot : Robot
    {
        public double MaxSpeed { get; private set; }
        public double MaxRotationalSpeed { get; private set; }
        public readonly double AccelerationRate = 1;
        public readonly double RotaionalAccelerationRate = 1;
        public SimulatedRobot()
        {
            this.Shape = new System.Numerics.Vector<double>[4];
            this.Shape[0] = new Vector<double>(new double[] { 5, 10, 0, 0 });
            this.Shape[1] = new Vector<double>(new double[] { 5, -10, 0, 0 });
            this.Shape[2] = new Vector<double>(new double[] { -5, -10, 0, 0 });
            this.Shape[3] = new Vector<double>(new double[] { -5, 10, 0, 0 });
            this.RoboticCommunication = new SimulatedCommunication(this);
        }

        protected override void UpdatePosition()
        {
            throw new System.NotImplementedException();
        }
    }
}
