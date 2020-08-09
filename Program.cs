using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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

            var flow = CreateFlow(
                resultCallback: res => Console.WriteLine(res));
            
            flow.Post(solids);
            // After execution completes we should see "Fuchsia" printed to the console
        }

        public static TransformBlock<List<Solid>, List<Cuboid>> CreateFlow(Action<KnownColor> resultCallback)
        {
            // Cuboids only
            var step1 = new TransformBlock<List<Solid>, List<Cuboid>>(input => CuboidFilter(input));

            // Mass greater than 2
            var step2 = new TransformBlock<List<Cuboid>, List<Cuboid>>(input => input.Where(x => x.Mass > 2).ToList());

            // Colours
            var step3 = new TransformBlock<List<Cuboid>, List<Color>>(input => input.Select(x => x.Color).ToList());

            // At least half red
            var step4 = new TransformBlock<List<Color>, List<Color>>(input => input.Where(x => x.R >= 128).ToList());

            // Order by blue
            var step5 = new TransformBlock<List<Color>, List<Color>>(input => input.OrderByDescending(x => x.B).ToList());

            // Highest blue
            var step6 = new TransformBlock<List<Color>, Color>(input => input.First());

            // Name of the colour
            var step7 = new TransformBlock<Color, KnownColor>(input => input.ToKnownColor());

            var callbackStep = new ActionBlock<KnownColor>(resultCallback);

            step1.LinkTo(step2, new DataflowLinkOptions());
            step2.LinkTo(step3, new DataflowLinkOptions());
            step3.LinkTo(step4, new DataflowLinkOptions());
            step4.LinkTo(step5, new DataflowLinkOptions());
            step5.LinkTo(step6, new DataflowLinkOptions());
            step6.LinkTo(step7, new DataflowLinkOptions());
            step7.LinkTo(callbackStep);
            return step1;
        }

        static List<Cuboid> CuboidFilter(List<Solid> input)
        {
            return input.Where(x => x is Cuboid)
                .Cast<Cuboid>()
                .ToList();
        }
    }
}
