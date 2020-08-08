using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace Flow
{
    class Program
    {
        static void Main(string[] args)
        {
            BlockingCollectionExample();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void BlockingCollectionExample()
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

            var flow = new Flow<List<Solid>, KnownColor>((start, builder) => start

                // Cuboids only
                .Step(builder, input => CuboidFilter(input))

                // Mass greater than 2
                .Step(builder, input => input.Where(x => x.Mass > 2).ToList())

                // Colours
                .Step(builder, input => input.Select(x => x.Color).ToList())

                // At least half red
                .Step(builder, input => input.Where(x => x.R >= 128).ToList())

                // Order by blue
                .Step(builder, input => input.OrderByDescending(x => x.B).ToList())

                // Highest blue
                .Step(builder, input => input.First())

                // Name of the colour
                .Step(builder, input => input.ToKnownColor())
            );

            flow.Finished += res => Console.WriteLine(res);

            flow.Execute(solids);
            // After execution completes we should see "Fuchsia" printed to the console
        }


        static List<Cuboid> CuboidFilter(List<Solid> input)
        {
            return input.Where(x => x is Cuboid)
                .Cast<Cuboid>()
                .ToList();
        }
    }
}
