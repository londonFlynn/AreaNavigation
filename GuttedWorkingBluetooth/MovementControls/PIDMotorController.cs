using System;

namespace RoboticNavigation.MovementControls
{
    public class PIDMotorController
    {
        protected double Porportional { get; set; }
        protected double Integral { get; set; }
        protected double Derivative { get; set; }
        protected double Target { get; set; }
        public double MarginOfError { get; protected set; }
        protected MovementDirection Direction;
        protected CallOnPIDFinished PIDFinished;
        protected ReceivePIDControlledMotorPower MotorPower;
        protected GetDouble DistanceFromTarget;
        protected bool Started;
        protected PID PID;

        public PIDMotorController(double percent, double integral, double derivative, double target, GetDouble current, double marginOfError, MovementDirection direction, CallOnPIDFinished pIDFinished, ReceivePIDControlledMotorPower motorPower)
        {
            this.Porportional = percent;
            this.Integral = integral;
            this.Derivative = derivative;
            this.Target = target;
            this.MarginOfError = marginOfError;
            this.Direction = direction;
            this.PIDFinished += pIDFinished;
            this.MotorPower += motorPower;
            this.DistanceFromTarget = current;
        }
        public void Execute()
        {
            Started = true;
            CreatePID();
            PID.Enable();
            SendPowerCommand(1);
        }
        public void Abort()
        {
            this.Started = false;
            PID.Disable();
        }
        private void Complete()
        {
            Abort();
            PIDFinished.DynamicInvoke(this);
        }
        private bool Completed()
        {
            return Math.Abs(GetCurrent() - Target) <= MarginOfError;
        }
        private double GetCurrent()
        {
            return (Target - DistanceFromTarget.Invoke());
        }
        private double GetTarget()
        {
            return Target;
        }
        private void CreatePID()
        {
            double processMax = Target;
            double processMin = -Target;
            PID = new PID(Porportional, Integral, Derivative, processMax, processMin, Target, -Target, GetCurrent, GetTarget, SendPowerCommand);

        }
        private void SendPowerCommand(double power)
        {
            if (Completed())
            {
                Complete();
            }
            power = power / Target;
            power = Math.Min(1, power);
            power = Math.Max(-1, power);
            if (GetCurrent() > Target)
            {
                power = -power;
            }

            if (!Double.IsNaN(power) && power >= -1 && power <= 1)
                MotorPower.DynamicInvoke(Direction, power);
        }
        public override string ToString()
        {
            return $"Proportional: {Porportional}\r\nIntegral: {Integral}\r\nDerivative: {Derivative}\r\nMargin: {MarginOfError}\r\nTarget: {Target}\r\nCurrent: {GetCurrent()}";
        }
    }
    public delegate void CallOnPIDFinished(PIDMotorController caller);
    public delegate void ReceivePIDControlledMotorPower(MovementDirection movementCommandState, double power);
}
