using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataFlow.Tests
{
    public class Utils
    {
        private static int WaitTime { get; } = 500;

        public static TInput Wait<TInput>(TInput input, int ms)
        {
            Thread.Sleep(ms);
            return input;
        }

        public static TInput Wait<TInput>(TInput input)
        {
            return Wait(input, WaitTime);
        }
    }
}
