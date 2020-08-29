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

        public static double RoundToPointFive(double number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number), number, "Number must be positive");
            return Math.Round(number / 0.5) * 0.5;
        }

        public static IEnumerable<double> SteppedRange(double start, int count, double step)
        {
            return Enumerable.Range(0, count)
                .Select(x => x * step + start);
        }

        public static bool CompareLists<TListItem>(List<TListItem> l1, List<TListItem> l2)
        {
            bool equal = true;
            int count = Math.Max(l1.Count, l2.Count);

            Console.WriteLine($"Index\t\tExpect\t\tResult");

            for (int i = 0; i < count; i++)
            {
                TListItem v1 = l1.ElementAtOrDefault(i);
                TListItem v2 = l2.ElementAtOrDefault(i);

                // Set console colour based on equality
                if (v1.Equals(v2))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    equal = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.WriteLine($"{i}\t\t\t{v1}\t\t\t{v2}");
                Console.ResetColor();
            }

            return equal;
        }
    }
}
