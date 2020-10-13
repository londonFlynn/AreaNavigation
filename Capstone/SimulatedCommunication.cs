using System.Threading.Tasks;

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
            throw new System.NotImplementedException();
        }

        public override void StartGettingSensorReadings(Sensor sensor)
        {
            if (sensor is GyroscopeSensor)
            {
                Task.Run(() => GyroscopeReading((GyroscopeSensor)sensor));
            }
            else if (sensor is AccelerometerSensor)
            {
                Task.Run(() => AccelerometerReading((AccelerometerSensor)sensor));
            }
        }
        private void GyroscopeReading(GyroscopeSensor sensor)
        {
            do
            {
                double reading = ((GyroscopeReading)sensor.GetCurrentReading()).RotationalAcceleration;
                if (SimBot.MovementCommandState == MovementCommandState.RIGHT && sensor.CurrentRotationalSpeed > -SimBot.MaxRotationalSpeed ||
                    SimBot.MovementCommandState != MovementCommandState.LEFT && sensor.CurrentRotationalSpeed > 0)
                {
                    reading -= SimBot.RotaionalAccelerationRate;
                }
                else if (SimBot.MovementCommandState == MovementCommandState.LEFT && sensor.CurrentRotationalSpeed < SimBot.MaxRotationalSpeed ||
                    SimBot.MovementCommandState != MovementCommandState.RIGHT && sensor.CurrentRotationalSpeed < 0)
                {
                    reading += SimBot.RotaionalAccelerationRate;
                }
                sensor.SetRecentReading(new GyroscopeReading(reading));
            } while (true);
        }
        private void AccelerometerReading(AccelerometerSensor sensor)
        {
            do
            {
                double reading = ((AccelerometerReading)sensor.GetCurrentReading()).Acceleration;
                if (SimBot.MovementCommandState == MovementCommandState.FORWARD && sensor.CurrentSpeed < SimBot.MaxSpeed ||
                    SimBot.MovementCommandState != MovementCommandState.REVERSE && sensor.CurrentSpeed < 0 ||
                    sensor.CurrentSpeed < -SimBot.MaxSpeed)
                {
                    reading += SimBot.AccelerationRate;
                }
                else if (SimBot.MovementCommandState == MovementCommandState.REVERSE && sensor.CurrentSpeed > -SimBot.MaxSpeed ||
                    SimBot.MovementCommandState != MovementCommandState.FORWARD && sensor.CurrentSpeed > 0 ||
                    sensor.CurrentSpeed > SimBot.MaxSpeed)
                {
                    reading -= SimBot.AccelerationRate;
                }
                sensor.SetRecentReading(new AccelerometerReading(reading));
            } while (true);
        }
    }
}
