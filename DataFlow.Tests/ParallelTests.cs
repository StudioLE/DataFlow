using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioLE.DataFlow;

namespace DataFlow.Tests
{
    [TestFixture, Category("Parallel")]
    public class ParallelTests
    {
        private int Count { get; } = 12;

        private List<Monitor> Monitors { get; set; }

        [SetUp]
        public void Setup()
        {
            Monitors = Enumerable.Range(1, Count)
                .Select(x => new Monitor(x))
                .ToList();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void MaxDegreeOfParallelism(int maxDegreeOfParallelism)
        {
            // Create a DataFlow
            var dataFlow = new DataFlow<Monitor, Monitor>()
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                }
                .Add<Monitor, Monitor>(x => x.SetStart())
                .Add<Monitor, Monitor>(Utils.Wait)
                .Add<Monitor, Monitor>(Utils.Wait)
                .Add<Monitor, Monitor>(x => x.SetEnd());

            // Execute the DataFlow for all monitors
            List<Task<Monitor>> tasks = Monitors.Select(dataFlow.Execute).ToList();

            // Wait for all tasks to complete
            Task.WaitAll(tasks.ToArray());

            // Extract the results
            List<Monitor> results = tasks.Select(x => x.Result).ToList();

            // Extract the end times
            List<double> endTimes = results.Select(x => Utils.RoundToPointFive(x.End)).ToList();

            // Print all with Monitor.ToString()
            results.ForEach(Console.WriteLine);

            // Expected results
            List<double> expect = Utils.SteppedRange(1, (Count / maxDegreeOfParallelism), 0.5)
                .SelectMany(x => Enumerable.Repeat(x, maxDegreeOfParallelism))
                .ToList();
            //List<double> expect = new List<double>
            //{
            //    1,   1,   1,
            //    1.5, 1.5, 1.5,
            //    2,   2,   2,
            //    2.5, 2.5, 2.5
            //};
            Console.WriteLine();
            Assert.IsTrue(Utils.CompareLists(expect, endTimes), "Expected DataFlows to complete execution in specific time");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void BoundedCapacity(int boundedCapacity)
        {
            // Create a DataFlow
            var dataFlow = new DataFlow<Monitor, Monitor>()
                {
                    MaxDegreeOfParallelism = -1,
                    BoundedCapacity = boundedCapacity
                }
                .Add<Monitor, Monitor>(x => x.SetStart())
                .Add<Monitor, Monitor>(Utils.Wait)
                .Add<Monitor, Monitor>(Utils.Wait)
                .Add<Monitor, Monitor>(x => x.SetEnd());

            // Execute the DataFlow for all monitors
            List<Task<Monitor>> tasks = Monitors.Select(dataFlow.Execute).ToList();

            // Wait for all tasks to complete
            Task.WaitAll(tasks.ToArray());

            // Extract the results
            List<Monitor> results = tasks.Select(x => x.Result).ToList();

            // Extract the end times
            List<double> endTimes = results.Select(x => Utils.RoundToPointFive(x.End)).ToList();

            // Print all with Monitor.ToString()
            results.ForEach(Console.WriteLine);

            // Expected results
            List<double> expect = Utils.SteppedRange(1, (Count / boundedCapacity), 0.5)
                .SelectMany(x => Enumerable.Repeat(x, boundedCapacity))
                .ToList();
            //List<double> expect = new List<double>
            //{
            //    1,   1,   1,
            //    1.5, 1.5, 1.5,
            //    2,   2,   2,
            //    2.5, 2.5, 2.5
            //};
            Console.WriteLine();
            Assert.IsTrue(Utils.CompareLists(expect, endTimes), "Expected DataFlows to complete execution in specific time");
        }
    }
}