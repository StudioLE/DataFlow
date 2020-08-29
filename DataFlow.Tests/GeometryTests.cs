using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudioLE.DataFlow;
using StudioLE.Geometry;

namespace DataFlow.Tests
{
    public class GeometryTests
    {
        private List<Solid> Solids { get; set; }

        private DataFlow<List<Solid>, KnownColor> Flow { get; set; }

        [SetUp]
        public void Setup()
        {
            Solids = new List<Solid>
            {
                new Cuboid(1, 1, 1)  { Color = Color.Magenta }, // 0
                new Cuboid(1, 1, 2)  { Color = Color.Cyan },    // 1
                new Cuboid(1, 4, 2)  { Color = Color.Gold },    // 2
                new Cuboid(1, 1, 2)  { Color = Color.Green },   // 3
                new Cuboid(3, 3, 3)  { Color = Color.Fuchsia }, // 4
                new Cube(1)          { Color = Color.Blue },    // 5
                new Cube(2)          { Color = Color.Lime },    // 6
                new Cube(3)          { Color = Color.Purple },  // 7
                new Sphere(0.5)      { Color = Color.Orange },  // 8
                new Sphere(1)        { Color = Color.Yellow },  // 9
                new Sphere(2)        { Color = Color.Red },     // 10
                new Cylinder(0.5, 1) { Color = Color.Indigo },  // 11
                new Cylinder(3, 6)   { Color = Color.Lavender } // 12
            };

            Flow = new DataFlow<List<Solid>, KnownColor>()

                // Cuboids only
                .Add<List<Solid>, List<Cuboid>>(CuboidFilter)

                // Mass greater than 2
                .Add<List<Cuboid>, List<Cuboid>>(input => input.Where(x => x.Mass > 2).ToList())

                // Colours
                .Add<List<Cuboid>, List<Color>>(input => input.Select(x => x.Color).ToList())

                // At least half red
                .Add<List<Color>, List<Color>>(input => input.Where(x => x.R >= 128).ToList())

                // Order by blue
                .Add<List<Color>, List<Color>>(input => input.OrderByDescending(x => x.B).ToList())

                // Highest blue
                .Add<List<Color>, Color>(input => input.First())

                // Name of the colour
                .Add<Color, KnownColor>(input => input.ToKnownColor());
        }

        [Test]
        public async Task EndToEnd()
        {
            // Execute the flow using the solids list as the input
            KnownColor result = await Flow.Execute(Solids);

            Console.WriteLine(result);

            // After execution completes we should see "Fuchsia" printed to the console
            var expect = KnownColor.Fuchsia;
            Assert.AreEqual(expect, result, "Result was not Fuchsia");
        }

        [TestCase(100)]
        public void Multiple_EndToEnd(int count)
        {
            var tasks = new List<Task<KnownColor>>();

            for (int i = 0; i < count; i++)
            {
                // Execute the flow using the solids list as the input
                tasks.Add(Flow.Execute(Solids));
            }

            // Wait for all to complete
            Task.WaitAll(tasks.ToArray());

            // Extract the results
            List<KnownColor> results = tasks.Select(x => x.Result).ToList();
            Console.WriteLine(results);

            // After execution completes we should see "Fuchsia" printed to the console
            var expect = KnownColor.Fuchsia;
            Assert.AreEqual(count, results.Count(x => x == expect), $"Expected {count} results to equal Fuchsia");
        }

        static List<Cuboid> CuboidFilter(List<Solid> input)
        {
            return input.Where(x => x is Cuboid)
                .Cast<Cuboid>()
                .ToList();
        }
    }
}