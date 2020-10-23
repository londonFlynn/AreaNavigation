namespace Capstone
{
    public class SimulatedWall : Wall
    {
        public SimulatedWall(int range = 200)
        {
            var rand = new System.Random();
            this.StartPosition = new Vector<double>(new double[] { (rand.NextDouble() - 0.5) * range, (rand.NextDouble() - 0.5) * range, 0, 0 });
            this.EndPosition = new Vector<double>(new double[] { (rand.NextDouble() - 0.5) * range, (rand.NextDouble() - 0.5) * range, 0, 0 });
        }
    }
}
