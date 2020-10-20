using Capstone;
using Lego.Ev3.Core;
using System;

namespace RobotCommunicationTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new EV3Robot();
            while (true) { }

        }
        static async void BrickSetup()
        {
            var coms = new Lego.Ev3.Desktop.UsbCommunication();
            var brick = new Brick(coms);
            brick.BrickChanged += OnBrickChanged;
            Console.WriteLine("Attempting to connect to brick...");
            await brick.ConnectAsync();
            Console.WriteLine("Brick connection successful");
            Console.WriteLine(brick.Ports[InputPort.One].SIValue);
        }
        static void OnBrickChanged(object sender, BrickChangedEventArgs e)
        {
            // print out the value of the sensor on Port 1 (more on this later...)
            Console.WriteLine(e.Ports[InputPort.One].SIValue);
        }
    }
}
