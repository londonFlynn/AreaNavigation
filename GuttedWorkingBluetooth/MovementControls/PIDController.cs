using System;

namespace RoboticNavigation.MovementControls
{
    public class PIDController
    {
        protected double Percentage { get; set; }
        protected double Integral { get; set; }
        protected double Derivative { get; set; }
        protected double Target { get; set; }
        protected double MarginOfError { get; set; }
        protected MovementDirection Direction;
        protected CallOnPIDFinished PIDFinished;
        protected ReceivePIDControlledMotorPower MotorPower;
        protected bool Started;
        protected DateTime StartTime;

        public PIDController(double percent, double integral, double derivative, double target, double marginOfError, MovementDirection direction, CallOnPIDFinished pIDFinished, ReceivePIDControlledMotorPower motorPower)
        {
            this.Percentage = percent;
            this.Integral = integral;
            this.Derivative = derivative;
            this.Target = target;
            this.MarginOfError = marginOfError;
            this.Direction = direction;
            this.PIDFinished += pIDFinished;
            this.MotorPower += motorPower;
        }
        public void Execute()
        {
            this.StartTime = DateTime.Now;
            Started = true;
        }
        public void Finish()
        {
            this.Started = false;
            PIDFinished.DynamicInvoke(this);
        }
        public void RecieveCurrentValue(double value)
        {
            if (Started)
            {
                //TODO Complicated Math



                SendPowerCommand(1);

            }
        }
        private void SendPowerCommand(double power)
        {
            MotorPower.DynamicInvoke(Direction, power);
        }
    }
    public delegate void CallOnPIDFinished(PIDController caller);
    public delegate void ReceivePIDControlledMotorPower(MovementDirection movementCommandState, double power);
}
