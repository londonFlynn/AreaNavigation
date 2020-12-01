using RoboticNavigation.MovementControls;
using RoboticNavigation.Sensors;

namespace RoboticNavigation.Robots
{
    public abstract class RoboticCommunication
    {
        public abstract void CommandMove(MovementDirection movementCommandState, double power);
        public abstract void StartGettingSensorReadings(Sensor sensor);
    }
}
