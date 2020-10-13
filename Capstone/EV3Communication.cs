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
        private readonly OutputPort leftDrive = OutputPort.C;
        private readonly OutputPort rightDrive = OutputPort.B;
        public EV3Communication()
        {
            //coms = new Lego.Ev3.WinRT.BluetoothCommunication();
            coms = new Lego.Ev3.WinRT.UsbCommunication();

            brick = new Brick(coms);
            brick.BrickChanged += OnBrickChanged;
            //brick.ConnectAsync();
            //connect to robot
        }

        public async override void CommandMove(MovementCommandState movementCommandState, double power)
        {
            switch (movementCommandState)
            {
                case MovementCommandState.LEFT:
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, (int)-(power * 100));
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, (int)(power * 100));
                    break;
                case MovementCommandState.RIGHT:
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, (int)(power * 100));
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, (int)-(power * 100));
                    break;
                case MovementCommandState.FORWARD:
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, (int)(power * 100));
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, (int)(power * 100));
                    break;
                case MovementCommandState.REVERSE:
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, (int)-(power * 100));
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, (int)-(power * 100));
                    break;
                case MovementCommandState.NEUTRAL:
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, 0);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, 0);
                    break;
            }
        }
        void OnBrickChanged(object sender, BrickChangedEventArgs e)
        {
            // print out the value of the sensor on Port 1 (more on this later...)
            Debug.WriteLine(e.Ports[InputPort.One].SIValue);
            foreach (var port in Enum.GetValues(typeof(InputPort)))
            {
                switch (e.Ports[(InputPort)port].Type)
                {
                    case DeviceType.Gyroscope:
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
                this.Gyro.SetRecentReading(new GyroscopeReading(port.RawValue));
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
