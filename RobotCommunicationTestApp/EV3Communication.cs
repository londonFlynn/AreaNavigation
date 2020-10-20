using Lego.Ev3.Core;
using System;
using System.Diagnostics;

namespace Capstone
{
    public class EV3Communication : RoboticCommunication
    {
        private Lego.Ev3.Core.ICommunication coms;
        private GyroscopeSensor Gyro;
        private RotationSensor LeftMotor;
        private RotationSensor RightMotor;
        private InfraredSensor IRSensor;
        private UltrasonicSensor USSensor;
        private Brick brick;
        private readonly OutputPort leftDrive = OutputPort.B;
        private readonly OutputPort rightDrive = OutputPort.C;
        public EV3Communication()
        {
            //coms = new Lego.Ev3.WinRT.BluetoothCommunication();
            //coms = new Lego.Ev3.WinRT.UsbCommunication();
            //coms = new Lego.Ev3.Desktop.UsbCommunication();
            coms = new Lego.Ev3.Desktop.BluetoothCommunication("com3");

            brick = new Brick(coms);
            brick.BrickChanged += OnBrickChanged;
            Connect();
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

        public async override void CommandMove(MovementCommandState movementCommandState, double power)
        {
            Debug.WriteLine("Sending move command to EV3");
            double leftPower = 0;
            double rightPower = 0;
            switch (movementCommandState)
            {
                case MovementCommandState.LEFT:
                    leftPower = -power;
                    rightPower = power;
                    break;
                case MovementCommandState.RIGHT:
                    leftPower = power;
                    rightPower = -power;
                    break;
                case MovementCommandState.FORWARD:
                    leftPower = power;
                    rightPower = power;
                    break;
                case MovementCommandState.REVERSE:
                    leftPower = -power;
                    rightPower = -power;
                    break;
            }
            //await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, (int)-(leftPower * 100));
            //await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, (int)(rightPower * 100));
            brick.BatchCommand.TurnMotorAtPower(leftDrive, (int)-(leftPower * 100));
            brick.BatchCommand.TurnMotorAtPower(rightDrive, (int)(rightPower * 100));
            await brick.BatchCommand.SendCommandAsync();
        }
        void OnBrickChanged(object sender, BrickChangedEventArgs e)
        {
            foreach (var port in Enum.GetValues(typeof(InputPort)))
            {
                switch (e.Ports[(InputPort)port].Type)
                {
                    case DeviceType.Gyroscope:
                        //Debug.WriteLine(e.Ports[(InputPort)port].SIValue);
                        UpdateGyroValue(e.Ports[(InputPort)port]);
                        break;
                    case DeviceType.Infrared:
                        UpdateInfraredValue(e.Ports[(InputPort)port]);
                        break;
                    case DeviceType.Ultrasonic:
                        UpdateUltrasonicValue(e.Ports[(InputPort)port]);
                        break;
                }
            }
            UpdateRotaionSensor(e.Ports[InputPortFromOutputPort(leftDrive)], true);
            //right motor is updated last
            UpdateRotaionSensor(e.Ports[InputPortFromOutputPort(rightDrive)], false);
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
            }
            return result;
        }
        private void UpdateGyroValue(Port port)
        {
            if (!(this.Gyro is null))
                this.Gyro.SetRecentReading(new GyroscopeReading(-port.RawValue));
        }
        private void UpdateRotaionSensor(Port port, bool left)
        {
            if (!(this.LeftMotor is null) && !(this.RightMotor is null))
            {

                if (left)
                {
                    LeftMotor.SetRecentReading(new RotationSensorReading(port.RawValue));
                }
                else
                {
                    RightMotor.SetRecentReading(new RotationSensorReading(port.RawValue));
                }
            }
        }
        private void UpdateInfraredValue(Port port)
        {
            if (!(this.IRSensor is null))
                IRSensor.SetRecentReading(new InfraredRangeReading(port.RawValue));
        }
        private void UpdateUltrasonicValue(Port port)
        {
            if (!(this.USSensor is null))
                USSensor.SetRecentReading(new UltrasonicRangeReading(port.RawValue));
        }
        public override void StartGettingSensorReadings(Sensor sensor)
        {
            if (sensor is UltrasonicSensor)
            {
                this.USSensor = (UltrasonicSensor)sensor;
            }
            else if (sensor is InfraredSensor)
            {
                this.IRSensor = (InfraredSensor)sensor;
            }
            else if (sensor is GyroscopeSensor)
            {
                this.Gyro = (GyroscopeSensor)sensor;
            }
            else if (sensor is RotationSensor)
            {
                if (((RotationSensor)sensor).LeftDrive)
                {
                    this.LeftMotor = (RotationSensor)sensor;
                }
                else
                {
                    this.RightMotor = (RotationSensor)sensor;
                }
            }
        }
    }
}
