using System;
using System.Diagnostics;

namespace DataFlow.Tests
{
    class Monitor
    {
        public int Index { get; }

        public double Start { get; private set; }

        public double End { get; private set; }

        public Stopwatch StopWatch { get; }

        public double Elapsed => StopWatch.Elapsed.TotalSeconds;

        public Monitor(int index)
        {
            Index = index;
            StopWatch = Stopwatch.StartNew();
        }

        public Monitor SetStart()
        {
            Start = Elapsed;
            return this;
        }

        public Monitor SetEnd()
        {
            End = Elapsed;
            return this;
        }

        public override string ToString()
        {
            return $"Monitor: {Index}\t\tStart: {Round(Start)}\t\tEnd: {Round(End)}";
        }

        public static double Round(double number)
        {
            return Math.Round(number, 3);
        }
    }
}
