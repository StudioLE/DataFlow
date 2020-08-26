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
    public class DataFlowTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task DataFlow_EndToEnd()
        {
            var solids = new List<Solid>()
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

            var flow = new DataFlow<List<Solid>, KnownColor>()

                // Cuboids only
                .Add<List<Solid>, List<Cuboid>>(input => CuboidFilter(input))

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
                .Add<Color, KnownColor>(input => input.ToKnownColor())

                // Set the last step as the result
                .Create();

            // Execute the flow using the solids list as the input
            var result = await flow.Execute(solids);

            Console.WriteLine(result);
            // After execution completes we should see "Fuchsia" printed to the console

            var expect = KnownColor.Fuchsia;
            Assert.AreEqual(expect, result, "Result was not Fuchsia");
        }

        static List<Cuboid> CuboidFilter(List<Solid> input)
        {
            return input.Where(x => x is Cuboid)
                .Cast<Cuboid>()
                .ToList();
        }


    }
}