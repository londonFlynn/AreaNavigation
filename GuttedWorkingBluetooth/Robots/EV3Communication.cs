using Lego.Ev3.Core;
using RoboticNavigation.MovementControls;
using RoboticNavigation.Sensors;
using RoboticNavigation.Sensors.SensorReadings;
using RoboticNavigation.VectorMath;
using System;
using System.Diagnostics;

namespace RoboticNavigation.Robots
{
    public class EV3Communication : RoboticCommunication
    {

        public const double ReverseTurnModifier = 0.8;
        private ICommunication coms;
        EV3Robot Robot;
        private Brick brick;
        private readonly OutputPort leftDrive = OutputPort.B;
        private readonly OutputPort rightDrive = OutputPort.C;

        public EV3Communication(EV3Robot robot)
        {
            this.Robot = robot;
            //coms = new Lego.Ev3.WinRT.UsbCommunication();
            //coms = new Lego.Ev3.WinRT.BluetoothCommunication();
            coms = new Lego.Ev3.Desktop.BluetoothCommunication("COM3");

            brick = new Brick(coms);
            brick.BrickChanged += OnBrickChanged;
            Connect();

            Robot.LeftMotor.ReadingValueOffset = brick.Ports[InputPortFromOutputPort(leftDrive)].RawValue;
            Robot.RightMotor.ReadingValueOffset = brick.Ports[InputPortFromOutputPort(rightDrive)].RawValue;
        }
        private async void Connect()
        {
            Debug.WriteLine("Attempting to connect to brick...");
            await brick.ConnectAsync();
            Debug.WriteLine("Connection successful");
            foreach (var port in Enum.GetValues(typeof(InputPort)))
            {
                switch (brick.Ports[(InputPort)port].Type)
                {
                    case DeviceType.Gyroscope:
                        brick.Ports[(InputPort)port].SetMode(GyroscopeMode.Angle);
                        break;
                    case DeviceType.Infrared:
                        brick.Ports[(InputPort)port].SetMode(InfraredMode.Proximity);
                        break;
                    case DeviceType.Ultrasonic:
                        brick.Ports[(InputPort)port].SetMode(UltrasonicMode.Centimeters);
                        break;
                    case DeviceType.LMotor:
                        brick.Ports[(InputPort)port].SetMode(MotorMode.Degrees);
                        break;
                }
            }
            Debug.WriteLine("Sensor Setup Successful");
        }

        public async override void CommandMove(MovementDirection movementCommandState, double power)
        {
            double leftPower = 0;
            double rightPower = 0;
            switch (movementCommandState)
            {
                case MovementDirection.LEFT:
                    leftPower = -power;
                    rightPower = power * ReverseTurnModifier;
                    break;
                case MovementDirection.RIGHT:
                    leftPower = power * ReverseTurnModifier;
                    rightPower = -power;
                    break;
                case MovementDirection.FORWARD:
                    leftPower = power;
                    rightPower = power;
                    break;
                case MovementDirection.REVERSE:
                    leftPower = -power;
                    rightPower = -power;
                    break;
            }
            await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, (int)(leftPower * 100));
            await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, (int)(rightPower * 100));
            //brick.BatchCommand.TurnMotorAtPower(leftDrive, (int)(leftPower * 100));
            //brick.BatchCommand.TurnMotorAtPower(rightDrive, (int)(rightPower * 100));
            //await brick.BatchCommand.SendCommandAsync();
        }
        public void OnBrickChanged(object sender, BrickChangedEventArgs e)
        {
            foreach (var port in Enum.GetValues(typeof(InputPort)))
            {
                switch (brick.Ports[(InputPort)port].Type)
                {
                    case DeviceType.Gyroscope:
                        //Debug.WriteLine(e.Ports[(InputPort)port].SIValue);
                        UpdateGyroValue(brick.Ports[(InputPort)port]);
                        break;
                    case DeviceType.Infrared:
                        UpdateRangeSensorValue(brick.Ports[(InputPort)port], (InputPort)port);
                        break;
                    case DeviceType.Ultrasonic:
                        UpdateRangeSensorValue(brick.Ports[(InputPort)port], (InputPort)port);
                        break;
                }
            }
            UpdateRotaionSensor(brick.Ports[InputPortFromOutputPort(leftDrive)], true);
            //right motor is updated last
            UpdateRotaionSensor(brick.Ports[InputPortFromOutputPort(rightDrive)], false);
        }
        private InputPort InputPortFromOutputPort(OutputPort outputPort)
        {
            InputPort result = InputPort.A;
            switch (outputPort)
            {
                case OutputPort.A:
                    result = InputPort.A;
                    break;
                case OutputPort.B:
                    result = InputPort.B;
                    break;
                case OutputPort.C:
                    result = InputPort.C;
                    break;
                case OutputPort.D:
                    result = InputPort.D;
                    break;
                default:
                    throw new ArgumentException("Output port cannot be mapped to an input port");
            }
            return result;
        }
        private void UpdateGyroValue(Port port)
        {
            if (!(Robot.Gyro is null))
                Robot.Gyro.SetRecentReading(new GyroscopeReading(-port.RawValue));
        }
        private void UpdateRotaionSensor(Port port, bool left)
        {
            if (!(Robot.LeftMotor is null) && !(Robot.RightMotor is null))
            {

                if (left)
                {
                    Robot.LeftMotor.SetRecentReading(new RotationSensorReading(port.RawValue));
                }
                else
                {
                    Robot.RightMotor.SetRecentReading(new RotationSensorReading(port.RawValue));
                }
            }
        }
        private double InfraredDataToCM(double data)
        {
            double result;
            if (data <= 38.983)
            {
                result = 20535640 + ((5.352759 - 20535640) / (1 + Math.Pow(data / 1017575, 1.320567)));
            }
            else
            {
                result = 103623100 + ((14.33844 - 103623100) / (1 + Math.Pow(data / 920.7437, 4.870784)));
            }

            return (200 * (data / 100d)) - 70;
            return result - 20;
        }
        private void UpdateRangeSensorValue(Port port, InputPort inputPort)
        {
            if (Robot.RangeSensorPorts.ContainsKey(inputPort) && !(Robot.RangeSensorPorts[inputPort] is null) && !(Robot.Gyro.GetCurrentReading() is null) && port.RawValue > 1)
            {
                var sensor = Robot.RangeSensorPorts[inputPort];
                var angleAdjust = sensor.AngleAdjustment;
                RangeReading reading = null;
                if (sensor is UltrasonicSensor)
                {
                    reading = new UltrasonicRangeReading(
                        (port.RawValue + 0d) / 10,
                        sensor.RelativePosition.Rotate(Robot.Orientation) + Robot.Position,
                        (Robot.Gyro.GetCurrentReading() as GyroscopeReading).Radians,
                        angleAdjust,
                        sensor.SensorFalloffDistance);
                }
                else if (sensor is InfraredSensor)
                {
                    reading = new InfraredRangeReading(
                        InfraredDataToCM(port.RawValue),
                        sensor.RelativePosition.Rotate(Robot.Orientation) + Robot.Position,
                        (Robot.Gyro.GetCurrentReading() as GyroscopeReading).Radians,
                        angleAdjust,
                        sensor.SensorFalloffDistance);
                }
                sensor.SetRecentReading(reading);
            }
        }
        public override void StartGettingSensorReadings(Sensor sensor)
        {

        }
    }
}
