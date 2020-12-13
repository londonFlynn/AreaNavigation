using System;
using System.Threading;

namespace RoboticNavigation.MovementControls
{
    public class PID
    {
        private double PGain;
        private double IGain;
        private double DGain;

        private DateTime LastUpdateTime;
        private double LastPorportialValue;
        private double ErrorSum;

        private GetDouble ReadPorportionalValue;
        private GetDouble ReadSetPoint;
        private SetDouble WriteOutputValue;

        private double PorportionalValueMax;
        private double PorportionalValueMin;
        private double OutputValueMax;
        private double OutputValueMin;

        private double ComputeHz = 10.0f;
        private Thread ComputeThread;


        public double PorpotionalGain
        {
            get { return PGain; }
            set { PGain = value; }
        }

        public double IntigralGain
        {
            get { return IGain; }
            set { IGain = value; }
        }

        public double DerivativeGain
        {
            get { return DGain; }
            set { DGain = value; }
        }

        public double PorportionalMin
        {
            get { return PorportionalValueMin; }
            set { PorportionalValueMin = value; }
        }

        public double PorportionalMax
        {
            get { return PorportionalValueMax; }
            set { PorportionalValueMax = value; }
        }

        public double OutputMin
        {
            get { return OutputValueMin; }
            set { OutputValueMin = value; }
        }

        public double OutputMax
        {
            get { return OutputValueMax; }
            set { OutputValueMax = value; }
        }

        public bool PIDOK
        {
            get { return ComputeThread != null; }
        }


        public PID(double pGain, double iGain, double dGain,
            double porportionalMax, double porportionalMin, double outputMax, double outputMin,
            GetDouble porportionalFunction, GetDouble setpointFunction, SetDouble outputFunction)
        {
            PGain = pGain;
            IGain = iGain;
            DGain = dGain;
            PorportionalValueMax = porportionalMax;
            PorportionalValueMin = porportionalMin;
            OutputValueMax = outputMax;
            OutputValueMin = outputMin;
            ReadPorportionalValue = porportionalFunction;
            ReadSetPoint = setpointFunction;
            WriteOutputValue = outputFunction;
        }

        ~PID()
        {
            Disable();
            ReadPorportionalValue = null;
            ReadSetPoint = null;
            WriteOutputValue = null;
        }

        public void Enable()
        {
            if (ComputeThread != null)
                return;

            Reset();

            ComputeThread = new Thread(new ThreadStart(Run));
            ComputeThread.IsBackground = true;
            ComputeThread.Name = "PID Processor";
            ComputeThread.Start();
        }

        public void Disable()
        {
            if (ComputeThread == null)
                return;

            ComputeThread.Abort();
            ComputeThread = null;
        }

        public void Reset()
        {
            ErrorSum = 0.0f;
            LastUpdateTime = DateTime.Now;
        }

        private double ScaleValue(double value, double valuemin,
                double valuemax, double scalemin, double scalemax)
        {
            double vPerc = (value - valuemin) / (valuemax - valuemin);
            double bigSpan = vPerc * (scalemax - scalemin);

            double retVal = scalemin + bigSpan;

            return retVal;
        }

        private double ClampValueToWithinMinAndMax(double value, double min, double max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        private void Compute()
        {
            if (ReadPorportionalValue == null || ReadSetPoint == null || WriteOutputValue == null)
                return;

            double porportionalValue = ReadPorportionalValue();
            double setPoint = ReadSetPoint();

            porportionalValue = ClampValueToWithinMinAndMax(porportionalValue, PorportionalValueMin, PorportionalValueMax);
            porportionalValue = ScaleValue(porportionalValue, PorportionalValueMin, PorportionalValueMax, -1.0f, 1.0f);

            setPoint = ClampValueToWithinMinAndMax(setPoint, PorportionalValueMin, PorportionalValueMax);
            setPoint = ScaleValue(setPoint, PorportionalValueMin, PorportionalValueMax, -1.0f, 1.0f);

            double error = setPoint - porportionalValue;

            double pTerm = error * PGain;
            double iTerm = 0.0f;
            double dTerm = 0.0f;

            double partialSum = 0.0f;
            DateTime nowTime = DateTime.Now;

            if (LastUpdateTime != null)
            {
                double dT = (nowTime - LastUpdateTime).TotalSeconds;

                if (porportionalValue >= PorportionalValueMin && porportionalValue <= PorportionalValueMax)
                {
                    partialSum = ErrorSum + dT * error;
                    iTerm = IGain * partialSum;
                }

                if (dT != 0.0f)
                    dTerm = DGain * (porportionalValue - LastPorportialValue) / dT;
            }

            LastUpdateTime = nowTime;
            ErrorSum = partialSum;
            LastPorportialValue = porportionalValue;
            double outReal = pTerm + iTerm + dTerm;

            outReal = ClampValueToWithinMinAndMax(outReal, -1.0f, 1.0f);
            outReal = ScaleValue(outReal, -1.0f, 1.0f, OutputValueMin, OutputValueMax);

            WriteOutputValue(outReal);
        }


        private void Run()
        {
            while (true)
            {
                try
                {
                    int sleepTime = (int)(1000 / ComputeHz);
                    Thread.Sleep(sleepTime);
                    Compute();
                }
                catch (Exception e)
                {

                }
            }
        }

    }
    public delegate double GetDouble();
    public delegate void SetDouble(double value);
}
