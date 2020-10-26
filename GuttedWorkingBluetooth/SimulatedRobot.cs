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
            this.Shape = new Vector2d<double>[4];
            this.Shape[0] = new Vector2d<double>(new double[] { 5, 10, 0, 0 });
            this.Shape[1] = new Vector2d<double>(new double[] { 5, -10, 0, 0 });
            this.Shape[2] = new Vector2d<double>(new double[] { -5, -10, 0, 0 });
            this.Shape[3] = new Vector2d<double>(new double[] { -5, 10, 0, 0 });
            this.RoboticCommunication = new SimulatedCommunication(this);
        }

        protected override void UpdatePosition()
        {
            throw new System.NotImplementedException();
        }
    }
}
