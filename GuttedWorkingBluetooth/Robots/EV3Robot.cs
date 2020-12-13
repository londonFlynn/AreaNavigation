using Lego.Ev3.Core;
using Newtonsoft.Json;
using RoboticNavigation.Sensors;
using RoboticNavigation.VectorMath;
using System;
using System.Collections.Generic;
using System.IO;

namespace RoboticNavigation.Robots
{
    public class EV3Robot : Robot
    {
        public Dictionary<InputPort, RangeSensor> RangeSensorPorts;
        private EV3Robot(double turnPercent, double turnIntegral, double turnDerivative, double turnError, double distancePercent, double distanceIntegral, double distanceDerivative, double distanceError, string imageFileName) :
            base(turnPercent, turnIntegral, turnDerivative, turnError, distancePercent, distanceIntegral, distanceDerivative, distanceError, imageFileName)
        {
            this.RangeSensorPorts = new Dictionary<InputPort, RangeSensor>();
        }
        public static EV3Robot Load()
        {
            var path = System.IO.Path.Combine(Environment.CurrentDirectory);
            path = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, @"..\..\"));
            path = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, "Assets", ApplicationConfig.RobotDataFilePath));
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    dynamic config = JsonConvert.DeserializeObject(json);
                    return ParseConfig(config);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
                //return Default();
            }
        }
        private static EV3Robot ParseConfig(dynamic config)
        {
            var robot = new EV3Robot((double)config.TurnPorportion, (double)config.TurnIntegral, (double)config.TurnDerivative, (double)config.TurnError, (double)config.DistancePorportion, (double)config.DistanceIntegral, (double)config.DistanceDerivative, (double)config.DistanceError, (string)config.ImageFileName);
            robot.Gyro = new GyroscopeSensor();
            robot.LeftMotor = new RotationSensor(true, 0, (double)config.Motors.Left.DistancePerRotation);
            robot.RightMotor = new RotationSensor(false, 0, (double)config.Motors.Right.DistancePerRotation);

            foreach (var sensorConfig in config.RangeSensors)
            {
                RangeSensorConfig(robot, sensorConfig);
            }

            robot.RoboticCommunication = new EV3Communication(robot);
            robot.Gyro.SubsribeToNewReadings(robot);
            robot.LeftMotor.SubsribeToNewReadings(robot);
            robot.RightMotor.SubsribeToNewReadings(robot);
            var shape = new List<Vector2d<double>>();
            foreach (var vector in config.Shape)
            {
                shape.Add(new Vector2d<double>(new double[] { vector.X, vector.Y }));
            }
            robot.Shape = shape.ToArray();
            return robot;
        }
        private static void RangeSensorConfig(EV3Robot robot, dynamic sensorConfig)
        {
            var pos = new Vector2d<double>(new double[] { sensorConfig.RelativeX, sensorConfig.RelativeY });
            double falloff = sensorConfig.Falloff;
            int portNum = sensorConfig.Port;
            double angle = sensorConfig.Angle;
            InputPort port;
            switch (portNum)
            {
                default:
                case 1:
                    port = InputPort.One;
                    break;
                case 2:
                    port = InputPort.Two;
                    break;
                case 3:
                    port = InputPort.Three;
                    break;
                case 4:
                    port = InputPort.Four;
                    break;
            }

            RangeSensor sensor = null;
            if (((string)sensorConfig.Type).Equals("Ultrasonic"))
            {
                sensor = new UltrasonicSensor(pos, falloff, angle);
            }
            else if (((string)sensorConfig.Type).Equals("Infrared"))
            {
                sensor = new InfraredSensor(pos, falloff, angle);

            }
            robot.RangeSensors.Add(sensor);
            robot.RangeSensorPorts.Add(port, sensor);
        }
        //public static EV3Robot Default()
        //{
        //    var robot = new EV3Robot(5, 6, 0.1, System.Math.PI / 90, 3, 0.1, 0.1, 5, "Robot.png");
        //    robot.Gyro = new GyroscopeSensor();
        //    robot.LeftMotor = new RotationSensor(true, 0, 1.5);
        //    robot.RightMotor = new RotationSensor(false, 0, 1.5);
        //    var IRSensor = new InfraredSensor(new Vector2d<double>(new double[] { -5, 3 }), 120);
        //    var USSensor = new UltrasonicSensor(new Vector2d<double>(new double[] { 5, 3 }), 120);
        //    robot.RangeSensors.Add(IRSensor);
        //    robot.RangeSensors.Add(USSensor);
        //    robot.RangeSensorPorts.Add(InputPort.Two, USSensor);
        //    robot.RoboticCommunication = new EV3Communication(robot);
        //    robot.Gyro.SubsribeToNewReadings(robot);
        //    robot.LeftMotor.SubsribeToNewReadings(robot);
        //    robot.RightMotor.SubsribeToNewReadings(robot);
        //    robot.Shape = new Vector2d<double>[4];
        //    robot.Shape[0] = new Vector2d<double>(new double[] { 11.5, 15.3, 0, 0 });
        //    robot.Shape[1] = new Vector2d<double>(new double[] { 11.5, -15.3, 0, 0 });
        //    robot.Shape[2] = new Vector2d<double>(new double[] { -11.5, -15.3, 0, 0 });
        //    robot.Shape[3] = new Vector2d<double>(new double[] { -11.5, 15.3, 0, 0 });
        //    return robot;
        //}

    }
}
