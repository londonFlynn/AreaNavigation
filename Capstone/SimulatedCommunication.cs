namespace Capstone
{
    public class SimulatedCommunication : RoboticCommunication
    {
        private SimulatedRobot SimBot;
        public SimulatedCommunication(SimulatedRobot bot)
        {
            this.SimBot = bot;
        }
        public override void CommandMove(MovementCommandState movementCommandState, double Power = 1)
        {
            //throw new System.NotImplementedException();
        }

        public override void StartGettingSensorReadings(Sensor sensor)
        {

        }
    }
}
