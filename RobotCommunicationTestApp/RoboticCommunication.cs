namespace Capstone
{
    public abstract class RoboticCommunication
    {
        public abstract void CommandMove(MovementCommandState movementCommandState, double power);
        public abstract void StartGettingSensorReadings(Sensor sensor);
    }
}
