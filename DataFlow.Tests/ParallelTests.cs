using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StudioLE.DataFlow;
using StudioLE.Geometry;

namespace DataFlow.Tests
{
    public class ParallelTests
    {
        private const int waitTime = 500;

        private Stopwatch start;

        private int count;

        private List<int> range;

        [SetUp]
        public void Setup()
        {
            start = Stopwatch.StartNew();
            count = 12;
            range = Enumerable.Range(1, count).ToList();
        }

        [TestCase(1)]
        [TestCase(3)]
        public void MaxDegreeOfParallelism(int maxDegreeOfParallelism)
        {
            // Create a DataFlow
            var flow = new DataFlow<int, double>()
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                }
                .Add<int, int>(x => Print(x, "START"))
                .Add<int, int>(Wait)
                .Add<int, int>(Wait)
                .Add<int, int>(x => Print(x, "END"))
                .Add<int, double>(TimeSinceStart);

            // Execute the DataFlow for all numbers in range
            List<Task<double>> tasks = range.Select(flow.Execute).ToList();

            // Wait for all tasks to complete
            Task.WaitAll(tasks.ToArray());

            // Extract the results
            List<double> results = tasks.Select(x => x.Result).ToList();

            // Expected results
            List<double> expect = SteppedRange(1, (count / maxDegreeOfParallelism), 0.5)
                .SelectMany(x => Enumerable.Repeat(x, maxDegreeOfParallelism))
                .ToList();
            //List<double> expect = new List<double>
            //{
            //    1,   1,   1,
            //    1.5, 1.5, 1.5,
            //    2,   2,   2,
            //    2.5, 2.5, 2.5
            //};

            Assert.IsTrue(CompareLists(expect, results), "Expected DataFlows to complete execution in specific time");
        }

        private int Wait(int flowIndex, int ms)
        {
            Thread.Sleep(ms);
            return flowIndex;
        }

        private int Wait(int flowIndex)
        {
            return Wait(flowIndex, waitTime);
        }

        private int Print(int flowIndex, string stepName)
        {
            double seconds = Math.Round(start.Elapsed.TotalSeconds, 3);
            Console.WriteLine($"DataFlow {flowIndex} has reached {stepName} after {seconds} seconds");
            return flowIndex;
        }

        private double TimeSinceStart(int flowIndex)
        {
            //return Math.Round(start.Elapsed.TotalSeconds, 3)
            return RoundToPointFive(start.Elapsed.TotalSeconds);
        }

        private double RoundToPointFive(double number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number), number, "Number must be positive");
            return Math.Round(number / 0.5) * 0.5;
        }

        private IEnumerable<double> SteppedRange(double start, int count, double step)
        {
            return Enumerable.Range(0, count)
                .Select(x => x * step + start);
        }

        private bool CompareLists<TListItem>(List<TListItem> l1, List<TListItem> l2)
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